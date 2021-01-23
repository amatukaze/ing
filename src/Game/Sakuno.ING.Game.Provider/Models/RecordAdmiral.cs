using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models
{
#nullable disable
    internal sealed class RecordAdmiral : RawAdmiral
    {
        public int api_member_id { get; set; }
        public override int Id => api_member_id;

        public string api_nickname { get; set; }
        public override string Name => api_nickname;

        public int api_level { get; set; }
        public int[] api_experience { get; set; }
        public override Leveling Leveling => new Leveling(api_level,
            api_experience[0],
            KnownLeveling.GetAdmiralExp(api_level),
            KnownLeveling.GetAdmiralExp(api_level + 1),
            api_level >= KnownLeveling.MaxAdmiralLevel);

        public AdmiralRank api_rank { get; set; }
        public override AdmiralRank Rank => api_rank;

        public int[] api_ship { get; set; }
        public override int MaxShipCount => api_ship[1];

        public int[] api_slotitem { get; set; }
        public override int MaxSlotItemCount => api_slotitem[1];

        public int api_material_max { get; set; }
        public override int MaxMaterial => api_material_max;
    }
#nullable enable
}
