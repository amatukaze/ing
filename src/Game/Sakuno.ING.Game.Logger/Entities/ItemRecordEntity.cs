using Microsoft.EntityFrameworkCore;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Logger.Entities
{
    [Owned]
    public class ItemRecordEntity
    {
        public UseItemId ItemId { get; set; }
        public int Count { get; set; }
        public static implicit operator ItemRecordEntity(ItemRecord record)
            => new ItemRecordEntity
            {
                ItemId = record.ItemId,
                Count = record.Count
            };
        public static implicit operator ItemRecord(ItemRecordEntity entity)
            => new ItemRecord
            {
                ItemId = entity.ItemId,
                Count = entity.Count
            };
    }
}
