namespace Sakuno.ING.Game.Models.Battle
{
    public enum AirFightingResult
    {
        /// <summary>制空均衡</summary>
        Parity = 0,
        /// <summary>制空権確保</summary>
        Supremacy = 1,
        /// <summary>航空優勢</summary>
        Superiority = 2,
        /// <summary>航空劣勢</summary>
        Denial = 3,
        /// <summary>制空権喪失</summary>
        Incapability = 4
    }
}
