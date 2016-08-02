using Sakuno.KanColle.Amatsukaze.Controls;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    public class ShipOverviewViewModel : WindowViewModel, IDisposable
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

        Dictionary<ShipTypeInfo, ShipTypeViewModel> r_TypeMap;
        public IReadOnlyCollection<ShipTypeViewModel> Types { get; }

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

        Dictionary<int, ShipViewModel> r_ShipVMs = new Dictionary<int, ShipViewModel>(100);
        ShipViewModel[] r_Ships;
        public IReadOnlyCollection<ShipViewModel> Ships { get; private set; }

        string r_SortingColumn;

        public int ShipLockingColumnWidth => ShipLockingService.Instance?.ShipLocking?.Count > 0 && KanColleGame.Current.MasterInfo.EventMapCount > 0 ? 30 : 0;

        bool r_ExceptExpeditionShips;
        public bool ExceptExpeditionShips
        {
            get { return r_ExceptExpeditionShips; }
            set
            {
                if(r_ExceptExpeditionShips != value)
                {
                    r_ExceptExpeditionShips = value;
                    OnPropertyChanged(nameof(ExceptExpeditionShips));
                    r_UpdateObservable.OnNext(Unit.Default);
                }
            }
        }

        bool r_ExceptSparklingShips;
        public bool ExceptSparklingShips
        {
            get { return r_ExceptSparklingShips; }
            set
            {
                if (r_ExceptSparklingShips != value)
                {
                    r_ExceptSparklingShips = value;
                    OnPropertyChanged(nameof(ExceptSparklingShips));
                    r_UpdateObservable.OnNext(Unit.Default);
                }
            }
        }

        bool r_ExceptLevel1Ships;
        public bool ExceptLevel1Ships
        {
            get { return r_ExceptLevel1Ships; }
            set
            {
                if (r_ExceptLevel1Ships != value)
                {
                    r_ExceptLevel1Ships = value;
                    OnPropertyChanged(nameof(ExceptLevel1Ships));
                    r_UpdateObservable.OnNext(Unit.Default);
                }
            }
        }

        internal ShipOverviewViewModel()
        {
            r_TypeMap = KanColleGame.Current.MasterInfo.ShipTypes.Values.Where(r => r.ID != 12 && r.ID != 15).ToDictionary(IdentityFunction<ShipTypeInfo>.Instance, r => new ShipTypeViewModel(r) { IsSelectedChangedCallback = UpdateSelection });
            Types = r_TypeMap.Values.ToArray().AsReadOnly();

            var rSelectedTypes = Preference.Instance.Game.SelectedShipTypes.Value;
            if (rSelectedTypes != null)
                foreach (var rID in rSelectedTypes)
                {
                    ShipTypeInfo rShipType;
                    ShipTypeViewModel rTypeVM;
                    if (KanColleGame.Current.MasterInfo.ShipTypes.TryGetValue(rID, out rShipType) && r_TypeMap.TryGetValue(rShipType, out rTypeVM))
                        rTypeVM.SetIsSelectedWithoutCallback(true);
                }

            r_UpdateSubscription = r_UpdateObservable.Do(_ => IsLoading = true).Throttle(TimeSpan.FromSeconds(.75)).Subscribe(_ => UpdateCore());

            UpdateSelectionCore();
        }

        public void Dispose()
        {
            if (r_UpdateSubscription != null)
            {
                r_UpdateSubscription.Dispose();
                r_UpdateSubscription = null;
            }
        }

        internal void Sort(GridViewColumnHeader rpColumnHeader)
        {
            var rColumn = Data.GetContent(rpColumnHeader.Column);
            if (rColumn == null)
                return;

            r_SortingColumn = rColumn;
            r_UpdateObservable.OnNext(Unit.Default);
        }

        void UpdateSelection()
        {
            UpdateSelectionCore();

            Preference.Instance.Game.SelectedShipTypes.Value = Types.Where(r => r.IsSelected).Select(r => r.ID).ToArray();
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
                Ships = null;
            else
            {
                r_Ships = KanColleGame.Current.Port.Ships.Values.Select(r =>
                {
                    ShipViewModel rResult;
                    if (!r_ShipVMs.TryGetValue(r.ID, out rResult))
                        rResult = new ShipViewModel(r, r_TypeMap[r.Info.Type]);

                    return rResult;
                }).ToArray();

                var rShips = r_Ships.Where(r => r.Type.IsSelected)
                    .Where(r => !ExceptExpeditionShips || (r.Ship.State & ShipState.Expedition) == 0)
                    .Where(r => !ExceptSparklingShips || (r.Ship.Condition < 50))
                    .Where(r => !ExceptLevel1Ships || (r.Ship.Level > 1));
                switch (r_SortingColumn)
                {
                    case "ID":
                        rShips = rShips.OrderBy(r => r.Ship.ID);
                        break;

                    case "Name":
                        rShips = rShips.OrderBy(r => r.Ship.Info.Name);
                        break;

                    case "ShipLocking":
                        rShips = rShips.OrderByDescending(r => r.Ship.RawData.LockingTag != 0).ThenBy(r => r.Ship.RawData.LockingTag);
                        break;

                    case "Level":
                        rShips = rShips.OrderByDescending(r => r.Ship.Level).ThenBy(r => r.Ship.ExperienceToNextLevel);
                        break;

                    case "Condition":
                        rShips = rShips.OrderByDescending(r => r.Ship.Condition);
                        break;

                    case "Firepower":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.FirepowerBase.Current);
                        break;
                    case "Torpedo":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.TorpedoBase.Current);
                        break;
                    case "AA":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.AABase.Current);
                        break;
                    case "Armor":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.ArmorBase.Current);
                        break;
                    case "Luck":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.LuckBase.Current);
                        break;

                    case "Evasion":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.Evasion);
                        break;
                    case "ASW":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.ASW);
                        break;
                    case "LoS":
                        rShips = rShips.OrderByDescending(r => r.Ship.Status.LoS);
                        break;

                    case "RepairTime":
                        rShips = rShips.OrderByDescending(r => r.Ship.RepairTime);
                        break;
                }

                Ships = rShips.ToArray().AsReadOnly();
            }

            OnPropertyChanged(nameof(Ships));
            IsLoading = false;
        }
    }
}
