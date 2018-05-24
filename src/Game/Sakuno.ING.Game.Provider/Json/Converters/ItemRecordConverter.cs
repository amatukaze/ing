﻿using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class ItemRecordConverter : IntArrayConverterBase<ItemRecord>
    {
        protected override int RequiredCount => 2;

        protected override ItemRecord ConvertValue(int[] array)
            => new ItemRecord
            {
                ItemId = array[0],
                Count = array[1]
            };
    }
}