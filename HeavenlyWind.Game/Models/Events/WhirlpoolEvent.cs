using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public SortieItem LostItem { get; }
        public int Amount { get; }

        public bool HasReduceLossesWithRadar { get; }

        internal WhirlpoolEvent(RawMapExploration rpData) : base(rpData)
        {
            LostItem = rpData.Whirlpool.MaterialType;
            Amount = rpData.Whirlpool.Amount;
        }
    }
}
