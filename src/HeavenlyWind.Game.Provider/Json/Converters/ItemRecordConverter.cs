using System;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.Converters
{
    internal class ItemRecordConverter : IntArrayConverterBase<ItemRecord>
    {
        protected override int RequiredCount => 2;

        protected override ItemRecord ConvertValue(ReadOnlySpan<int> array)
            => new ItemRecord
            {
                ItemId = array[0],
                Count = array[1]
            };
    }
}
