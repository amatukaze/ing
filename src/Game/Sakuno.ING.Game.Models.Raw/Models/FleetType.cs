namespace Sakuno.ING.Game.Models
{
    public enum FleetType
    {
        None = 0,
        /// <summary>
        /// 通常艦隊
        /// </summary>
        SingleFleet = 1,
        /// <summary>
        /// 空母機動部隊
        /// </summary>
        CarrierTaskForceFleet = 2,
        /// <summary>
        /// 水上打撃部隊
        /// </summary>
        SurfaceTaskForceFleet = 3,
        /// <summary>
        /// 輸送護衛部隊
        /// </summary>
        TransportEscortFleet = 4,
        /// <summary>
        /// 遊撃部隊
        /// </summary>
        StrikingForceFleet = 5,
    }
}
