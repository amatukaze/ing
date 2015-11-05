namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class WhirlpoolEvent : SortieEvent
    {
        public SortieItem LostItem { get; }
        public int Amount { get; }

        public bool HasReduceLossesWithRadar { get; }

        internal WhirlpoolEvent()
        {

        }
    }
}
