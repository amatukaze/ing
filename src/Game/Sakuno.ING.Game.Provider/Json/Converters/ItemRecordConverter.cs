using System;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class ItemRecordConverter : IntArrayConverterBase<UseItemRecord?>
    {
        protected override int RequiredCount => 2;

        protected override UseItemRecord? ConvertValue(ReadOnlySpan<int> array)
            => array[0] > 0 ?
            new UseItemRecord
            {
                ItemId = (UseItemId)array[0],
                Count = array[1]
            } : (UseItemRecord?)null;
    }
}
