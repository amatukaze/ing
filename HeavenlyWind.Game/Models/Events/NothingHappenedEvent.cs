using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Events
{
    public class NothingHappenedEvent : SortieEvent
    {
        public string Message { get; }

        public bool CanManuallySelectRoute { get; }

        internal NothingHappenedEvent(RawMapExploration rpData) : base(rpData)
        {
            if (rpData.CellEventSubType == 0)
                Message = "気のせいだった。";
            else if (rpData.CellEventSubType == 1)
                Message = "敵影を見ず。";
            else if (rpData.CellEventSubType == 3)
                Message = "能動分岐";
        }
    }
}
