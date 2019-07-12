using Sakuno.ING.Game.Models.Knowledge;

namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class EquipmentTypeInfo
    {
        public bool IsPlane { get; private set; }
        partial void CreateCore() => IsPlane = Id.IsPlane();
    }
}
