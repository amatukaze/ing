using System;
using System.Collections.Immutable;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class SortieStartCounter : QuestCounter
    {
        public SortieStartCounter(in QuestCounterParams @params) : base(@params)
        {
        }

        public virtual void OnSortieStart(IStatePersist statePersist, MapId mapId, HomeportFleet fleet1, HomeportFleet fleet2)
            => Increase(statePersist);
    }

    internal class BattleResultCounter : QuestCounter
    {
        public BattleResultCounter(in QuestCounterParams @params) : base(@params)
        {
        }

        public void OnBattleComplete(IStatePersist statePersist, MapRouting routing, Battle battle, BattleResult result)
            => Increase(statePersist, IncreaseCount(routing, battle, result));

        protected virtual int IncreaseCount(MapRouting routing, Battle battle, BattleResult result) => 1;
    }

    internal class BattleWinCounter : BattleResultCounter
    {
        private readonly BattleRank rankRequired;
        private readonly Predicate<MapRouting> routingFilter;

        public BattleWinCounter(in QuestCounterParams @params, BattleRank rankRequired = BattleRank.B, Predicate<MapRouting> routingFilter = null) : base(@params)
        {
            this.rankRequired = rankRequired;
            this.routingFilter = routingFilter;
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
            => result.Rank <= rankRequired && routingFilter?.Invoke(routing) != false
            ? 1 : 0;
    }

    internal class BattleBossCounter : BattleResultCounter
    {
        private readonly Predicate<MapId> mapFilter;
        private readonly BattleRank rankRequired;
        private readonly Predicate<Fleet> fleetFilter;

        public BattleBossCounter(in QuestCounterParams @params,
            Predicate<MapId> mapFilter = null,
            Predicate<Fleet> fleetFilter = null,
            BattleRank rankRequired = BattleRank.B) : base(@params)
        {
            this.mapFilter = mapFilter;
            this.fleetFilter = fleetFilter;
            this.rankRequired = rankRequired;
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
            => routing.EventKind == MapEventKind.Boss
            && result.Rank <= rankRequired
            && mapFilter?.Invoke(routing.Map.Id) != false
            && fleetFilter?.Invoke(battle.Ally.Fleet?.FleetInfo) != false
            ? 1 : 0;
    }

    internal class EnemySunkCounter : BattleResultCounter
    {
        private readonly ImmutableArray<int> shipTypes;

        public EnemySunkCounter(in QuestCounterParams @params, ImmutableArray<int> shipTypes) : base(@params)
        {
            this.shipTypes = shipTypes;
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
        {
            int count = 0;
            if (battle.Enemy.Fleet is { } f1)
                foreach (var e in f1)
                    if (e.IsSunk && e.Ship?.Info?.Type?.Id is { } id && shipTypes.Contains(id))
                        count++;
            if (battle.Enemy.Fleet2 is { } f2)
                foreach (var e in f2)
                    if (e.IsSunk && e.Ship?.Info?.Type?.Id is { } id && shipTypes.Contains(id))
                        count++;
            return count;
        }
    }

    internal class ExerciseCounter : QuestCounter
    {
        private readonly Predicate<HomeportFleet> fleetFilter;
        private readonly BattleRank rankRequired;

        public ExerciseCounter(in QuestCounterParams @params,
            Predicate<HomeportFleet> fleetFilter = null, BattleRank rankRequired = BattleRank.B)
            : base(@params)
        {
            this.fleetFilter = fleetFilter;
            this.rankRequired = rankRequired;
        }

        public void OnExerciseComplete(IStatePersist statePersist, HomeportFleet fleet, BattleResult result)
        {
            if (result.Rank <= rankRequired &&
                fleetFilter?.Invoke(fleet) != false)
                Increase(statePersist);
        }
    }

    internal class MapRoutingCounter : QuestCounter
    {
        private readonly Predicate<MapRouting> routingFilter;
        public MapRoutingCounter(in QuestCounterParams @params, Predicate<MapRouting> routingFilter = null)
            : base(@params)
        {
            this.routingFilter = routingFilter;
        }

        public void OnMapRouting(IStatePersist statePersist, MapRouting routing)
        {
            if (routingFilter?.Invoke(routing) != false)
                Increase(statePersist);
        }
    }
}
