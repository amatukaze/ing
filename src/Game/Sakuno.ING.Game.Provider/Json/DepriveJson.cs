using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class DepriveJson : IMaterialsUpdate
    {
        public class Data
        {
            public RawShip api_unset_ship;
            public RawShip api_set_ship;
        }
        public Data api_ship_data;
        public int? api_bauxite;

        public MaterialsChangeReason Reason => MaterialsChangeReason.ShipEquip;

        public void Apply(ref Materials materials)
        {
            if (api_bauxite is int bauxite)
                materials.Bauxite = bauxite;
        }
    }
}
