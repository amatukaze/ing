using System.Collections.Generic;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.Knowledge;
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
                [214] = Create(
                    new SortieStartCounter((QuestId)214, counterId: 0, maximum: 36),
                    new BattleBossCounter((QuestId)214, counterId: 1, maximum: 24, rankRequired: BattleRank.E),
                    new BattleBossCounter((QuestId)214, counterId: 2, maximum: 12, rankRequired: BattleRank.B),
                    new BattleWinCounter((QuestId)214, counterId: 3, maximum: 6, rankRequired: BattleRank.S)), //あ号作戦
                [220] = Create(new EnemySunkCounter((QuestId)220, maximum: 20, shipTypes: new[] { AircraftCarrier, LightAircraftCarrier })), //い号作戦
                [213] = Create(new EnemySunkCounter((QuestId)213, maximum: 20, shipTypes: new[] { Transport })), //海上通商破壊作戦
                [221] = Create(new EnemySunkCounter((QuestId)221, maximum: 50, shipTypes: new[] { Transport })), //ろ号作戦
                [228] = Create(new EnemySunkCounter((QuestId)228, maximum: 15, shipTypes: new[] { Submarine })), //海上護衛戦
                [241] = Create(new BattleBossCounter((QuestId)241, maximum: 5, m => m.AreaId == 3 && m.CategoryNo >= 3)), //敵北方艦隊主力を撃滅せよ！
                [229] = Create(new BattleBossCounter((QuestId)229, maximum: 12, m => m.AreaId == 4)), //敵東方艦隊を撃滅せよ！
                [261] = Create(new BattleBossCounter((QuestId)261, maximum: 3, (MapId)15)), //海上輸送路の安全確保に努めよ！
                [265] = Create(new BattleBossCounter((QuestId)265, maximum: 10, (MapId)15)), //海上護衛強化月間

                [303] = Create(new ExerciseCounter((QuestId)303, maximum: 3, rankRequired: BattleRank.E)), //「演習」で練度向上！
                [304] = Create(new ExerciseCounter((QuestId)304, maximum: 5)), //「演習」で他提督を圧倒せよ！
                [302] = Create(new ExerciseCounter((QuestId)302, maximum: 20)), //大規模演習
                [311] = Create(new ExerciseCounter((QuestId)311, maximum: 7, periodOverride: QuestPeriod.Daily)), //精鋭艦隊演習

                [402] = Create(new ExpeditionCounter((QuestId)402, maximum: 3)), //「遠征」を3回成功させよう！
                [403] = Create(new ExpeditionCounter((QuestId)403, maximum: 10)), //「遠征」を10回成功させよう！
                [404] = Create(new ExpeditionCounter((QuestId)404, maximum: 30)), //大規模遠征作戦、発令！
                [410] = Create(new ExpeditionCounter((QuestId)410, maximum: 1, e => e == 37 || e == 38)), //南方への輸送作戦を成功させよ！
                [411] = Create(new ExpeditionCounter((QuestId)411, maximum: 6, e => e == 37 || e == 38)), //南方への鼠輸送を継続実施せよ!
                [424] = Create(new ExpeditionCounter((QuestId)424, maximum: 4, (ExpeditionId)5)), //輸送船団護衛を強化せよ！
                [426] = Create(
                    new ExpeditionCounter((QuestId)426, counterId: 0, maximum: 1, expeditionId: (ExpeditionId)3),
                    new ExpeditionCounter((QuestId)426, counterId: 1, maximum: 1, expeditionId: (ExpeditionId)4),
                    new ExpeditionCounter((QuestId)426, counterId: 2, maximum: 1, expeditionId: (ExpeditionId)5),
                    new ExpeditionCounter((QuestId)426, counterId: 3, maximum: 1, expeditionId: (ExpeditionId)10)), //海上通商航路の警戒を厳とせよ！
                [428] = Create(
                    new ExpeditionCounter((QuestId)428, counterId: 0, maximum: 2, expeditionId: (ExpeditionId)4),
                    new ExpeditionCounter((QuestId)428, counterId: 1, maximum: 2, expeditionId: (ExpeditionId)102),
                    new ExpeditionCounter((QuestId)428, counterId: 2, maximum: 2, expeditionId: (ExpeditionId)103)), //近海に侵入する敵潜を制圧せよ！

                [503] = Create(new SingletonEventCounter((QuestId)503, maximum: 5, SingletonEvent.ShipRepair)), //艦隊大整備！
                [504] = Create(new SingletonEventCounter((QuestId)504, maximum: 15, SingletonEvent.ShipSupply)), //艦隊酒保祭り！

                [605] = Create(new SingletonEventCounter((QuestId)605, maximum: 1, SingletonEvent.EquipmentCreate)), //新装備「開発」指令
                [606] = Create(new SingletonEventCounter((QuestId)606, maximum: 1, SingletonEvent.ShipConstruct)), //新造艦「建造」指令
                [607] = Create(new SingletonEventCounter((QuestId)607, maximum: 3, SingletonEvent.EquipmentCreate)), //装備「開発」集中強化！
                [608] = Create(new SingletonEventCounter((QuestId)608, maximum: 3, SingletonEvent.ShipConstruct)), //艦娘「建造」艦隊強化！
                [609] = Create(new ShipDismantleCounter((QuestId)609, maximum: 2)), //軍縮条約対応！
                [613] = Create(new EquipmentDismantleCounter((QuestId)613, maximum: 24)), //資源の再利用
                [673] = Create(new EquipmentDismantleTypedCounter((QuestId)673, maximum: 4, typeId: KnownEquipmentType.SmallMainGun)), //装備開発力の整備
                [674] = Create(new EquipmentDismantleTypedCounter((QuestId)674, maximum: 3, typeId: KnownEquipmentType.AAGun)), //工廠環境の整備
                [638] = Create(new EquipmentDismantleTypedCounter((QuestId)638, maximum: 6, typeId: KnownEquipmentType.AAGun)), //対空機銃量産
                [643] = Create(new EquipmentDismantleTypedCounter((QuestId)643, maximum: 2, id: (EquipmentInfoId)20)), //主力「陸攻」の調達
                [645] = Create(new EquipmentDismantleTypedCounter((QuestId)645, maximum: 1, id: (EquipmentInfoId)35)), //「洋上補給」物資の調達
                [663] = Create(new EquipmentDismantleTypedCounter((QuestId)663, maximum: 10, typeId: KnownEquipmentType.LargeMainGun)), //新型艤装の継続研究
                [675] = Create(
                    new EquipmentDismantleTypedCounter((QuestId)675, counterId: 0, maximum: 6, typeId: KnownEquipmentType.FighterAircraft),
                    new EquipmentDismantleTypedCounter((QuestId)675, counterId: 1, maximum: 4, typeId: KnownEquipmentType.AAGun)), //運用装備の統合整備
                [676] = Create(
                    new EquipmentDismantleTypedCounter((QuestId)676, counterId: 0, maximum: 3, typeId: KnownEquipmentType.MediumMainGun),
                    new EquipmentDismantleTypedCounter((QuestId)676, counterId: 1, maximum: 3, typeId: KnownEquipmentType.SecondaryGun),
                    new EquipmentDismantleTypedCounter((QuestId)676, counterId: 2, maximum: 1, id: (EquipmentInfoId)75)), //装備開発力の集中整備
                [677] = Create(
                    new EquipmentDismantleTypedCounter((QuestId)677, counterId: 0, maximum: 4, typeId: KnownEquipmentType.LargeMainGun),
                    new EquipmentDismantleTypedCounter((QuestId)677, counterId: 1, maximum: 2, typeId: KnownEquipmentType.ReconnaissanceSeaplane),
                    new EquipmentDismantleTypedCounter((QuestId)677, counterId: 2, maximum: 3, typeId: KnownEquipmentType.Torpedo)), //継戦支援能力の整備
                [678] = Create(
                    new EquipmentDismantleTypedCounter((QuestId)678, counterId: 0, maximum: 3, id: (EquipmentInfoId)19),
                    new EquipmentDismantleTypedCounter((QuestId)678, counterId: 1, maximum: 5, id: (EquipmentInfoId)20)), //主力艦上戦闘機の更新
                [680] = Create(
                    new EquipmentDismantleTypedCounter((QuestId)680, counterId: 0, maximum: 4, typeId: KnownEquipmentType.AAGun),
                    new EquipmentDismantleTypedCounter((QuestId)680, counterId: 1, maximum: 4, equipmentFilter: x => x.Type?.Id.IsRadar() == true)), //対空兵装の整備拡充
                [688] = Create(
                    new EquipmentDismantleTypedCounter((QuestId)688, counterId: 0, maximum: 3, typeId: KnownEquipmentType.FighterAircraft),
                    new EquipmentDismantleTypedCounter((QuestId)688, counterId: 1, maximum: 3, typeId: KnownEquipmentType.DiveBomber),
                    new EquipmentDismantleTypedCounter((QuestId)688, counterId: 2, maximum: 3, typeId: KnownEquipmentType.TorpedoBomber),
                    new EquipmentDismantleTypedCounter((QuestId)688, counterId: 3, maximum: 3, typeId: KnownEquipmentType.ReconnaissanceSeaplane)), //航空戦力の強化

                [702] = Create(new ShipPowerupCounter((QuestId)702, maximum: 2)), //艦の「近代化改修」を実施せよ！
                [703] = Create(new ShipPowerupCounter((QuestId)703, maximum: 15)), //「近代化改修」を進め、戦備を整えよ！
            };
        }

        public void Load()
        {
            foreach (var target in targets.Values)
                foreach (var c in target.Counters)
                    c.Load(statePersist);
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

        public void OnSortieStart(MapId mapId, HomeportFleet fleet1, HomeportFleet fleet2)
        {
            foreach (var target in targets.Values)
                target.OnSortieStart(mapId, fleet1, fleet2);
            statePersist.SaveChanges();
        }

        public void OnExpeditionComplete(HomeportFleet fleet, ExpeditionInfo expedition, ExpeditionResult result)
        {
            foreach (var target in targets.Values)
                target.OnExpeditionComplete(fleet, expedition, result);
            statePersist.SaveChanges();
        }

        public void OnShipPowerup(HomeportShip ship, IReadOnlyCollection<HomeportShip> consumed, bool success)
        {
            foreach (var target in targets.Values)
                target.OnShipPowerup(ship, consumed, success);
            statePersist.SaveChanges();
        }

        public void OnShipDismantle(IReadOnlyCollection<HomeportShip> ships)
        {
            foreach (var target in targets.Values)
                target.OnShipDismantle(ships);
            statePersist.SaveChanges();
        }

        public void OnEquipmentDismantle(IReadOnlyCollection<HomeportEquipment> equipment)
        {
            foreach (var target in targets.Values)
                target.OnEquipmentDismantle(equipment);
            statePersist.SaveChanges();
        }

        public void OnSingletonEvent(SingletonEvent @event)
        {
            foreach (var target in targets.Values)
                target.OnSingletonEvent(@event);
            statePersist.SaveChanges();
        }

        public void OnExerciseComplete(HomeportFleet fleet, BattleResult currentBattleResult)
        {
            foreach (var target in targets.Values)
                target.OnExerciseComplete(fleet, currentBattleResult);
            statePersist.SaveChanges();
        }
    }
}
