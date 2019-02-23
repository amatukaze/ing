using Newtonsoft.Json.Linq;
using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Events.Combat
{
    public readonly struct BattleDetail
    {
        public BattleDetail(RawBattle parsed, JToken unparsed)
        {
            Parsed = parsed;
            Unparsed = unparsed;
        }

        public RawBattle Parsed { get; }
        public JToken Unparsed { get; }
    }
}
