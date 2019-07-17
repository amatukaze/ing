using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.ViewModels.Catalog
{
    [Export(typeof(EquipmentCatalogVM), SingleInstance = false)]
    public class EquipmentCatalogVM : BindableObject
    {
        private readonly ITable<EquipmentId, HomeportEquipment> source;

        private class LifeTimeTracker
        {
            private readonly WeakReference<EquipmentCatalogVM> weak;
            private readonly IUpdationSource source;

            public LifeTimeTracker(IUpdationSource source, EquipmentCatalogVM vm)
            {
                weak = new WeakReference<EquipmentCatalogVM>(vm);
                this.source = source;
            }

            public void Update()
            {
                if (weak.TryGetTarget(out var target))
                    target.Update();
                else
                    source.Updated -= Update;
            }
        }

        public EquipmentCatalogVM(NavalBase navalBase)
        {
            source = navalBase.AllEquipment;
            source.Updated += new LifeTimeTracker(source, this).Update;
            IconFilters = navalBase.MasterData.EquipmentInfos
                .Select(x => x.IconId)
                .Distinct().OrderBy(x => x)
                .Select(x => new EquipmentIconHolder(this, x))
                .ToArray();
            Update();
        }

        private void Update()
        {
            topLevel = source
                .GroupBy(x => x.Info)
                .Select(x => new EquipmentGroup(x))
                .OrderBy(x => x.Info.Type.Id)
                .ThenBy(x => x.Info.Id)
                .ToArray();
            UpdateFilters();
        }

        internal void UpdateFilters()
        {
            Groups = topLevel
                .Where(x => IconFilters.FirstOrDefault(i => i.IconId == x.Info.IconId)?.IsSelected != false)
                .ToArray();

            bool allTrue = true, allFalse = true;
            foreach (var f in IconFilters)
                if (f.IsSelected)
                    allFalse = false;
                else
                    allTrue = false;

            FilterAll = (allTrue, allFalse) switch
            {
                (true, false) => true,
                (false, true) => false,
                _ => (bool?)null
            };
        }

        public IReadOnlyList<EquipmentIconHolder> IconFilters { get; }

        private EquipmentGroup[] topLevel;

        private IReadOnlyList<EquipmentGroup> groups;
        public IReadOnlyList<EquipmentGroup> Groups
        {
            get => groups;
            private set => Set(ref groups, value);
        }

        private bool? filterAll;
        public bool? FilterAll
        {
            get => filterAll;
            set
            {
                Set(ref filterAll, value);
                if (value is bool v)
                {
                    foreach (var filter in IconFilters)
                        filter.IsSelected = v;
                }
            }
        }
    }

    public class EquipmentGroup
    {
        internal EquipmentGroup(IGrouping<EquipmentInfo, HomeportEquipment> group)
        {
            Info = group.Key;
            SubGroups = group
                .GroupBy(x => (x.ImprovementLevel, x.AirProficiency))
                .Select(x => new ImprovementGroup(x))
                .OrderBy(x => x.ImprovementLevel)
                .ThenBy(x => x.AirProficiency)
                .ToArray();
            Available = SubGroups.Sum(x => x.Available);
            Total = group.Count();
        }

        public EquipmentInfo Info { get; }
        public int Available { get; }
        public int Total { get; }
        public IReadOnlyList<ImprovementGroup> SubGroups { get; }
    }

    public class ImprovementGroup
    {
        internal ImprovementGroup(IGrouping<(int ImprovementLevel, int AirProficiency), HomeportEquipment> group)
        {
            ImprovementLevel = group.Key.ImprovementLevel;
            AirProficiency = group.Key.AirProficiency;
            Available = group.Count(x => x.Slot is null);
            SubGroups = group
                .Where(x => x.Slot != null)
                .GroupBy(x => x.Slot.Ship)
                .Select(x => new ShipGroup(x.Key, x.Count()))
                .ToArray();
            Total = group.Count();
        }

        public int ImprovementLevel { get; }
        public int AirProficiency { get; }
        public int Available { get; }
        public int Total { get; }
        public IReadOnlyList<ShipGroup> SubGroups { get; }
    }

    public class ShipGroup
    {
        public ShipGroup(HomeportShip ship, int count)
        {
            Ship = ship;
            Count = count;
        }

        public HomeportShip Ship { get; }
        public int Count { get; }
    }

    public class EquipmentIconHolder : BindableObject
    {
        private readonly EquipmentCatalogVM owner;

        internal EquipmentIconHolder(EquipmentCatalogVM owner, int iconId)
        {
            this.owner = owner;
            IconId = iconId;
        }

        public int IconId { get; }

        private bool isSelected = true;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    NotifyPropertyChanged();
                    owner.UpdateFilters();
                }
            }
        }
    }
}
