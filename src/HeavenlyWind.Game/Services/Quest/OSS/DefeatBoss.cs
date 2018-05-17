using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    class DefeatBoss : Battle
    {
        public HashSet<int> MapIDs { get; private set; }

        public BattleRank LowestRank { get; private set; }

        protected DefeatBoss(int[] rpMapIDs, BattleRank rpLowestRank)
        {
            MapIDs = rpMapIDs == null ? null : new HashSet<int>(rpMapIDs);
            LowestRank = rpLowestRank;
        }

        protected override void Process(ProgressInfo rpProgress, BattleInfo rpBattle, RawBattleResult rpResult)
        {
            if (!rpBattle.IsBossBattle)
                return;

            var rSortie = SortieInfo.Current;
            if (MapIDs.Contains(rSortie.Map.ID) && rpResult.Rank >= LowestRank)
                rpProgress.Progress++;
        }

        [Quest(226)]
        class SoundWesternBoss : DefeatBoss
        {
            public SoundWesternBoss() : base(new[] { 21, 22, 23, 24, 25 }, BattleRank.B) { }
        }
        [Quest(229)]
        class EasternBoss : DefeatBoss
        {
            public EasternBoss() : base(new[] { 41, 42, 43, 44, 45 }, BattleRank.B) { }
        }
        [Quest(242)]
        class Eastern2Boss : DefeatBoss
        {
            public Eastern2Boss() : base(new[] { 44 }, BattleRank.B) { }
        }
        [Quest(243)]
        class SouthernBoss : DefeatBoss
        {
            public SouthernBoss() : base(new[] { 52 }, BattleRank.S) { }
        }
        [Quest(241)]
        class NothernBoss : DefeatBoss
        {
            public NothernBoss() : base(new[] { 33, 34, 35 }, BattleRank.B) { }
        }
        [Quest(256)]
        class SW61Boss : DefeatBoss
        {
            public SW61Boss() : base(new[] { 61 }, BattleRank.S) { }
        }

        [Quest(261)]
        class Weekly15Boss : DefeatBoss
        {
            public Weekly15Boss() : base(new[] { 15 }, BattleRank.A) { }
        }
        [Quest(265)]
        class Monthly15Boss : DefeatBoss
        {
            public Monthly15Boss() : base(new[] { 15 }, BattleRank.A) { }
        }
    }
}
