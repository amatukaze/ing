using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class HomeportMaterialUpdate : IMaterialUpdate
    {
        private readonly RawMaterialItem[] _items;

        public HomeportMaterialUpdate(RawMaterialItem[] items)
        {
            _items = items;
        }
    }
}
