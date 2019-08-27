using System.Collections.Generic;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using static Sakuno.ING.Game.Models.Knowledge.KnownShipType;

namespace Sakuno.ING.Game.Models.Quests
{
    [Export(typeof(IQuestKnowledges))]
    public class QuestKnowledges : IQuestKnowledges
    {
        private readonly IStatePersist statePersist;
        private readonly Dictionary<int, KnownQuestTarget> targets;

        private KnownQuestTarget Create(params QuestCounter[] counters)
            => new KnownQuestTarget(statePersist, counters);

        public QuestKnowledges(IStatePersist statePersist)
        {
            this.statePersist = statePersist;

            targets = new Dictionary<int, KnownQuestTarget>
            {
                [210] = Create(new BattleWinCounter((QuestId)210, maximum: 10)), //敵艦隊を10回邀撃せよ！
                [218] = Create(new EnemySunkCounter((QuestId)218, maximum: 3, shipTypes: new[] { Transport })), //敵補給艦を3隻撃沈せよ！
                [211] = Create(new EnemySunkCounter((QuestId)211, maximum: 3, shipTypes: new[] { AircraftCarrier, LightAircraftCarrier })), //敵空母を3隻撃沈せよ！
                [212] = Create(new EnemySunkCounter((QuestId)212, maximum: 5, shipTypes: new[] { Transport })), //敵輸送船団を叩け！
                [226] = Create(new BattleBossCounter((QuestId)226, maximum: 5, m => m.AreaId == 2)), //南西諸島海域の制海権を握れ！
                [230] = Create(new EnemySunkCounter((QuestId)230, maximum: 6, shipTypes: new[] { Submarine })), //敵潜水艦を制圧せよ！

                [220] = Create(new EnemySunkCounter((QuestId)220, maximum: 20, shipTypes: new[] { AircraftCarrier, LightAircraftCarrier })), //い号作戦
                [213] = Create(new EnemySunkCounter((QuestId)213, maximum: 20, shipTypes: new[] { Transport })), //海上通商破壊作戦
                [221] = Create(new EnemySunkCounter((QuestId)221, maximum: 50, shipTypes: new[] { Transport })), //ろ号作戦
                [228] = Create(new EnemySunkCounter((QuestId)228, maximum: 15, shipTypes: new[] { Submarine })), //海上護衛戦
                [241] = Create(new BattleBossCounter((QuestId)241, maximum: 5, m => m.AreaId == 3 && m.CategoryNo >= 3)), //敵北方艦隊主力を撃滅せよ！
                [229] = Create(new BattleBossCounter((QuestId)229, maximum: 12, m => m.AreaId == 4)), //敵東方艦隊を撃滅せよ！
                [261] = Create(new BattleBossCounter((QuestId)261, maximum: 3, (MapId)15)), //海上輸送路の安全確保に努めよ！
                [265] = Create(new BattleBossCounter((QuestId)265, maximum: 10, (MapId)15)), //海上護衛強化月間
            };
        }

        public QuestTarget GetTargetFor(QuestId id)
        {
            targets.TryGetValue(id, out var target);
            return target;
        }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {
            foreach (var target in targets.Values)
                target.OnBattleComplete(routing, battle, result);
            statePersist.SaveChanges();
        }
    }
}
