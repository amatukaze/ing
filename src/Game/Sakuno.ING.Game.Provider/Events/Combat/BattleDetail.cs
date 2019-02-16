using System;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Events.Combat
{
    public readonly struct BattleDetail
    {
        public BattleDetail(RawBattle parsed, ReadOnlyMemory<byte> unparsed)
        {
            Parsed = parsed;
            Unparsed = unparsed;
        }

        public RawBattle Parsed { get; }
        public ReadOnlyMemory<byte> Unparsed { get; }
    }
}
