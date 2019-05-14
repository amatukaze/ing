using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public class BattleParticipantCollection : ReadOnlyCollection<BattleParticipant>
    {
        public Fleet FleetInfo { get; }

        public BattleParticipantCollection(Fleet fleet, int startIndex)
            : base(Build(fleet.Ships, startIndex))
        {
            FleetInfo = fleet;
        }

        public BattleParticipantCollection(IReadOnlyList<RawShipInBattle> rawShips, MasterDataRoot masterData, int startIndex, bool isEnemy)
            : base(BuildFromRaw(rawShips, masterData, startIndex, isEnemy, out var ships))
        {
            FleetInfo = new ImplicitFleet(ships);
        }

        public BattleParticipantCollection(IReadOnlyList<RawShipInBattle> rawAirBases)
            : base(rawAirBases.Select((x, i) => new BattleParticipant(i + 1, null, x, false)).ToArray())
        { }

        private static BattleParticipant[] Build(IReadOnlyList<Ship> ships, int startIndex)
            => ships.Select((s, i) => new BattleParticipant(startIndex + i, s)).ToArray();
        private static BattleParticipant[] BuildFromRaw(IReadOnlyList<RawShipInBattle> raw, MasterDataRoot masterData, int startIndex, bool isEnemy, out Ship[] ships)
        {
            ships = new Ship[raw.Count];
            var result = new BattleParticipant[raw.Count];
            for (int i = 0; i < raw.Count; i++)
            {
                ships[i] = new BattlingShip(masterData, raw[i]);
                result[i] = new BattleParticipant(startIndex + i, ships[i], raw[i], isEnemy);
            }
            return result;
        }

        internal void Load(IReadOnlyList<RawShipInBattle> raw)
        {
            if (raw is null) return;
            for (int i = 0; i < raw.Count; i++)
                this[i].Load(raw[i]);
        }

        internal void CompleteAppendBattlePart()
        {
            if (Count == 0) return;
            var mvp = this[0];

            foreach (var s in this)
            {
                s.IsMvp = false;
                if (s.DamageGiven >= mvp.DamageGiven)
                    mvp = s;
            }

            if (this[0].DamageGiven >= mvp.DamageGiven)
                mvp = this[0];
            mvp.IsMvp = true;

            foreach (var s in this)
                s.Notify();
        }
    }
}
