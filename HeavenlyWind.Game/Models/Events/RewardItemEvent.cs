namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class RewardItemEvent : SortieEvent
    {
        public SortieItem Item { get; }
        public int Quantity { get; }

        internal RewardItemEvent()
        {
        }
    }
}
