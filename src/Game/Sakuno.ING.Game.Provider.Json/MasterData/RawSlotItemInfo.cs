using Sakuno.ING.Game.Events.MasterData;

namespace Sakuno.ING.Game.Provider.Json.MasterData;

public partial class RawSlotItemInfo
{
    SlotItemTypeId ISlotItemInfoUpdated.TypeId => (SlotItemTypeId)api_type[2];
    int ISlotItemInfoUpdated.IconId => api_type[3];
    int ISlotItemInfoUpdated.PlaneId => api_type[4];
}
