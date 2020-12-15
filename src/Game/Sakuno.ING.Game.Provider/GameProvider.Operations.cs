using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<FleetCompositionChange> FleetCompositionChanged { get; private set; }

        private static FleetCompositionChange ParseFleetCompositionChange(NameValueCollection request)
        {
            var fleetId = (FleetId)request.GetInt("api_id");
            var position = request.GetInt("api_ship_idx");
            var shipId = (ShipId)request.GetInt("api_ship_id");

            if (position == -1 || shipId == -2)
                return new FleetCompositionChange(fleetId, null, null);
            else if (shipId == -1)
                return new FleetCompositionChange(fleetId, position, null);
            else
                return new FleetCompositionChange(fleetId, position, shipId);
        }

        public IObservable<RepairStart> RepairStarted { get; private set; }
        public IObservable<RepairDockId> InstantRepairUsed { get; private set; }

        private static RepairStart ParseRepairStart(NameValueCollection request) => new RepairStart
        (
            instantRepair: request.GetBool("api_highspeed"),
            shipId: (ShipId)request.GetInt("api_ship_id"),
            dockId: (RepairDockId)request.GetInt("api_ndock_id")
        );
        private static RepairDockId ParseInstantRepair(NameValueCollection request) =>
            (RepairDockId)request.GetInt("api_ndock_id");

        public IObservable<ConstructionStart> ConstructionStarted { get; private set; }
        public IObservable<ConstructionDockId> InstantConstructionUsed { get; private set; }

        private static ConstructionStart ParseConstructionStart(NameValueCollection request) => new ConstructionStart
        (
            dockId: (ConstructionDockId)request.GetInt("api_kdock_id"),
            instantBuild: request.GetBool("api_highspeed"),
            isLSC: request.GetBool("api_large_flag"),
            consumption: new Materials()
            {
                Fuel = request.GetInt("api_item1"),
                Bullet = request.GetInt("api_item2"),
                Steel = request.GetInt("api_item3"),
                Bauxite = request.GetInt("api_item4"),
                Development = request.GetInt("api_item5"),
            }
        );
        private static ConstructionDockId ParseInstantConstruction(NameValueCollection request) =>
            (ConstructionDockId)request.GetInt("api_kdock_id");

        public IObservable<AirForceSquadronDeployment> AirForceSquadronDeployed { get; private set; }
        public IObservable<AirForceActionUpdate> AirForceActionUpdated { get; private set; }

        private static AirForceSquadronDeployment ParseAirForceSquadronDeployment(NameValueCollection request, AirForceSquadronDeploymentJson response) =>
            new AirForceSquadronDeployment
            (
                mapAreaId: (MapAreaId)request.GetInt("api_area_id"),
                groupId: (AirForceGroupId)request.GetInt("api_base_id"),
                baseCombatRadius: response.api_distance.api_base,
                bonusCombatRadius: response.api_distance.api_bonus,
                updatedSquadrons: response.api_plane_info
            );
        private static IEnumerable<AirForceActionUpdate> ParseAirForceActionUpdates(NameValueCollection request)
        {
            var mapArea = request.GetInt("api_area_id");

            return request.GetInts("api_base_id").Zip(request.GetInts("api_action_kind"),
                (id, action) => new AirForceActionUpdate
                (
                    mapAreaId: (MapAreaId)mapArea,
                    groupId: (AirForceGroupId)id,
                    action: (AirForceAction)action
                ));
        }
    }
}
