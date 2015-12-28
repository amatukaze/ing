using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public enum NothingHappenedMessage { Imagination, NoSighOfTheEnemy, ManualSelection }

    public class NothingHappenedEvent : SortieEvent
    {
        public NothingHappenedMessage Message => (NothingHappenedMessage)RawData.CellEventSubType;

        public bool CanManuallySelectRoute { get; }

        internal NothingHappenedEvent(RawMapExploration rpData) : base(rpData) { }
    }
}
