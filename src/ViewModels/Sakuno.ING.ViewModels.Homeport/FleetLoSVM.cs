using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class FleetLoSVM : BindableObject
    {
        public IReadOnlyList<FleetLoSVMItem> Selections { get; } = new FleetLoSVMItem[]
        {
            new FleetLoSVMItem("Simple", (e, s) => s),
            new FleetLoSVMItem("Factor1", (e, s) => e.MultiplyWith(1)),
            new FleetLoSVMItem("Factor2", (e, s) => e.MultiplyWith(2)),
            new FleetLoSVMItem("Factor3", (e, s) => e.MultiplyWith(3)),
            new FleetLoSVMItem("Factor4", (e, s) => e.MultiplyWith(4)),
        };

        private FleetLoSVMItem _selectedItem;
        public FleetLoSVMItem SelectedItem
        {
            get => _selectedItem;
            set => Set(ref _selectedItem, value);
        }

        private int _simple;
        public int Simple
        {
            get => _simple;
            set
            {
                Set(ref _simple, value);
                foreach (var s in Selections)
                    s.Update(Effective, Simple);
            }
        }

        private LineOfSight _effective;
        public LineOfSight Effective
        {
            get => _effective;
            set
            {
                Set(ref _effective, value);
                foreach (var s in Selections)
                    s.Update(Effective, Simple);
            }
        }

        public FleetLoSVM()
        {
            SelectedItem = Selections[1];
        }
    }

    public class FleetLoSVMItem : BindableObject
    {
        public string Id { get; }
        public double DisplayingValue { get; private set; }
        private readonly Func<LineOfSight, int, double> selector;

        internal FleetLoSVMItem(string id, Func<LineOfSight, int, double> selector)
        {
            Id = id;
            this.selector = selector;
        }

        internal void Update(LineOfSight effective, int simple)
        {
            DisplayingValue = selector(effective, simple);
            NotifyPropertyChanged(nameof(DisplayingValue));
        }
    }
}
