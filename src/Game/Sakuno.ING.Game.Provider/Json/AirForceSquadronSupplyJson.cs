using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class AirForceSquadronSupplyJson : IMaterialUpdate
    {
        public int api_after_fuel { get; set; }
        public int api_after_bauxite { get; set; }

        public RawAirForceSquadron[] api_plane_info { get; set; }

        public void Apply(Materials materials)
        {
            materials.Fuel = api_after_fuel;
            materials.Bauxite = api_after_bauxite;
        }
    }
#nullable enable
}
