using System.Collections.Generic;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class Side
    {
        public Side(MasterDataRoot masterData, IReadOnlyList<Ship> fleet, IReadOnlyList<Ship> fleet2, in RawSide raw, bool isEnemy)
        {
            Formation = raw.Formation;
            Detection = raw.Detection;
            ActiveFleetId = raw.ActiveFleetId;
            if (raw.Fleet != null)
            {
                var f = new BattleParticipant[raw.Fleet.Count];
                for (int i = 0; i < raw.Fleet.Count; i++)
                    f[i] = new BattleParticipant(fleet?[i] ?? new BattlingShip(masterData, raw.Fleet[i]), raw.Fleet[i], isEnemy);
                Fleet = f;
            }
            if (raw.Fleet2 != null)
            {
                var f = new BattleParticipant[raw.Fleet2.Count];
                for (int i = 0; i < raw.Fleet2.Count; i++)
                    f[i] = new BattleParticipant(fleet2?[i] ?? new BattlingShip(masterData, raw.Fleet2[i]), raw.Fleet2[i], isEnemy);
                Fleet2 = f;
            }
        }

        public Formation Formation { get; }
        public IReadOnlyList<BattleParticipant> Fleet { get; }
        public IReadOnlyList<BattleParticipant> Fleet2 { get; }
        public Detection? Detection { get; }
        public int? ActiveFleetId { get; }
        public EquipmentInfo NightTouchingPlane { get; internal set; }
        public BattleParticipant FlareShootingShip { get; internal set; }

        public BattleParticipant FindShip(int index)
        {
            if (index < Fleet?.Count)
                return Fleet[index];
            if (index - 6 < Fleet2?.Count)
                return Fleet2[index - 6];
            return null;
        }
    }
}
