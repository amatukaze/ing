namespace Sakuno.ING.Game.Models
{
    public interface IRawMap : IIdentifiable
    {
        bool IsCleared { get; }
        EventMapRank? Rank { get; }
        int? GaugeIndex { get; }
        EventMapGaugeType? GaugeType { get; }
        ClampedValue? Gauge { get; }
        int AvailableAirForceGroups { get; }
        int? DefeatedCount { get; }
    }

    public enum EventMapRank
    {
        None = 0,
        /// <summary>
        /// 丁
        /// </summary>
        Casual = 1,
        /// <summary>
        /// 丙
        /// </summary>
        Easy = 2,
        /// <summary>
        /// 乙
        /// </summary>
        Normal = 3,
        /// <summary>
        /// 甲
        /// </summary>
        Hard = 4
    }

    public enum EventMapGaugeType
    {
        HP = 2,
        Transport = 3
    }
}
