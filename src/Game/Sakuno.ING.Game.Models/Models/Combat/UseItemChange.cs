using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Combat
{
    public readonly struct UseItemChange
    {
        public UseItemChange(UseItemInfo item, int count)
        {
            Item = item;
            Count = count;
        }

        public UseItemInfo Item { get; }
        public int Count { get; }
    }
}
