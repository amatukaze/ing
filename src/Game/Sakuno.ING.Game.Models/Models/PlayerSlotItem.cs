namespace Sakuno.ING.Game.Models
{
    public partial class PlayerSlotItem
    {
        partial void UpdateCore(RawSlotItem raw)
        {
            Info = _owner.MasterData.SlotItemInfos[raw.SlotItemInfoId];
        }
    }
}
