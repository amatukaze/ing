using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public class InstantRepair : IMaterialsUpdate
    {
        public MaterialsChangeReason Reason => MaterialsChangeReason.InstantRepair;

        public void Apply(ref Materials materials) => materials.InstantRepair--;
    }
}
