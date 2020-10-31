namespace Sakuno.ING.Game.Models.MasterData
{
#nullable disable
    public sealed class RawShipEquipLimitation
    {
        public ShipInfoId api_ship_id { get; set; }
        public SlotItemTypeId[] api_equip_type { get; set; }
    }
#nullable enable
}
