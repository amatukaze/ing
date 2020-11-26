namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class SlotItemInfo
    {
        partial void UpdateCore(RawSlotItemInfo raw)
        {
            Type = _owner.SlotItemTypes[raw.TypeId];
        }
    }
}
