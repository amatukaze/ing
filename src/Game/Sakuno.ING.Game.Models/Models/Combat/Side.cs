namespace Sakuno.ING.Game.Models.Combat
{
    public partial class Side : BindableObject
    {
        public Side(Fleet fleet, Fleet fleet2)
        {
            if (fleet != null)
                Fleet = new BattleParticipantCollection(fleet, 1);
            if (fleet2 != null)
                Fleet2 = new BattleParticipantCollection(fleet2, 7);
        }

        public void Load(in RawSide raw)
        {
            LoadEnvironment(raw);

            Fleet?.Load(raw.Fleet);
            Fleet2?.Load(raw.Fleet2);
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
                Fleet = new BattleParticipantCollection(raw.Fleet, masterData, 1, isEnemy);
            if (raw.Fleet2 != null)
                Fleet2 = new BattleParticipantCollection(raw.Fleet2, masterData, 7, isEnemy);
        }

        public Side(in RawSide rawLandBase)
        {
            LoadEnvironment(rawLandBase);
            if (rawLandBase.Fleet != null)
                Fleet = new BattleParticipantCollection(rawLandBase.Fleet);
        }

        public BattleParticipantCollection Fleet { get; }
        public BattleParticipantCollection Fleet2 { get; }
        public int Count => (Fleet?.Count ?? 0) + (Fleet2?.Count ?? 0);

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
            using (EnterBatchNotifyScope())
            {
                int nowHP = 0, maxHP = 0, sunk = 0;
                if (Fleet != null)
                    foreach (var s in Fleet)
                    {
                        nowHP += s.ToHP.Current;
                        maxHP += s.FromHP.Current;
                        if (s.IsSunk) sunk++;
                    }
                if (Fleet2 != null)
                    foreach (var s in Fleet2)
                    {
                        nowHP += s.ToHP.Current;
                        maxHP += s.FromHP.Current;
                        if (s.IsSunk) sunk++;
                    }
                DamageRate = (maxHP - nowHP) / (double)maxHP;
                SunkCount = sunk;
            }
        }
    }
}
