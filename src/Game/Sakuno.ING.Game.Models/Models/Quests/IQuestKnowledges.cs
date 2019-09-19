using System.Collections.Generic;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    public enum SingletonEvent
    {
        ShipRepair,
        ShipSupply,
        ShipConstruct,
        EquipmentCreate,
        EquipmentImprove
    }

    public interface IQuestKnowledges
    {
        void Load();

        QuestTarget GetTargetFor(QuestId id);

        void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result);
        void OnSortieStart(MapId mapId, HomeportFleet fleet1, HomeportFleet fleet2);
        void OnExpeditionComplete(HomeportFleet fleet, ExpeditionInfo expedition, ExpeditionResult result);
        void OnShipPowerup(HomeportShip ship, IReadOnlyCollection<HomeportShip> consumed, bool success);
        void OnShipDismantle(IReadOnlyCollection<HomeportShip> ships);
        void OnEquipmentDismantle(IReadOnlyCollection<HomeportEquipment> equipment);
        void OnSingletonEvent(SingletonEvent @event);
        void OnExerciseComplete(HomeportFleet fleet, BattleResult currentBattleResult);
        void OnMapRouting(MapRouting routing);
    }
}
