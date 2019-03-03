using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Game.Models.Combat
{
    public partial class Side : BindableObject
    {
        public Side(IReadOnlyList<Ship> fleet, IReadOnlyList<Ship> fleet2)
        {
            Fleet = fleet?.Select(s => new BattleParticipant(s)).ToArray();
            Fleet2 = fleet2?.Select(s => new BattleParticipant(s)).ToArray();
            Count = (Fleet?.Count ?? 0) + (Fleet2?.Count ?? 0);
        }

        public void Load(in RawSide raw)
        {
            LoadEnvironment(raw);

            if (raw.Fleet != null && Fleet != null)
                for (int i = 0; i < raw.Fleet.Count; i++)
                    Fleet[i].Load(raw.Fleet[i]);
            if (raw.Fleet2 != null && Fleet2 != null)
                for (int i = 0; i < raw.Fleet2.Count; i++)
                    Fleet2[i].Load(raw.Fleet2[i]);
        }

        private void LoadEnvironment(in RawSide raw)
        {
            using (EnterBatchNotifyScope())
            {
                Formation = raw.Formation;
                Detection = raw.Detection;
            }
        }

        public Side(MasterDataRoot masterData, in RawSide raw, bool isEnemy)
        {
            LoadEnvironment(raw);

            if (raw.Fleet != null)
            {
                Count += raw.Fleet.Count;
                var f = new BattleParticipant[raw.Fleet.Count];
                for (int i = 0; i < raw.Fleet.Count; i++)
                    f[i] = new BattleParticipant(new BattlingShip(masterData, raw.Fleet[i]), raw.Fleet[i], isEnemy);
                Fleet = f;
            }
            if (raw.Fleet2 != null)
            {
                Count += raw.Fleet2.Count;
                var f = new BattleParticipant[raw.Fleet2.Count];
                for (int i = 0; i < raw.Fleet2.Count; i++)
                    f[i] = new BattleParticipant(new BattlingShip(masterData, raw.Fleet2[i]), raw.Fleet2[i], isEnemy);
                Fleet2 = f;
            }
        }

        public IReadOnlyList<BattleParticipant> Fleet { get; }
        public IReadOnlyList<BattleParticipant> Fleet2 { get; }
        public int Count { get; }

        public BattleParticipant FindShip(int index)
        {
            if (index < Fleet?.Count)
                return Fleet[index];
            if (index - 6 < Fleet2?.Count)
                return Fleet2[index - 6];
            return null;
        }

        internal void UpdateDamageRate()
        {
            int nowHP = 0, maxHP = 0, sunk = 0;
            if (Fleet != null)
                foreach (var s in Fleet)
                {
                    nowHP += s.ToHP;
                    maxHP += s.FromHP;
                    if (s.ToHP <= 0) sunk++;
                }
            if (Fleet2 != null)
                foreach (var s in Fleet2)
                {
                    nowHP += s.ToHP;
                    maxHP += s.FromHP;
                    if (s.ToHP <= 0) sunk++;
                }
            DamageRate = (maxHP - nowHP) / (double)maxHP;
            SunkCount = sunk;
        }
    }
}
