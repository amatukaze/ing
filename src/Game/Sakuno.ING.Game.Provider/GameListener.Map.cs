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
    public partial class GameProvider
    {
        public event TimedMessageHandler<IReadOnlyCollection<RawMap>> MapsUpdated;
        public event TimedMessageHandler<IReadOnlyCollection<RawAirForceGroup>> AirForceUpdated;
        public event TimedMessageHandler<AirForceSetPlane> AirForcePlaneSet;
        public event TimedMessageHandler<IEnumerable<AirForceSetAction>> AirForceActionSet;
        public event TimedMessageHandler<AirForceSupply> AirForceSupplied;
        public event TimedMessageHandler<RawAirForceGroup> AirForceExpanded;

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
