using Sakuno.KanColle.Amatsukaze.Game.Events;
using Sakuno.KanColle.Amatsukaze.Game.Json.MasterData;
using Sakuno.KanColle.Amatsukaze.Messaging;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    partial class GameListener
    {
        public IProducer<MasterDataUpdate> MasterDataUpdated;

        private MasterDataUpdate ParseMasterData(ParsedMessage<MasterDataJson> raw)
        {
            var res = raw.Response;

            foreach (int id in res.api_mst_equip_exslot)
            {
                var type = res.api_mst_slotitem_equiptype.Find(x => x.Id == id);
                if (type != null)
                    type.AvailableInExtraSlot = true;
            }

            foreach (var i in res.api_mst_equip_exslot_ship)
            {
                var e = res.api_mst_slotitem.Find(x => x.Id == i.api_slotitem_id);
                if (e != null)
                    e.ExtraSlotAcceptingShips = i.api_ship_ids;
            }

            foreach (var m in res.api_mst_mapinfo)
                m.BgmInfo = res.api_mst_mapbgm.Find(x => x.Id == m.Id);

            return new MasterDataUpdate
            {
                ShipInfos = res.api_mst_ship,
                ShipTypes = res.api_mst_stype,
                EquipmentInfos = res.api_mst_slotitem,
                EquipmentTypes = res.api_mst_slotitem_equiptype,
                FurnitureInfos = res.api_mst_furniture,
                UseItems = res.api_mst_useitem,
                MapAreas = res.api_mst_maparea,
                Maps = res.api_mst_mapinfo,
                Expeditions = res.api_mst_mission,
                Bgms = res.api_mst_bgm
            };
        }
    }
}
