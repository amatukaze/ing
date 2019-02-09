using System;
using Sakuno.ING.Game.Json.Combat;

namespace Sakuno.ING.Game.Events.Combat
{
    public readonly struct BattleDetail
    {
        public BattleDetail(BattleApi parsed, ReadOnlyMemory<byte> unparsed)
        {
            Parsed = parsed;
            Unparsed = unparsed;
        }

        public BattleApi Parsed { get; }
        public ReadOnlyMemory<byte> Unparsed { get; }
    }
}
