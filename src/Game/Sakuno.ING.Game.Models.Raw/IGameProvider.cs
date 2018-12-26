using System;
using System.Collections.Generic;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.Battle;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Battle;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public interface IGameProvider
    {
        event Action<SerializationError> SerialzationError;
        event TimedMessageHandler<MasterDataUpdate> MasterDataUpdated;

        event TimedMessageHandler<IReadOnlyCollection<IRawEquipment>> AllEquipmentUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawBuildingDock>> BuildingDockUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawUseItemCount>> UseItemUpdated;
        event TimedMessageHandler<IReadOnlyDictionary<string, int[]>> FreeEquipmentUpdated;

        event TimedMessageHandler<HomeportUpdate> HomeportReturned;
        event TimedMessageHandler<IRawAdmiral> AdmiralUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawFleet>> FleetsUpdated;
        event TimedMessageHandler<CompositionChange> CompositionChanged;
        event TimedMessageHandler<IRawFleet> FleetPresetSelected;
        event TimedMessageHandler<ShipId> ShipExtraSlotOpened;
        event TimedMessageHandler<ShipEquipmentUpdate> ShipEquipmentUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawShip>> PartialShipsUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawFleet>> PartialFleetsUpdated;
        event TimedMessageHandler<IMaterialsUpdate> MaterialsUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawRepairingDock>> RepairingDockUpdated;
        event TimedMessageHandler<RepairStart> RepairStarted;
        event TimedMessageHandler<RepairingDockId> InstantRepaired;
        event TimedMessageHandler<IReadOnlyCollection<IShipSupply>> ShipSupplied;
        event TimedMessageHandler<ExpeditionCompletion> ExpeditionCompleted;
        event TimedMessageHandler<IReadOnlyCollection<IRawIncentiveReward>> IncentiveRewarded;

        event TimedMessageHandler<ShipCreation> ShipCreated;
        event TimedMessageHandler<BuildingDockId> InstantBuilt;
        event TimedMessageHandler<ShipBuildCompletion> ShipBuildCompleted;
        event TimedMessageHandler<EquipmentCreation> EquipmentCreated;
        event TimedMessageHandler<ShipDismantling> ShipDismantled;
        event TimedMessageHandler<IReadOnlyCollection<EquipmentId>> EquipmentDismantled;
        event TimedMessageHandler<EquipmentImprove> EquipmentImproved;
        event TimedMessageHandler<ShipPowerup> ShipPoweruped;

        event TimedMessageHandler<QuestPageUpdate> QuestUpdated;
        event TimedMessageHandler<QuestId> QuestCompleted;

        event TimedMessageHandler<IReadOnlyCollection<IRawMap>> MapsUpdated;
        event TimedMessageHandler<IReadOnlyCollection<IRawAirForceGroup>> AirForceUpdated;
        event TimedMessageHandler<AirForceSetPlane> AirForcePlaneSet;
        event TimedMessageHandler<IEnumerable<AirForceSetAction>> AirForceActionSet;
        event TimedMessageHandler<AirForceSupply> AirForceSupplied;
        event TimedMessageHandler<IRawAirForceGroup> AirForceExpanded;

        event TimedMessageHandler<EnemyDebuffConfirm> EnemyDebuffConfirmed;
        event TimedMessageHandler<SortieStart> SortieStarting;
        event TimedMessageHandler<IRawMapRouting> MapRouting;
        event TimedMessageHandler<IRawBattleResult> BattleCompleted;
    }
}
