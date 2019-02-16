using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Json
{
    internal sealed class RecordAdmiral : RawAdmiral
    {
        internal int api_member_id;
        public override int Id => api_member_id;

        internal string api_nickname;
        public override string Name => api_nickname;

        internal string api_cmt;
        public override string Comment => api_cmt;

        public int api_level;
        public int[] api_experience;
        public override Leveling Leveling => new Leveling(api_level,
            api_experience.At(0),
            KnownLeveling.GetAdmiralExp(api_level),
            api_experience.At(1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        internal AdmiralRank api_rank;
        public override AdmiralRank Rank => api_rank;

        internal int[] api_ship;
        public override int MaxShipCount => api_ship.At(1);

        internal int[] api_slotitem;
        public override int MaxEquipmentCount => api_slotitem.At(1);

        public class RecordBattleStat
        {
            public int api_win;
            public int api_lose;
            public double api_rate;
            public BattleStat ToValue()
                => new BattleStat(api_win, api_lose, api_rate);
        }
        internal RecordBattleStat api_war;
        public override BattleStat BattleStat => api_war.ToValue();

        internal RecordBattleStat api_practice;
        public override BattleStat PracticeStat => api_practice.ToValue();


        public class RecordExpeditionStat
        {
            public int api_count;
            public int api_success;
            public double api_rate;
            public ExpeditionStat ToValue()
                => new ExpeditionStat(api_success, api_count, api_rate);
        }
        internal RecordExpeditionStat api_mission;
        public override ExpeditionStat ExpeditionStat => api_mission.ToValue();

        internal bool api_large_dock;
        public override bool CanLSC => api_large_dock;

        internal int api_material_max;
        public override int MaxMaterial => api_material_max;
    }
}
