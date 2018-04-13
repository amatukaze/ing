using System;
using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.Game.Json.Converters
{
    internal class MaterialsConverter : IntArrayConverterBase<Materials>
    {
        protected override int RequiredCount => 4;

        protected override Materials ConvertValue(ReadOnlySpan<int> array)
            => new Materials
            {
                Fuel = array[0],
                Bullet = array[1],
                Steel = array[2],
                Bauxite = array[3]
            };
    }
}
