using System.Collections.Generic;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Json.MasterData;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public ITimedMessageProvider<MasterDataUpdate> MasterDataUpdated;

        private static MasterDataUpdate ParseMasterData(MasterDataJson raw)
        {

            foreach (int id in raw.api_mst_equip_exslot)
            {
                var type = raw.api_mst_slotitem_equiptype.Find(x => x.Id == id);
                if (type != null)
                    type.AvailableInExtraSlot = true;
            }

            foreach (var i in raw.api_mst_equip_exslot_ship)
            {
                var e = raw.api_mst_slotitem.Find(x => x.Id == i.api_slotitem_id);
                if (e != null)
                    e.ExtraSlotAcceptingShips = i.api_ship_ids;
            }

            foreach (var m in raw.api_mst_mapinfo)
                m.BgmInfo = raw.api_mst_mapbgm.Find(x => x.Id == m.Id);

            foreach (var s in raw.api_mst_ship)
            {
                var u = raw.api_mst_shipupgrade.Find(x => x.api_current_ship_id == s.Id);
                if (u == null) continue;

                var l = new List<ItemRecord>(3);
                if (u.api_drawing_count != 0)
                    l.Add(new ItemRecord { ItemId = (int)KnownUseItem.Blueprint, Count = u.api_drawing_count });
                if (u.api_catapult_count != 0)
                    l.Add(new ItemRecord { ItemId = (int)KnownUseItem.FlightDeckCatapult, Count = u.api_catapult_count });
                if (u.api_report_count != 0)
                    l.Add(new ItemRecord { ItemId = (int)KnownUseItem.ActionReport, Count = u.api_report_count });
                s.UpgradeSpecialConsumption = l;
            }

            return new MasterDataUpdate
            {
                ShipInfos = raw.api_mst_ship,
                ShipTypes = raw.api_mst_stype,
                EquipmentInfos = raw.api_mst_slotitem,
                EquipmentTypes = raw.api_mst_slotitem_equiptype,
                FurnitureInfos = raw.api_mst_furniture,
                UseItems = raw.api_mst_useitem,
                MapAreas = raw.api_mst_maparea,
                Maps = raw.api_mst_mapinfo,
                Expeditions = raw.api_mst_mission,
                Bgms = raw.api_mst_bgm
            };
        }
    }
}
