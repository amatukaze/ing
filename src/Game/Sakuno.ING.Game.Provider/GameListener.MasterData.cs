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
        private readonly ITimedMessageProvider<MasterDataUpdate> masterDataUpdated;
        public event TimedMessageHandler<MasterDataUpdate> MasterDataUpdated
        {
            add => masterDataUpdated.Received += value;
            remove => masterDataUpdated.Received -= value;
        }

        private static MasterDataUpdate ParseMasterData(MasterDataJson raw)
        {

            foreach (var id in raw.api_mst_equip_exslot)
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
                    l.Add(new ItemRecord { ItemId = KnownUseItem.Blueprint, Count = u.api_drawing_count });
                if (u.api_catapult_count != 0)
                    l.Add(new ItemRecord { ItemId = KnownUseItem.FlightDeckCatapult, Count = u.api_catapult_count });
                if (u.api_report_count != 0)
                    l.Add(new ItemRecord { ItemId = KnownUseItem.ActionReport, Count = u.api_report_count });
                s.UpgradeSpecialConsumption = l;
            }

            return new MasterDataUpdate
            (
                shipInfos: raw.api_mst_ship,
                shipTypes: raw.api_mst_stype,
                equipmentInfos: raw.api_mst_slotitem,
                equipmentTypes: raw.api_mst_slotitem_equiptype,
                furnitureInfos: raw.api_mst_furniture,
                useItems: raw.api_mst_useitem,
                mapAreas: raw.api_mst_maparea,
                maps: raw.api_mst_mapinfo,
                expeditions: raw.api_mst_mission,
                bgms: raw.api_mst_bgm
            );
        }
    }
}
