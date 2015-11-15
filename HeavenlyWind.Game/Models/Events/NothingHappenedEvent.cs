namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class NothingHappenedEvent : SortieEvent
    {
        public string Message { get; }

        public bool CanManuallySelectRoute { get; }

        internal NothingHappenedEvent()
        {
        }
    }
}
