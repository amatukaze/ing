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
        FilterTypeViewModel<EquipmentIconType>[] _types;

        public IList<FilterTypeViewModel<EquipmentIconType>> ArtilleryTypes { get; private set; }
        public IList<FilterTypeViewModel<EquipmentIconType>> OtherTypes { get; private set; }
        public IList<FilterTypeViewModel<EquipmentIconType>> AircraftTypes { get; private set; }

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
                        foreach (var rType in _types)
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

        static IComparer<EquipmentIconType> _typeComparer;

        static EquipmentOverviewViewModel()
        {
            _typeComparer = new DelegatedComparer<EquipmentIconType>((x, y) =>
            {
                if (x == EquipmentIconType.SpecialAmphibiousLandingCraft && y > EquipmentIconType.LandingCraft)
                    return -1;
                if (y == EquipmentIconType.SpecialAmphibiousLandingCraft && x > EquipmentIconType.LandingCraft)
                    return 1;

                return x - y;
            });
        }
        public EquipmentOverviewViewModel()
        {
            r_TypeMap = KanColleGame.Current.MasterInfo.Equipment.Values.Select(r => r.Icon).Distinct().ToDictionary(IdentityFunction<EquipmentIconType>.Instance, r => new FilterTypeViewModel<EquipmentIconType>(r));
            _types = r_TypeMap.Values.ToArray();

            ArtilleryTypes = new[]
            {
                r_TypeMap[EquipmentIconType.SmallCaliberGun],
                r_TypeMap[EquipmentIconType.HighAngleGun],
                r_TypeMap[EquipmentIconType.MediumCaliberGun],
                r_TypeMap[EquipmentIconType.LargeCaliberGun],
                r_TypeMap[EquipmentIconType.SecondaryGun],
                r_TypeMap[EquipmentIconType.Torpedo],
                r_TypeMap[EquipmentIconType.AAGun],
                r_TypeMap[EquipmentIconType.AAShell],
                r_TypeMap[EquipmentIconType.APShell],
                r_TypeMap[EquipmentIconType.Soner],
                r_TypeMap[EquipmentIconType.ASW],
                r_TypeMap[EquipmentIconType.AAFireDirector],
                r_TypeMap[EquipmentIconType.AntiGroundArtillery],
            };
            AircraftTypes = new[]
            {
                r_TypeMap[EquipmentIconType.CarrierBasedFighter],
                r_TypeMap[EquipmentIconType.CarrierBasedDiveBomber],
                r_TypeMap[EquipmentIconType.CarrierBasedTorpedoBomber],
                r_TypeMap[EquipmentIconType.CarrierBasedRecon],
                r_TypeMap[EquipmentIconType.JetFighterBomberKeiunKai],
                r_TypeMap[EquipmentIconType.JetFighterBomberKikkaKai],
                r_TypeMap[EquipmentIconType.NightFighter],
                r_TypeMap[EquipmentIconType.NightTorpedoBomber],
                r_TypeMap[EquipmentIconType.LandBasedFighter],
                r_TypeMap[EquipmentIconType.InterceptorFighter],
                r_TypeMap[EquipmentIconType.LandBasedAttackAircraft],
                r_TypeMap[EquipmentIconType.LandBasedPatrolAircraft],
                r_TypeMap[EquipmentIconType.Seaplane],
                r_TypeMap[EquipmentIconType.SeaplaneFighter],
                r_TypeMap[EquipmentIconType.FlyingBoat],
                r_TypeMap[EquipmentIconType.Autogyro],
                r_TypeMap[EquipmentIconType.ASAircraft],
            };

            OtherTypes = _types.Except(ArtilleryTypes.Concat(AircraftTypes)).OrderBy(r => r.Type, _typeComparer).ToArray();

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
            _types = null;
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

            Preference.Instance.Game.SelectedEquipmentTypes.Value = _types.Where(r => r.IsSelected).Select(r => (int)r.Type).ToArray();
        }
        void UpdateSelectionCore()
        {
            var rTypeCount = _types.Length;
            var rSelectedCount = _types.Count(r => r.IsSelected);

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
            foreach (var rType in _types)
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
