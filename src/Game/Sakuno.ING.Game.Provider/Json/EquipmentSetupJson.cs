using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class EquipmentSetupJson : IMaterialsUpdate
    {
        public int? api_bauxite;

        public MaterialsChangeReason Reason => MaterialsChangeReason.ShipEquip;

        public void Apply(ref Materials materials)
        {
            if (api_bauxite is int bauxite)
                materials.Bauxite = bauxite;
        }
    }
}
