using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawMap>>> MapsUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawAirForceGroup>>> AirForceUpdated;
        public readonly IProducer<ITimedMessage<AirForceSetPlane>> AirForcePlaneSet;
        public readonly IProducer<ITimedMessage<IEnumerable<AirForceSetAction>>> AirForceActionSet;
        public readonly IProducer<ITimedMessage<AirForceSupply>> AirForceSupplied;
        public readonly IProducer<ITimedMessage<IRawAirForceGroup>> AirForceExpanded;

        private static AirForceSetPlane ParseAirForcePlaneSet(NameValueCollection request, AirForceSetPlaneJson response)
            => new AirForceSetPlane
            {
                MapAreaId = request.GetInt("api_area_id"),
                AirForceId = request.GetInt("api_base_id"),
                NewDistance = response.api_distance,
                UpdatedSquadrons = response.api_plane_info
            };

        private static IEnumerable<AirForceSetAction> ParseAirForceActionSet(NameValueCollection request)
        {
            int mapArea = request.GetInt("api_area_id");
            return request.GetInts("api_base_id").Zip(
                request.GetInts("api_action_kind"),
                (id, action) => new AirForceSetAction
                {
                    MapAreaId = mapArea,
                    AirForceId = id,
                    Action = (AirForceAction)action
                });
        }

        private static AirForceSupply ParseAirForceSupply(NameValueCollection request, AirForceSupplyJson response)
            => new AirForceSupply
            {
                MapAreaId = request.GetInt("api_area_id"),
                AirForceId = request.GetInt("api_base_id"),
                UpdatedSquadrons = response.api_plane_info
            };
    }
}
