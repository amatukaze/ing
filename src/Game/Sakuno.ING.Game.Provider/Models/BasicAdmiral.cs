using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models
{
#nullable disable
    internal sealed class BasicAdmiral : RawAdmiral
    {
        public int api_member_id { get; set; }
        public override int Id => api_member_id;

        public string api_nickname { get; set; }
        public override string Name => api_nickname;

        public int api_level { get; set; }
        public int api_experience { get; set; }
        public override Leveling Leveling => new Leveling(api_level,
            api_experience,
            KnownLeveling.GetAdmiralExp(api_level),
            KnownLeveling.GetAdmiralExp(api_level + 1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        public AdmiralRank api_rank { get; set; }
        public override AdmiralRank Rank => api_rank;

        public int api_max_chara { get; set; }
        public override int MaxShipCount => api_max_chara;

        public int api_max_slotitem { get; set; }
        public override int MaxSlotItemCount => api_max_slotitem;

        public override int MaxMaterial => api_level * 250 + 750;
    }
#nullable enable
}
