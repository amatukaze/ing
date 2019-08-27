using System;
using System.Linq;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class BattleResultCounter : QuestCounter
    {
        public BattleResultCounter(QuestId questId, int maximum, int counterId = 0) : base(questId, maximum, counterId)
        {
        }

        public void OnBattleComplete(IStatePersist statePersist, MapRouting routing, Battle battle, BattleResult result)
            => Increase(statePersist, IncreaseCount(routing, battle, result));

        protected virtual int IncreaseCount(MapRouting routing, Battle battle, BattleResult result) => 1;
    }

    internal class BattleWinCounter : BattleResultCounter
    {
        public BattleWinCounter(QuestId questId, int maximum, int counterId = 0) : base(questId, maximum, counterId)
        {
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
            => result.Rank <= BattleRank.B ? 1 : 0;
    }

    internal class BattleBossCounter : BattleResultCounter
    {
        private readonly Predicate<MapId> mapFilter;
        private readonly BattleRank rankRequired;

        public BattleBossCounter(QuestId questId, int maximum, MapId mapId,
            BattleRank rankRequired = BattleRank.B, int counterId = 0) : base(questId, maximum, counterId)
        {
            mapFilter = m => m == mapId;
            this.rankRequired = rankRequired;
        }

        public BattleBossCounter(QuestId questId, int maximum, Predicate<MapId> mapFilter = null,
            BattleRank rankRequired = BattleRank.B, int counterId = 0) : base(questId, maximum, counterId)
        {
            this.mapFilter = mapFilter;
            this.rankRequired = rankRequired;
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
            => routing.EventKind == MapEventKind.Boss
            && result.Rank <= rankRequired
            && mapFilter?.Invoke(routing.Map.Id) != false
            ? 1 : 0;
    }

    internal class EnemySunkCounter : BattleResultCounter
    {
        private readonly KnownShipType[] shipTypes;

        public EnemySunkCounter(QuestId questId, int maximum, KnownShipType[] shipTypes, int counterId = 0) : base(questId, maximum, counterId)
        {
            this.shipTypes = shipTypes;
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
        {
            int count = 0;
            if (battle.Enemy.Fleet is { } f1)
                foreach (var e in f1)
                    if (e.IsSunk && e.Ship?.Info?.Type?.Id is { } id && shipTypes.Contains((KnownShipType)id))
                        count++;
            if (battle.Enemy.Fleet2 is { } f2)
                foreach (var e in f2)
                    if (e.IsSunk && e.Ship?.Info?.Type?.Id is { } id && shipTypes.Contains((KnownShipType)id))
                        count++;
            return count;
        }
    }
}
