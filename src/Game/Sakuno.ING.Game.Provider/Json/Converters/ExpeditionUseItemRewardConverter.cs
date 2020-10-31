using Sakuno.ING.Game.Models;
using System;
using System.Runtime.CompilerServices;

namespace Sakuno.ING.Game.Json.Converters
{
    internal sealed class ExpeditionUseItemRewardConverter : IntArrayConverterBase<ExpeditionUseItemReward?>
    {
        protected override int MaxLength => 2;

        protected override ExpeditionUseItemReward? Parse(ReadOnlySpan<int> span)
        {
            if (span[0] == 0)
                return null;

            return Unsafe.As<int, ExpeditionUseItemReward>(ref Unsafe.AsRef(span.GetPinnableReference()));
        }
    }
}
