using System;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class ItemRecordConverter : IntArrayConverterBase<ItemRecord>
    {
        protected override int RequiredCount => 2;

        protected override ItemRecord ConvertValue(ReadOnlySpan<int> array)
            => new ItemRecord
            {
                ItemId = (UseItemId)array[0],
                Count = array[1]
            };
    }
}
