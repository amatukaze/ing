using System;
using System.Runtime.CompilerServices;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class MaterialsConverter : IntArrayConverterBase<Materials>
    {
        protected override int MaxLength => 8;
        protected override int MinLength => 4;

        protected override Materials Parse(ReadOnlySpan<int> span) =>
            Unsafe.As<int, Materials>(ref Unsafe.AsRef(span.GetPinnableReference()));
    }
}
