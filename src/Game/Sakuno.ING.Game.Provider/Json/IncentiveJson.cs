using Newtonsoft.Json;
using Sakuno.ING.Game.Json.Converters;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json
{
    internal class IncentiveJson
    {
        public int api_count;
        public class Item : IRawIncentiveReward
        {
            public int api_type;
            public int api_mst_id;
            [JsonProperty("api_getmes"), JsonConverter(typeof(HtmlNewLineEater))]
            public string Description { get; set; }
            public bool TryGetShip(out ShipInfoId ship)
            {
                if (api_type == 1)
                {
                    ship = (ShipInfoId)api_mst_id;
                    return true;
                }
                else
                {
                    ship = default;
                    return false;
                }
            }
            public bool TryGetEquipment(out EquipmentRecord equipment)
            {
                if (api_type == 2)
                {
                    equipment = new EquipmentRecord
                    {
                        Id = (EquipmentInfoId)api_mst_id,
                        Count = 1
                    };
                    return true;
                }
                else
                {
                    equipment = default;
                    return false;
                }
            }
            public bool TryGetUseItem(out UseItemRecord useItem)
            {
                if (api_type == 3)
                {
                    useItem = new UseItemRecord
                    {
                        ItemId = (UseItemId)api_mst_id,
                        Count = 1
                    };
                    return true;
                }
                else
                {
                    useItem = default;
                    return false;
                }
            }
            public bool TryGetFurniture(out FurnitureId furniture)
            {
                if (api_type == 5)
                {
                    furniture = (FurnitureId)api_mst_id;
                    return true;
                }
                else
                {
                    furniture = default;
                    return false;
                }
            }
        }
        public Item[] api_item;
    }
}
