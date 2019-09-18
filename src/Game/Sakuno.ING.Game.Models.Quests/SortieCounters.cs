using System;
using System.Linq;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class SortieStartCounter : QuestCounter
    {
        public SortieStartCounter(QuestId questId, int maximum, int counterId = 0) : base(questId, maximum, counterId)
        {
        }

        public virtual void OnSortieStart(IStatePersist statePersist, MapId mapId, HomeportFleet fleet1, HomeportFleet fleet2)
            => Increase(statePersist);
    }

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
        private readonly BattleRank rankRequired;

        public BattleWinCounter(QuestId questId, int maximum, int counterId = 0, BattleRank rankRequired = BattleRank.B) : base(questId, maximum, counterId)
        {
            this.rankRequired = rankRequired;
        }

        protected override int IncreaseCount(MapRouting routing, Battle battle, BattleResult result)
            => result.Rank <= rankRequired ? 1 : 0;
    }

    internal class BattleBossCounter : BattleResultCounter
    {
        private readonly Predicate<MapId> mapFilter;
        private readonly BattleRank rankRequired;
        private readonly Predicate<Fleet> fleetFilter;

        public BattleBossCounter(QuestId questId, int maximum, MapId mapId, Predicate<Fleet> fleetFilter = null,
            BattleRank rankRequired = BattleRank.B, int counterId = 0) : base(questId, maximum, counterId)
        {
            mapFilter = m => m == mapId;
            this.fleetFilter = fleetFilter;
            this.rankRequired = rankRequired;
        }

        public BattleBossCounter(QuestId questId, int maximum, Predicate<MapId> mapFilter = null, Predicate<Fleet> fleetFilter = null,
            BattleRank rankRequired = BattleRank.B, int counterId = 0) : base(questId, maximum, counterId)
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

    internal class ExerciseCounter : QuestCounter
    {
        private readonly Predicate<HomeportFleet> fleetFilter;
        private readonly BattleRank rankRequired;

        public ExerciseCounter(QuestId questId, int maximum, int counterId = 0,
            Predicate<HomeportFleet> fleetFilter = null, BattleRank rankRequired = BattleRank.B, QuestPeriod? periodOverride = null)
            : base(questId, maximum, counterId, periodOverride)
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

    internal static class FleetExtension
    {
        public static bool ContainsShips(this Fleet fleet, int requiredCount = 0, bool requireFlagship = false, bool allowUpgrade = true, params ShipInfoId[] ids)
        {
            bool Satisfy(Ship ship)
            {
                var info = ship?.Info;
                if (info is null) return false;
                foreach (var id in ids)
                    if (allowUpgrade ?
                        info.CanUpgradeFrom(id) :
                        info.Id == id)
                        return true;
                return false;
            }

            if (fleet is null) return false;
            if (requireFlagship && !Satisfy(fleet.Ships.FirstOrDefault()))
                return false;
            return fleet.Ships.Count(Satisfy) >= requiredCount;
        }

        public static bool ContainsShipType(this Fleet fleet, int requiredCount = 0, bool requireFlagship = false, params KnownShipType?[] ids)
        {
            bool Satisfy(Ship ship)
            {
                var type = ship?.Info?.Type;
                foreach (var id in ids)
                    if (type?.Id == id)
                        return true;
                return false;
            }

            if (fleet is null) return false;
            if (requireFlagship && !Satisfy(fleet.Ships.FirstOrDefault()))
                return false;
            return fleet.Ships.Count(Satisfy) >= requiredCount;
        }
    }
}
