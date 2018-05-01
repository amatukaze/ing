using System.Collections.Specialized;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<ShipCreation>> ShipCreated;
        public readonly IProducer<ITimedMessage<BuildingDockId>> InstantBuilt;
        public readonly IProducer<ITimedMessage<IShipBuildCompletion>> ShipBuildCompleted;
        public readonly IProducer<ITimedMessage<EquipmentCreation>> EquipmentCreated;
        public readonly IProducer<ITimedMessage<ShipDismantling>> ShipDismantled;
        public readonly IProducer<ITimedMessage<EquipmentDismantling>> EquipmentDismantled;

        private ShipCreation ParseShipCreation(NameValueCollection request)
            => new ShipCreation
            {
                BuildingDockId = request.GetInt("api_kdock_id"),
                InstantBuild = request.GetBool("api_highspeed"),
                IsLSC = request.GetBool("api_large_flag"),
                Consumption = new Materials
                {
                    Fuel = request.GetInt("api_item1"),
                    Bullet = request.GetInt("api_item2"),
                    Steel = request.GetInt("api_item3"),
                    Bauxite = request.GetInt("api_item4"),
                    Development = request.GetInt("api_item5"),
                }
            };

        private BuildingDockId ParseInstantBuilt(NameValueCollection request)
            => new BuildingDockId(request.GetInt("api_kdock_id"));

        private EquipmentCreation ParseEquipmentCreation(NameValueCollection request, EquipmentCreationJson response)
        {
            var result = new EquipmentCreation
            {
                IsSuccess = response.api_create_flag,
                Equipment = response.api_slot_item,
                Consumption = new Materials
                {
                    Fuel = request.GetInt("api_item1"),
                    Bullet = request.GetInt("api_item2"),
                    Steel = request.GetInt("api_item3"),
                    Bauxite = request.GetInt("api_item4"),
                }
            };
            if (response.api_slot_item != null)
                result.SelectedEquipentInfoId = response.api_slot_item.EquipmentInfoId;
            else
            {
                var index = response.api_fdata.IndexOf(',');
                int.TryParse(response.api_fdata.Substring(index + 1), out int id);
                result.SelectedEquipentInfoId = id;
            }
            return result;
        }

        private ShipDismantling ParseShipDismantling(NameValueCollection request)
            => new ShipDismantling
            {
                ShipIds = request.GetInts("api_ship_id"),
                DismantleEquipments = request.GetBool("api_slot_dest_flag")
            };

        private EquipmentDismantling ParseEquipmentDimantling(NameValueCollection request)
            => new EquipmentDismantling
            {
                EquipmentIds = request.GetInts("api_slotitem_ids")
            };
    }
}
