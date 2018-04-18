using System;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.Converters
{
    internal class ShipMordenizationConverter : IntArrayConverterBase<ShipMordenizationStatus>
    {
        protected override int RequiredCount => 2;

        protected override ShipMordenizationStatus ConvertValue(int[] array)
            => new ShipMordenizationStatus(array[0], array[1], 0);
    }
}
