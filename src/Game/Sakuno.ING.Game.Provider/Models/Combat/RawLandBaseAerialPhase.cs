using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public class RawLandBaseAerialPhase : RawAerialPhase
    {
        public int GroupId { get; }
        public IReadOnlyList<EquipmentRecord> Squadrons { get; }
        public RawLandBaseAerialPhase(BattleDetailJson.LandBase api) : base(api)
        {
            GroupId = api.api_base_id;
            Squadrons = api.api_squadron_plane.Select(x => new EquipmentRecord
            {
                Id = (EquipmentInfoId)x.api_mst_id,
                Count = x.api_count
            }).ToArray();
        }
    }
}
