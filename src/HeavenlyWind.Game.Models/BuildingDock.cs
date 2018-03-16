using System;
using Sakuno.KanColle.Amatsukaze.Game.Models.MasterData;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public abstract class BuildingDock : ManualNotifyObject, IIdentifiable
    {
        protected BuildingDock(int id) => Id = id;

        public int Id { get; protected set; }
        public DateTimeOffset CompletionTime { get; protected set; }
        public BuildingDockState State { get; protected set; }

        public int FuelConsumption { get; protected set; }
        public int BulletConsumption { get; protected set; }
        public int SteelConsumption { get; protected set; }
        public int BauxiteConsumption { get; protected set; }
        public int DevelopmentConsumption { get; protected set; }
        public ShipInfo ShipBuilt { get; protected set; }
        public bool IsLSC { get; protected set; }
        public int InstantBuildConsumption { get; protected set; }
    }

    public enum BuildingDockState
    {
        Locked = -1,
        Empty = 0,
        Building = 2,
        BuildCompleted = 3,
    }
}
