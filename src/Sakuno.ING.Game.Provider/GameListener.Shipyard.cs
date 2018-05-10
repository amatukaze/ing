using System.Collections.Specialized;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly ITimedMessageProvider<ShipCreation> ShipCreated;
        public readonly ITimedMessageProvider<BuildingDockId> InstantBuilt;
        public readonly ITimedMessageProvider<IShipBuildCompletion> ShipBuildCompleted;
        public readonly ITimedMessageProvider<EquipmentCreation> EquipmentCreated;
        public readonly ITimedMessageProvider<ShipDismantling> ShipDismantled;
        public readonly ITimedMessageProvider<EquipmentDismantling> EquipmentDismantled;
        public readonly ITimedMessageProvider<EquipmentImprove> EquipmentImproved;
        public readonly ITimedMessageProvider<ShipPowerup> ShipPoweruped;

        private static ShipCreation ParseShipCreation(NameValueCollection request)
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

        private static BuildingDockId ParseInstantBuilt(NameValueCollection request)
            => new BuildingDockId(request.GetInt("api_kdock_id"));

        private static EquipmentCreation ParseEquipmentCreation(NameValueCollection request, EquipmentCreationJson response)
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

        private static ShipDismantling ParseShipDismantling(NameValueCollection request)
            => new ShipDismantling
            {
                ShipIds = request.GetInts("api_ship_id"),
                DismantleEquipments = request.GetBool("api_slot_dest_flag")
            };

        private static EquipmentDismantling ParseEquipmentDimantling(NameValueCollection request)
            => new EquipmentDismantling
            {
                EquipmentIds = request.GetInts("api_slotitem_ids")
            };

        private static EquipmentImprove ParseEquipmentImprove(NameValueCollection request, EquipmentImproveJson response)
            => new EquipmentImprove
            {
                EquipmentId = request.GetInt("api_slot_id"),
                RecipeId = request.GetInt("api_slot_id"),
                GuaranteedSuccess = request.GetBool("api_certain_flag"),
                IsSuccess = response.api_remodel_flag,
                UpdatedTo = response.api_after_slot,
                ConsumedEquipmentId = response.api_use_slot_id
            };

        private static ShipPowerup ParseShipPowerup(NameValueCollection request, ShipPowerupJson response)
            => new ShipPowerup
            {
                ShipId = request.GetInt("api_id"),
                ConsumedShipIds = request.GetInts("api_id_items"),
                IsSuccess = response.api_powerup_flag,
                ShipAfter = response.api_ship
            };
    }
}
