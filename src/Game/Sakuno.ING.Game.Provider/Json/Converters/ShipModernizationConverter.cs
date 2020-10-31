using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class ShipModernizationConverter : IntArrayConverterBase<ShipModernizationStatus>
    {
        protected override int MaxLength => 2;

        protected override ShipModernizationStatus Parse(ReadOnlySpan<int> span) =>
            new ShipModernizationStatus
            (
                min: span[0],
                max: span[1]
            );
    }
}
