using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Converters
{
    internal class ShipMordenizationConverter : IntArrayConverterBase<ShipMordenizationStatus>
    {
        protected override int RequiredCount => 2;

        protected override ShipMordenizationStatus ConvertValue(int[] array)
            => new ShipMordenizationStatus
            {
                Min = array[0],
                Max = array[1]
            };
    }
}
