using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using Sakuno.KanColle.Amatsukaze.Views.Overviews;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(Overview))]
    public class OverviewViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Overview; }
            protected set { throw new NotImplementedException(); }
        }

        bool r_IsAdmiralInitialized;

        public AdmiralViewModel Admiral { get; } = new AdmiralViewModel();
        public MaterialsViewModel Materials { get; } = new MaterialsViewModel();

        int r_ShipCount;
        public int ShipCount
        {
            get { return r_ShipCount; }
            private set
            {
                if (r_ShipCount != value)
                {
                    r_ShipCount = value;
                    OnPropertyChanged(nameof(ShipCount));

                    if (Admiral.Source != null)
                        CheckShipCapacity();
                }
            }
        }
        bool r_ShowShipCountWarning;
        public bool ShowShipCountWarning
        {
            get { return r_ShowShipCountWarning; }
            set
            {
                if (r_ShowShipCountWarning != value)
                {
                    r_ShowShipCountWarning = value;
                    OnPropertyChanged(nameof(ShowShipCountWarning));
                }
            }
        }

        int r_EquipmentCount;
        public int EquipmentCount
        {
            get { return r_EquipmentCount; }
            private set
            {
                if (r_EquipmentCount != value)
                {
                    r_EquipmentCount = value;
                    OnPropertyChanged(nameof(EquipmentCount));

                    if (Admiral.Source != null)
                        CheckEquipmentCapacity();
                }
            }
        }
        bool r_ShowEquipmentCountWarning;
        public bool ShowEquipmentCountWarning
        {
            get { return r_ShowEquipmentCountWarning; }
            set
            {
                if (r_ShowEquipmentCountWarning != value)
                {
                    r_ShowEquipmentCountWarning = value;
                    OnPropertyChanged(nameof(ShowEquipmentCountWarning));
                }
            }
        }

        IReadOnlyList<FleetViewModel> r_Fleets;
        public IReadOnlyList<FleetViewModel> Fleets
        {
            get { return r_Fleets; }
            internal set
            {
                if (r_Fleets != value)
                {
                    r_Fleets = value;
                    OnPropertyChanged(nameof(Fleets));
                }
            }
        }

        public AirBaseViewModel AirBase { get; }

        PropertyChangedEventListener r_AirBasePCEL;

        public IList<ModelBase> RightTabs { get; private set; }

        ModelBase r_SelectedTab;
        public ModelBase SelectedTab
        {
            get { return r_SelectedTab; }
            set
            {
                if (r_SelectedTab != value)
                {
                    r_SelectedTab = value;
                    OnPropertyChanged(nameof(SelectedTab));
                }
            }
        }

        IReadOnlyCollection<RepairDockViewModel> r_RepairDocks;
        public IReadOnlyCollection<RepairDockViewModel> RepairDocks
        {
            get { return r_RepairDocks; }
            private set
            {
                if (r_RepairDocks != value)
                {
                    r_RepairDocks = value;
                    OnPropertyChanged(nameof(RepairDocks));
                }
            }
        }
        IReadOnlyCollection<ConstructionDockViewModel> r_ConstructionDocks;
        public IReadOnlyCollection<ConstructionDockViewModel> ConstructionDocks
        {
            get { return r_ConstructionDocks; }
            private set
            {
                if (r_ConstructionDocks != value)
                {
                    r_ConstructionDocks = value;
                    OnPropertyChanged(nameof(ConstructionDocks));
                }
            }
        }

        IList<QuestViewModel> r_ActiveQuests;
        public IList<QuestViewModel> ActiveQuests
        {
            get { return r_ActiveQuests; }
            internal set
            {
                if (r_ActiveQuests != value)
                {
                    r_ActiveQuests = value;
                    OnPropertyChanged(nameof(ActiveQuests));
                }
            }
        }

        public ICommand ShowShipOverviewWindowCommand { get; }
        public ICommand ShowEquipmentOverviewWindowCommand { get; }

        internal OverviewViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            var rPortPCEL = PropertyChangedEventListener.FromSource(rPort);
            rPortPCEL.Add(nameof(rPort.Ships), (s, e) => ShipCount = rPort.Ships.Count);
            rPortPCEL.Add(nameof(rPort.Equipment), (s, e) => EquipmentCount = rPort.Equipment.Count);
            rPortPCEL.Add(nameof(rPort.RepairDocks), (s, e) => RepairDocks = rPort.RepairDocks.Values.Select(r => new RepairDockViewModel(r)).ToList());
            rPortPCEL.Add(nameof(rPort.ConstructionDocks), (s, e) => ConstructionDocks = rPort.ConstructionDocks.Values.Select(r => new ConstructionDockViewModel(r)).ToList());
            rPortPCEL.Add(nameof(rPort.Admiral), delegate
            {
                if (!r_IsAdmiralInitialized)
                {
                    var rAdmiral = rPort.Admiral;
                    var rAdmiralPCEL = PropertyChangedEventListener.FromSource(rAdmiral);
                    rAdmiralPCEL.Add(nameof(rAdmiral.MaxShipCount), (s, e) => CheckShipCapacity());
                    rAdmiralPCEL.Add(nameof(rAdmiral.MaxEquipmentCount), (s, e) => CheckEquipmentCapacity());

                    r_IsAdmiralInitialized = true;
                }

                CheckCapacity();
            });

            AirBase = new AirBaseViewModel();

            r_AirBasePCEL = PropertyChangedEventListener.FromSource(rPort.AirBase);
            r_AirBasePCEL.Add(nameof(rPort.AirBase.AllGroups), delegate
            {
                if (rPort.AirBase.Table.Count == 0)
                    return;

                DispatcherUtil.UIDispatcher.InvokeAsync(() =>
                {
                    if (RightTabs == null)
                    {
                        RightTabs = new ObservableCollection<ModelBase>();
                        OnPropertyChanged(nameof(RightTabs));
                    }

                    RightTabs.Add(AirBase);
                });

                r_AirBasePCEL.Dispose();
                r_AirBasePCEL = null;
            });

            ApiService.Subscribe("api_req_map/next", delegate
            {
                var rSortie = SortieInfo.Current;
                if (rSortie != null)
                    ShipCount = rPort.Ships.Count + rSortie.PendingShipCount;
            });

            ShowShipOverviewWindowCommand = new DelegatedCommand(() => WindowService.Instance.Show<ShipOverviewWindow>());
            ShowEquipmentOverviewWindowCommand = new DelegatedCommand(() => WindowService.Instance.Show<EquipmentOverviewWindow>());
        }

        void CheckShipCapacity() => ShowShipCountWarning = r_ShipCount > Admiral.Source.MaxShipCount - 5;
        void CheckEquipmentCapacity() =>ShowEquipmentCountWarning = r_EquipmentCount > Admiral.Source.MaxEquipmentCount - 17;
        void CheckCapacity()
        {
            CheckShipCapacity();
            CheckEquipmentCapacity();
        }
    }
}
