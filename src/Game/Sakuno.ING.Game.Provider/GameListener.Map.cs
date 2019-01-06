using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameProvider
    {
        #region Events
        private readonly ITimedMessageProvider<IReadOnlyCollection<RawMap>> mapsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawMap>> MapsUpdated
        {
            add => mapsUpdated.Received += value;
            remove => mapsUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<IReadOnlyCollection<RawAirForceGroup>> airForceUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawAirForceGroup>> AirForceUpdated
        {
            add => airForceUpdated.Received += value;
            remove => airForceUpdated.Received -= value;
        }

        private readonly ITimedMessageProvider<AirForceSetPlane> airForcePlaneSet;
        public event TimedMessageHandler<AirForceSetPlane> AirForcePlaneSet
        {
            add => airForcePlaneSet.Received += value;
            remove => airForcePlaneSet.Received -= value;
        }

        private readonly ITimedMessageProvider<IEnumerable<AirForceSetAction>> airForceActionSet;
        public event TimedMessageHandler<IEnumerable<AirForceSetAction>> AirForceActionSet
        {
            add => airForceActionSet.Received += value;
            remove => airForceActionSet.Received -= value;
        }

        private readonly ITimedMessageProvider<AirForceSupply> airForceSupplied;
        public event TimedMessageHandler<AirForceSupply> AirForceSupplied
        {
            add => airForceSupplied.Received += value;
            remove => airForceSupplied.Received -= value;
        }

        private readonly ITimedMessageProvider<RawAirForceGroup> airForceExpanded;
        public event TimedMessageHandler<RawAirForceGroup> AirForceExpanded
        {
            add => airForceExpanded.Received += value;
            remove => airForceExpanded.Received -= value;
        }
        #endregion

        private static AirForceSetPlane ParseAirForcePlaneSet(NameValueCollection request, AirForceSetPlaneJson response)
            => new AirForceSetPlane
            (
                mapAreaId: (MapAreaId)request.GetInt("api_area_id"),
                groupId: (AirForceGroupId)request.GetInt("api_base_id"),
                newDistanceBase: response.api_distance.api_base,
                newDistanceBonus: response.api_distance.api_bonus,
                updatedSquadrons: response.api_plane_info
            );

        private static IEnumerable<AirForceSetAction> ParseAirForceActionSet(NameValueCollection request)
        {
            int mapArea = request.GetInt("api_area_id");
            return request.GetInts("api_base_id").Zip(
                request.GetInts("api_action_kind"),
                (id, action) => new AirForceSetAction
                (
                    mapAreaId: (MapAreaId)mapArea,
                    groupId: (AirForceGroupId)id,
                    action: (AirForceAction)action
                ));
        }

        private static AirForceSupply ParseAirForceSupply(NameValueCollection request, AirForceSupplyJson response)
            => new AirForceSupply
            (
                mapAreaId: (MapAreaId)request.GetInt("api_area_id"),
                groupId: (AirForceGroupId)request.GetInt("api_base_id"),
                updatedSquadrons: response.api_plane_info
            );
    }
}
