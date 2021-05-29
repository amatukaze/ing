using Sakuno.ING.Game.Json.Converters;
using System.Text.Json.Serialization;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawSlotItemLockInfo
    {
        [JsonConverter(typeof(IntToBooleanConverter))]
        public bool api_locked { get; set; }
    }
}
