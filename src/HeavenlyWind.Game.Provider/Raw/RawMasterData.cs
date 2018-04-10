using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Raw
{
    internal class RawMasterData
    {
        public RawShipInfo[] api_mst_ship;
        public RawShipGraphics[] api_mst_shipgraph;
        public RawEquipmentType[] api_mst_equiptype;
        public int[] api_mst_equip_exslot;
        public RawEquipmentInExtraSlot[] api_mst_equip_exslot_ship;
        public RawShipType[] api_mst_stype;
        public RawEquipmentInfo[] api_mst_slotitem;
        public RawFurnitureInfo[] api_mst_furniture;
        public RawFurnitureGraphics[] api_mst_furnituregraph;
        public RawUseItem[] api_mst_useitem;
        public RawPayItem[] api_mst_payitem;
        public RawItemShop api_mst_item_ship;
        public RawMapAreaInfo[] api_mst_maparea;
        public RawMapInfo[] api_mst_mapinfo;
        public RawMapBgm[] api_mst_mapbgm;
        public RawExpeditionInfo[] api_mst_mission;
        public RawConstants api_mst_const;
        public RawShipRemodelInfo[] api_mst_shipupgrade;
        public RawHomeportBgm[] api_mst_bgm;
    }
    internal class RawShipInfo
    {
        public int api_id;
        public int api_sortno;
        public string api_name;
        public string api_yomi;
        public int api_stype;
        public int api_afterlv;
        public int api_aftershipid;
        public int[] api_taik;
        public int[] api_souk;
        public int[] api_houg;
        public int[] api_raig;
        public int[] api_tyku;
        public int[] api_lock;
        public int api_soku;
        public int api_leng;
        public int api_slot_num;
        public int[] api_maxeq;
        public int api_buildtime;
        public int[] api_broken;
        public int[] api_powerup;
        public int api_backs;
        public string api_getmes;
        public int api_afterfuel;
        public int api_afterbull;
        public int api_fuel_max;
        public int api_bull_max;
        public int api_voicef;
    }
    internal class RawShipGraphics
    {
        public int api_id;
        public int api_sortno;
        public string api_filename;
        public int[] api_version;
        public int[] api_boko_n;
        public int[] api_boko_d;
        public int[] api_kaisyu_n;
        public int[] api_kaisyu_d;
        public int[] api_kaizo_n;
        public int[] api_kaizo_d;
        public int[] api_map_n;
        public int[] api_map_d;
        public int[] api_ensyuf_n;
        public int[] api_ensyuf_d;
        public int[] api_ensyue_n;
        public int[] api_battle_n;
        public int[] api_battle_d;
        public int[] api_weda;
        public int[] api_wedb;
    }
    internal class RawEquipmentType
    {
        public int api_id;
        public int api_name;
        public int api_show_flg;
    }
    internal class RawEquipmentInExtraSlot
    {
        public int api_slotitem_id;
        public int[] api_ship_ids;
    }
    internal class RawShipType
    {
        public int api_id;
        public int api_sortno;
        public string api_name;
        public int api_scnt;
        public int api_kcnt;
        public SortedList<int, int> api_equip_type;
    }
    internal class RawEquipmentInfo
    {
        public int api_id;
        public int api_sortno;
        public string api_name;
        public int[] api_type;
        public int api_taik;
        public int api_souk;
        public int api_houg;
        public int api_raig;
        public int api_soku;
        public int api_baku;
        public int api_tyku;
        public int api_tais;
        public int api_atap;
        public int api_houm;
        public int api_raim;
        public int api_houk;
        public int api_raik;
        public int api_bakk;
        public int api_saku;
        public int api_sakb;
        public int api_luck;
        public int api_leng;
        public int api_cost;
        public int api_distance;
        public int api_rare;
        public int[] api_broken;
        public string api_info;
        public int api_usebull;
        public int api_version;
    }
    internal class RawFurnitureInfo
    {
        public int api_id;
        public int api_type;
        public int api_no;
        public string api_title;
        public string api_description;
        public int api_rarity;
        public int api_price;
        public int api_saleflg;
        public int api_season;
    }
    internal class RawFurnitureGraphics
    {
        public int api_id;
        public int api_type;
        public string api_filename;
        public int api_version;
    }
    internal class RawUseItem
    {
        public int api_id;
        public int api_usetype;
        public string[] api_description;
        public int api_price;
    }
    internal class RawPayItem
    {
        public int api_id;
        public int api_type;
        public string api_name;
        public string[][] api_description;
        public int[] api_item;
        public int api_price;
    }
    internal class RawItemShop
    {
        public int[] api_cabinet_1;
        public int[] api_cabinet_2;
    }
    internal class RawMapAreaInfo
    {
        public int api_id;
        public string api_name;
        public int api_type;
    }
    internal class RawMapInfo
    {
        public int api_id;
        public int api_maparea_id;
        public int api_no;
        public string api_name;
        public int api_level;
        public string api_opetext;
        public string api_infotext;
        public int[] api_item;
        public int? api_maxhp;
        public int api_required_defeat_count;
        public int[] api_sally_flag;
    }
    internal class RawMapBgm
    {
        public int api_id;
        public int api_maparea_id;
        public int api_no;
        public int api_moving_bgm;
        public int[] api_map_bgm;
        public int[] api_boss_bgm;
    }
    internal class RawExpeditionInfo
    {
        public int api_id;
        public string api_disp_no;
        public int api_maparea_id;
        public string name;
        public string api_details;
        public int api_time;
        public int api_deck_num;
        public int api_difficulty;
        public double api_use_fuel;
        public double api_use_bull;
        public int[] api_win_item1;
        public int[] api_win_item2;
        public int api_return_flag;
    }
    internal class RawConstants
    {
        public RawConstant api_parallel_quest_max;
        public RawConstant api_boko_max_ships;
        public RawConstant api_dpflag_quest;
    }
    internal class RawConstant
    {
        public string api_string_value;
        public int api_int_value;
    }
    internal class RawShipRemodelInfo
    {
        public int api_mst_id;
        public int api_current_ship_id;
        public int api_original_ship_id;
        public int api_upgrade_type;
        public int api_upgrade_level;
        public int api_drawing_count;
        public int api_catapult_count;
        public int api_report_count;
        public int api_sortno;
    }
    internal class RawHomeportBgm
    {
        public int api_id;
        public string api_name;
    }
}
