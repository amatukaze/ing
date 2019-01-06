using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models
{
    internal sealed class BasicAdmiral : RawAdmiral
    {
        internal int api_member_id;
        public override int Id => api_member_id;

        internal string api_nickname;
        public override string Name => api_nickname;

        internal int api_level;
        internal int api_experience;
        public override Leveling Leveling => new Leveling(api_level,
            api_experience,
            KnownLeveling.GetAdmiralExp(api_level),
            KnownLeveling.GetAdmiralExp(api_level + 1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        internal AdmiralRank api_rank;
        public override AdmiralRank Rank => api_rank;

        internal string api_comment;
        public override string Comment => api_comment;

        internal int api_max_chara;
        public override int MaxShipCount => api_max_chara;

        internal int api_max_slotitem;
        public override int MaxEquipmentCount => api_max_slotitem;

        internal int api_st_win;
        internal int api_st_lose;
        internal int api_ms_count;
        internal int api_ms_success;
        internal int api_pt_win;
        internal int api_pt_lose;

        public override BattleStat BattleStat => new BattleStat(api_st_win, api_st_lose);
        public override BattleStat PracticeStat => new BattleStat(api_pt_win, api_pt_lose);
        public override ExpeditionStat ExpeditionStat => new ExpeditionStat(api_ms_success, api_ms_count);

        internal bool api_large_dock;
        public override bool CanLSC => api_large_dock;
        public override int MaxMaterial => Leveling.Level * 250 + 750;
    }
}
