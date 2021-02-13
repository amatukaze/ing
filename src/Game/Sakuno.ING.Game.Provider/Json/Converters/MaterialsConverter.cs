using System;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class MaterialsConverter : IntArrayConverterBase<Materials>
    {
        protected override int MaxLength => 8;
        protected override int MinLength => 4;

        protected override Materials Parse(ReadOnlySpan<int> span) => new Materials()
        {
            Fuel = span[0],
            Bullet = span[1],
            Steel = span[2],
            Bauxite = span[3],
            InstantBuild = span[4],
            InstantRepair = span[5],
            Development = span[6],
            Improvement = span[7],
        };
    }
}
