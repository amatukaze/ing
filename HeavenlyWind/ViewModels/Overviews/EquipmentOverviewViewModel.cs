using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class EquipmentOverviewViewModel : WindowViewModel, IDisposable
    {
        Subject<Unit> r_UpdateObservable = new Subject<Unit>();
        IDisposable r_UpdateSubscription;

        bool r_IsLoading;
        public bool IsLoading
        {
            get { return r_IsLoading; }
            private set
            {
                if (r_IsLoading != value)
                {
                    r_IsLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        Dictionary<EquipmentIconType, FilterTypeViewModel<EquipmentIconType>> r_TypeMap;
        public IList<FilterTypeViewModel<EquipmentIconType>> Types { get; private set; }

        bool? r_SelectAllTypes = false;
        public bool? SelectAllTypes
        {
            get { return r_SelectAllTypes; }
            set
            {
                if (r_SelectAllTypes != value)
                {
                    r_SelectAllTypes = value;
                    if (r_SelectAllTypes.HasValue)
                    {
                        foreach (var rType in Types)
                            rType.IsSelected = r_SelectAllTypes.Value;

                        UpdateSelection();
                        OnPropertyChanged(nameof(SelectAllTypes));
                    }
                }
            }
        }

        Dictionary<EquipmentInfo, EquipmentGroupByMasterID> r_EquipmentMap;
        public IList<EquipmentGroupByMasterID> Equipment { get; private set; }

        IDisposable r_HomeportSubscription;

        internal EquipmentOverviewViewModel()
        {
            r_TypeMap = KanColleGame.Current.MasterInfo.Equipment.Values.Select(r => r.Icon).Distinct().ToDictionary(IdentityFunction<EquipmentIconType>.Instance, r => new FilterTypeViewModel<EquipmentIconType>(r));
            Types = r_TypeMap.Values.ToArray();

            var rSelectedTypes = Preference.Instance.Game.SelectedEquipmentTypes.Value;
            if (rSelectedTypes != null)
                foreach (var rID in rSelectedTypes)
                {
                    FilterTypeViewModel<EquipmentIconType> rTypeVM;
                    if (r_TypeMap.TryGetValue((EquipmentIconType)rID, out rTypeVM))
                        rTypeVM.IsSelected = true;
                }

            r_UpdateSubscription = r_UpdateObservable.Do(_ => IsLoading = true).Throttle(TimeSpan.FromSeconds(.75)).Subscribe(_ => UpdateCore());

            UpdateSelectionCore();

            r_HomeportSubscription = ApiService.Subscribe("api_port/port", _ => Refresh());
        }

        public void Dispose()
        {
            r_TypeMap.Clear();
            Types = null;
            r_EquipmentMap?.Clear();
            Equipment = null;

            if (r_UpdateSubscription != null)
            {
                r_UpdateSubscription.Dispose();
                r_UpdateSubscription = null;
            }

            if (r_HomeportSubscription != null)
            {
                r_HomeportSubscription.Dispose();
                r_HomeportSubscription = null;
            }
        }

        public void UpdateSelection()
        {
            UpdateSelectionCore();

            Preference.Instance.Game.SelectedEquipmentTypes.Value = Types.Where(r => r.IsSelected).Select(r => (int)r.Type).ToArray();
        }
        void UpdateSelectionCore()
        {
            var rTypeCount = Types.Count;
            var rSelectedCount = Types.Count(r => r.IsSelected);

            if (rSelectedCount == 0)
                r_SelectAllTypes = false;
            else if (rSelectedCount == rTypeCount)
                r_SelectAllTypes = true;
            else
                r_SelectAllTypes = null;

            OnPropertyChanged(nameof(SelectAllTypes));

            Refresh();
        }

        public void Filter(FilterTypeViewModel<EquipmentIconType> rpType)
        {
            foreach (var rType in Types)
                rType.IsSelected = rType == rpType;

            UpdateSelection();
        }

        void UpdateCore()
        {
            if (r_SelectAllTypes.HasValue && !r_SelectAllTypes.Value)
                Equipment = null;
            else
            {
                var rGame = KanColleGame.Current;
                var rShips = rGame.Port.Ships.Values;

                r_EquipmentMap = rGame.Port.Equipment.Values.GroupBy(r => r.Info).Where(r => r_TypeMap.GetValueOrDefault(r.Key.Icon)?.IsSelected ?? false).OrderBy(r => r.Key.Type).ThenBy(r => r.Key.ID)
                    .ToDictionary(r => r.Key, r => new EquipmentGroupByMasterID(r.Key, r_TypeMap[r.Key.Icon], r));

                foreach (var rShip in rShips)
                    foreach (var rEquipment in rShip.EquipedEquipment)
                    {
                        EquipmentGroupByMasterID rGroup;
                        if (r_EquipmentMap.TryGetValue(rEquipment.Info, out rGroup))
                            rGroup.Update(rShip, new EquipmentGroupingKey(rEquipment.Level, rEquipment.Proficiency));
                    }

                Equipment = r_EquipmentMap.Values.Where(r => r.Type.IsSelected).OrderBy(r => r.Info.Icon).ToArray();
            }

            OnPropertyChanged(nameof(Equipment));
            IsLoading = false;
        }

        void Refresh() => r_UpdateObservable.OnNext(Unit.Default);
    }
}
