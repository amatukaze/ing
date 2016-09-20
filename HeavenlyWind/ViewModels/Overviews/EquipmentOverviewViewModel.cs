using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class EquipmentOverviewViewModel : WindowViewModel, IDisposable
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

        Dictionary<EquipmentIconType, EquipmentTypeViewModel> r_TypeMap;
        public IList<EquipmentTypeViewModel> Types { get; private set; }

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
                            rType.SetIsSelectedWithoutCallback(r_SelectAllTypes.Value);

                        UpdateSelection();
                        OnPropertyChanged(nameof(SelectAllTypes));
                    }
                }
            }
        }

        Dictionary<EquipmentInfo, EquipmentGroupByMasterID> r_EquipmentMap;
        public IList<EquipmentGroupByMasterID> Equipment { get; private set; }

        internal EquipmentOverviewViewModel()
        {
            r_TypeMap = KanColleGame.Current.MasterInfo.Equipment.Values.Select(r => r.Icon).Distinct().ToDictionary(IdentityFunction<EquipmentIconType>.Instance, r => new EquipmentTypeViewModel(r) { IsSelectedChangedCallback = UpdateSelection });
            Types = r_TypeMap.Values.ToArray();

            var rSelectedTypes = Preference.Instance.Game.SelectedEquipmentTypes.Value;
            if (rSelectedTypes != null)
                foreach (var rID in rSelectedTypes)
                {
                    EquipmentTypeViewModel rTypeVM;
                    if (r_TypeMap.TryGetValue((EquipmentIconType)rID, out rTypeVM))
                        rTypeVM.SetIsSelectedWithoutCallback(true);
                }

            r_UpdateSubscription = r_UpdateObservable.Do(_ => IsLoading = true).Throttle(TimeSpan.FromSeconds(.75)).Subscribe(_ => UpdateCore());

            UpdateSelectionCore();
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
        }

        void UpdateSelection()
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

            r_UpdateObservable.OnNext(Unit.Default);
        }

        void UpdateCore()
        {
            if (r_SelectAllTypes.HasValue && !r_SelectAllTypes.Value)
                Equipment = null;
            else
            {
                var rGame = KanColleGame.Current;
                var rShips = rGame.Port.Ships.Values;

                r_EquipmentMap = rGame.Port.Equipment.Values.GroupBy(r => r.Info).Where(r => r_TypeMap[r.Key.Icon].IsSelected).OrderBy(r => r.Key.Type).ThenBy(r => r.Key.ID)
                    .ToDictionary(r => r.Key, r => new EquipmentGroupByMasterID(r.Key, r_TypeMap[r.Key.Icon], r));

                foreach (var rShip in rShips)
                    foreach (var rEquipment in rShip.EquipedEquipment)
                    {
                        EquipmentGroupByMasterID rGroup;
                        if (r_EquipmentMap.TryGetValue(rEquipment.Info, out rGroup))
                            rGroup.Update(rShip, new EquipmentGroupingKey(rEquipment.Level, rEquipment.Proficiency));
                    }

                Equipment = r_EquipmentMap.Values.Where(r => r.Type.IsSelected).ToArray();
            }

            OnPropertyChanged(nameof(Equipment));
            IsLoading = false;
        }
    }
}
