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
    class ShipOverviewViewModel : WindowViewModel, IDisposable
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

        public Dictionary<ShipTypeInfo, FilterTypeViewModel<ShipTypeInfo>> TypeMaps { get; }
        public IList<FilterTypeViewModel<ShipTypeInfo>> Types { get; private set; }

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

        ShipCollectionView r_Ships;
        public IEnumerable<ShipViewModel> Ships { get; }

        public int ShipLockingColumnWidth => ShipLockingService.Instance?.ShipLocking?.Count > 0 && KanColleGame.Current.MasterInfo.EventMapCount > 0 ? 30 : 0;

        bool r_ExceptExpeditionShips = Preference.Instance.Game.ShipOverview_ExceptExpeditionShips;
        public bool ExceptExpeditionShips
        {
            get { return r_ExceptExpeditionShips; }
            set
            {
                if (r_ExceptExpeditionShips != value)
                {
                    r_ExceptExpeditionShips = value;
                    OnPropertyChanged(nameof(ExceptExpeditionShips));

                    Preference.Instance.Game.ShipOverview_ExceptExpeditionShips.Value = value;

                    Refresh();
                }
            }
        }

        bool r_ExceptSparklingShips = Preference.Instance.Game.ShipOverview_ExceptSparklingShips;
        public bool ExceptSparklingShips
        {
            get { return r_ExceptSparklingShips; }
            set
            {
                if (r_ExceptSparklingShips != value)
                {
                    r_ExceptSparklingShips = value;
                    OnPropertyChanged(nameof(ExceptSparklingShips));

                    Preference.Instance.Game.ShipOverview_ExceptSparklingShips.Value = value;

                    Refresh();
                }
            }
        }

        bool r_ExceptLevel1Ships = Preference.Instance.Game.ShipOverview_ExceptLevel1Ships;
        public bool ExceptLevel1Ships
        {
            get { return r_ExceptLevel1Ships; }
            set
            {
                if (r_ExceptLevel1Ships != value)
                {
                    r_ExceptLevel1Ships = value;
                    OnPropertyChanged(nameof(ExceptLevel1Ships));

                    Preference.Instance.Game.ShipOverview_ExceptLevel1Ships.Value = value;

                    Refresh();
                }
            }
        }

        bool r_ExceptMaxModernizationShips = Preference.Instance.Game.ShipOverview_ExceptMaxModernizationShips;
        public bool ExceptMaxModernizationShips
        {
            get { return r_ExceptMaxModernizationShips; }
            set
            {
                if (r_ExceptMaxModernizationShips != value)
                {
                    r_ExceptMaxModernizationShips = value;
                    OnPropertyChanged(nameof(ExceptMaxModernizationShips));

                    Preference.Instance.Game.ShipOverview_ExceptMaxModernizationShips.Value = value;

                    Refresh();
                }
            }
        }

        IDisposable r_HomeportSubscription;

        public bool IsOppositeOrder { get; set; }

        public ShipOverviewViewModel()
        {
            TypeMaps = KanColleGame.Current.MasterInfo.ShipTypes.Values.Where(r => r.ID != 12 && r.ID != 15).ToDictionary(IdentityFunction<ShipTypeInfo>.Instance, r => new FilterTypeViewModel<ShipTypeInfo>(r));
            Types = TypeMaps.Values.ToArray();

            r_Ships = new ShipCollectionView(this);
            Ships = r_Ships;

            var rSelectedTypes = Preference.Instance.Game.SelectedShipTypes.Value;
            if (rSelectedTypes != null)
                foreach (var rID in rSelectedTypes)
                {
                    ShipTypeInfo rShipType;
                    FilterTypeViewModel<ShipTypeInfo> rTypeVM;
                    if (KanColleGame.Current.MasterInfo.ShipTypes.TryGetValue(rID, out rShipType) && TypeMaps.TryGetValue(rShipType, out rTypeVM))
                        rTypeVM.IsSelected = true;
                }

            r_UpdateSubscription = r_UpdateObservable
                .Do(_ => IsLoading = true)
                .Throttle(TimeSpan.FromSeconds(.5))
                .Do(_ => r_Ships.Refresh())
                .Do(_ => IsLoading = false)
                .Subscribe();

            UpdateSelectionCore();

            r_HomeportSubscription = ApiService.Subscribe("api_port/port", _ => Refresh());
        }

        public void Dispose()
        {
            r_Ships.Dispose();
            TypeMaps.Clear();
            Types = null;

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

        internal void Sort(GridViewColumnHeader rpColumnHeader)
        {
            var rColumn = Data.GetContent(rpColumnHeader.Column);
            if (rColumn == null)
                return;

            if (r_Ships.SortingColumn == rColumn)
                IsOppositeOrder = !IsOppositeOrder;
            else
            {
                r_Ships.SortingColumn = rColumn;
                IsOppositeOrder = false;
            }

            Refresh();
        }

        public void UpdateSelection()
        {
            UpdateSelectionCore();

            Preference.Instance.Game.SelectedShipTypes.Value = Types.Where(r => r.IsSelected).Select(r => r.Type.ID).ToArray();
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

        void Refresh() => r_UpdateObservable.OnNext(Unit.Default);
    }
}
