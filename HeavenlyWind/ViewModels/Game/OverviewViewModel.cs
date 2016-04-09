using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using Sakuno.KanColle.Amatsukaze.Views.Overviews;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
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

        IReadOnlyCollection<QuestViewModel> r_ExecutingQuests;
        public IReadOnlyCollection<QuestViewModel> ExecutingQuests
        {
            get { return r_ExecutingQuests; }
            internal set
            {
                if (r_ExecutingQuests != value)
                {
                    r_ExecutingQuests = value;
                    OnPropertyChanged(nameof(ExecutingQuests));
                }
            }
        }

        public ICommand ShowShipOverviewWindowCommand { get; }
        public ICommand ShowEquipmentOverviewWindowCommand { get; }

        internal OverviewViewModel()
        {
            var rPort = KanColleGame.Current.Port;

            var rPortPCEL = PropertyChangedEventListener.FromSource(rPort);
            rPortPCEL.Add(nameof(rPort.Ships), delegate
            {
                var rCount = rPort.Ships.Count;
                if (KanColleGame.Current.Sortie != null)
                    rCount += KanColleGame.Current.Sortie.PendingShipCount;

                ShipCount = rCount;
            });
            rPortPCEL.Add(nameof(rPort.Equipment), (s, e) => EquipmentCount = rPort.Equipment.Count);
            rPortPCEL.Add(nameof(rPort.RepairDocks), (s, e) => RepairDocks = rPort.RepairDocks.Values.Select(r => new RepairDockViewModel(r)).ToList());
            rPortPCEL.Add(nameof(rPort.ConstructionDocks), (s, e) => ConstructionDocks = rPort.ConstructionDocks.Values.Select(r => new ConstructionDockViewModel(r)).ToList());

            ShowShipOverviewWindowCommand = new DelegatedCommand(ShowShipOverviewWindow);
            ShowEquipmentOverviewWindowCommand = new DelegatedCommand(ShowEquipmentOverviewWindow);
        }

        void ShowShipOverviewWindow()
        {
            new ShipOverviewWindow().Show();
        }
        void ShowEquipmentOverviewWindow()
        {
            new EquipmentOverviewWindow().Show();
        }

    }
}
