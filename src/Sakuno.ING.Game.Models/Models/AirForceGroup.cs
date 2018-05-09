using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public class AirForceGroup : Calculated<IRawAirForceGroup>, ITableProvider
    {
        internal readonly IdTable<AirForceSquadron, IRawAirForceSquadron> squadrons;
        public ITable<AirForceSquadron> Squadrons => squadrons;

        public MapAreaInfo MapArea { get; }

        public int GroupId { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private int _distance;
        public int Distance
        {
            get => _distance;
            set => Set(ref _distance, value);
        }

        private AirForceAction _action;
        public AirForceAction Action
        {
            get => _action;
            set => Set(ref _action, value);
        }

        public AirForceGroup(int id, ITableProvider owner)
            : base(id, owner)
        {
            MapArea = owner.GetTable<MapAreaInfo>()[id >> 16];
            GroupId = (ushort)id;
            squadrons = new IdTable<AirForceSquadron, IRawAirForceSquadron>(this);
        }

        public override void Update(IRawAirForceGroup raw)
        {
            Name = raw.Name;
            Distance = raw.Distance;
            Action = raw.Action;
            squadrons.BatchUpdate(raw.Squadrons);
        }

        public ITable<T> TryGetTable<T>()
        {
            if (typeof(T) == typeof(AirForceSquadron))
                return (ITable<T>)Squadrons;

            return null;
        }
    }
}
