using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Sakuno.ING.Game.Events.Shipyard;
using Sakuno.ING.Game.Json.Shipyard;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public event TimedMessageHandler<ShipCreation> ShipCreated;
        public event TimedMessageHandler<BuildingDockId> InstantBuilt;
        public event TimedMessageHandler<ShipBuildCompletion> ShipBuildCompleted;
        public event TimedMessageHandler<EquipmentCreation> EquipmentCreated;
        public event TimedMessageHandler<ShipDismantling> ShipDismantled;
        public event TimedMessageHandler<IReadOnlyCollection<EquipmentId>> EquipmentDismantled;
        public event TimedMessageHandler<EquipmentImprove> EquipmentImproved;
        public event TimedMessageHandler<ShipPowerup> ShipPoweruped;

        private static ShipCreation ParseShipCreation(NameValueCollection request)
            => new ShipCreation
            (
                buildingDockId: (BuildingDockId)request.GetInt("api_kdock_id"),
                instantBuild: request.GetBool("api_highspeed"),
                isLSC: request.GetBool("api_large_flag"),
                consumption: new Materials
                {
                    Fuel = request.GetInt("api_item1"),
                    Bullet = request.GetInt("api_item2"),
                    Steel = request.GetInt("api_item3"),
                    Bauxite = request.GetInt("api_item4"),
                    Development = request.GetInt("api_item5"),
                }
            );

        private static BuildingDockId ParseInstantBuilt(NameValueCollection request)
            => (BuildingDockId)request.GetInt("api_kdock_id");

        private static ShipBuildCompletion ParseShipBuildCompletion(ShipBuildCompletionJson response)
            => new ShipBuildCompletion
            (
                ship: response.api_ship,
                equipments: response.api_slotitem ?? Array.Empty<RawEquipment>()
            );

        private static EquipmentCreation ParseEquipmentCreation(NameValueCollection request, EquipmentCreationJson response)
        {
            if (response.api_slot_item != null)
                return new EquipmentCreation
                (
                    isSuccess: response.api_create_flag,
                    equipment: response.api_slot_item,
                    consumption: new Materials
                    {
                        Fuel = request.GetInt("api_item1"),
                        Bullet = request.GetInt("api_item2"),
                        Steel = request.GetInt("api_item3"),
                        Bauxite = request.GetInt("api_item4"),
                    },
                    selectedEquipentInfoId: response.api_slot_item.EquipmentInfoId
                );
            else
            {
                var index = response.api_fdata.IndexOf(',');
                int.TryParse(response.api_fdata.Substring(index + 1), out int id);
                return new EquipmentCreation
                (
                    isSuccess: response.api_create_flag,
                    equipment: response.api_slot_item,
                    consumption: new Materials
                    {
                        Fuel = request.GetInt("api_item1"),
                        Bullet = request.GetInt("api_item2"),
                        Steel = request.GetInt("api_item3"),
                        Bauxite = request.GetInt("api_item4"),
                    },
                    selectedEquipentInfoId: (EquipmentInfoId)id
                );
            }
        }

        private static ShipDismantling ParseShipDismantling(NameValueCollection request)
            => new ShipDismantling
            (
                shipIds: request.GetShipIds("api_ship_id"),
                dismantleEquipments: request.GetBool("api_slot_dest_flag")
            );

        private static IReadOnlyCollection<EquipmentId> ParseEquipmentDimantling(NameValueCollection request)
            => request.GetEquipmentIds("api_slotitem_ids");

        private static EquipmentImprove ParseEquipmentImprove(NameValueCollection request, EquipmentImproveJson response)
            => new EquipmentImprove
            (
                equipmentId: (EquipmentId)request.GetInt("api_slot_id"),
                recipeId: request.GetInt("api_slot_id"),
                guaranteedSuccess: request.GetBool("api_certain_flag"),
                isSuccess: response.api_remodel_flag,
                updatedTo: response.api_after_slot,
                consumedEquipmentIds: response.api_use_slot_id
            );

        private static ShipPowerup ParseShipPowerup(NameValueCollection request, ShipPowerupJson response)
            => new ShipPowerup
            (
                shipId: (ShipId)request.GetInt("api_id"),
                consumedShipIds: request.GetShipIds("api_id_items"),
                isSuccess: response.api_powerup_flag,
                updatedTo: response.api_ship
            );
    }
}
