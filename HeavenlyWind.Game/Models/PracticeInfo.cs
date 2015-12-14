using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using Sakuno.KanColle.Amatsukaze.Game.Services;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class PracticeInfo : SortieInfo
    {
        static PracticeInfo r_Current;

        public PracticeOpponentInfo Opponent { get; }

        BattleInfo r_Battle;
        public BattleInfo Battle
        {
            get { return r_Battle; }
            internal set
            {
                r_Battle = value;
                OnPropertyChanged(nameof(Battle));
            }
        }

        static PracticeInfo()
        {
            SessionService.Instance.Subscribe("api_port/port", _ => r_Current = null);
        }
        internal PracticeInfo(RawPracticeOpponentInfo rpRawData)
        {
            r_Current = this;

            Opponent = new PracticeOpponentInfo(rpRawData);
        }
    }
}
