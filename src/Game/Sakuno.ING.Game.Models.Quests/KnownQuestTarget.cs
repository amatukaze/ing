using System.Collections.Generic;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class KnownQuestTarget : QuestTarget
    {
        public KnownQuestTarget(IStatePersist statePersist, IReadOnlyList<QuestCounter> counters)
            : base(statePersist)
        {
            Counters = counters;
        }

        public override IReadOnlyList<QuestCounter> Counters { get; }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {
            foreach (var c in Counters)
                if (c is BattleResultCounter bc)
                    bc.OnBattleComplete(StatePersist, routing, battle, result);
            UpdateProgress();
        }

        public void OnSortieStart(MapId mapId, HomeportFleet fleet1, HomeportFleet fleet2)
        {
            foreach (var c in Counters)
                if (c is SortieStartCounter sc)
                    sc.OnSortieStart(StatePersist, mapId, fleet1, fleet2);
            UpdateProgress();
        }

        public void OnExpeditionComplete(HomeportFleet fleet, ExpeditionInfo expedition, ExpeditionResult result)
        {
            foreach (var c in Counters)
                if (c is ExpeditionCounter ec)
                    ec.OnExpeditionComplete(StatePersist, fleet, expedition, result);
            UpdateProgress();
        }

        public void OnShipPowerup(HomeportShip ship, IReadOnlyCollection<HomeportShip> consumed, bool success)
        {
            foreach (var c in Counters)
                if (c is ShipPowerupCounter pc)
                    pc.OnShipPowerup(StatePersist, ship, consumed, success);
            UpdateProgress();
        }

        public void OnShipDismantle(IReadOnlyCollection<HomeportShip> ships)
        {
            foreach (var c in Counters)
                if (c is ShipDismantleCounter dc)
                    dc.OnShipDismantle(StatePersist, ships);
            UpdateProgress();
        }

        public void OnEquipmentDismantle(IReadOnlyCollection<HomeportEquipment> equipment)
        {
            foreach (var c in Counters)
                if (c is EquipmentDismantleCounter dc)
                    dc.OnEquipmentDismantle(StatePersist, equipment);
            UpdateProgress();
        }

        public void OnSingletonEvent(SingletonEvent @event)
        {
            foreach (var c in Counters)
                if (c is SingletonEventCounter sc)
                    sc.OnSingletonEvent(StatePersist, @event);
            UpdateProgress();
        }

        public void OnExerciseComplete(HomeportFleet fleet, BattleResult currentBattleResult)
        {
            foreach (var c in Counters)
                if (c is ExerciseCounter ec)
                    ec.OnExerciseComplete(StatePersist, fleet, currentBattleResult);
            UpdateProgress();
        }

        public void OnMapRouting(MapRouting routing, HomeportFleet fleet, HomeportFleet fleet2)
        {
            foreach (var c in Counters)
                if (c is MapRoutingCounter rc)
                    rc.OnMapRouting(StatePersist, routing, fleet, fleet2);
            UpdateProgress();
        }
    }
}
