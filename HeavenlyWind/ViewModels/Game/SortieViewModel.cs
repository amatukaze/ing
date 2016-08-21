using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(SortieOverview))]
    public class SortieViewModel : TabItemViewModel
    {
        public enum DisplayType { None, Sortie, Practice }

        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Sortie; }
            protected set { throw new NotImplementedException(); }
        }

        DisplayType r_Type;
        public DisplayType Type
        {
            get { return r_Type; }
            private set
            {
                r_Type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        SortieInfo r_Info;
        public SortieInfo Info
        {
            get { return r_Info; }
            private set
            {
                if (r_Info != value)
                {
                    r_Info = value;
                    OnPropertyChanged(nameof(Info));
                }
            }
        }

        internal SortieViewModel()
        {
            SessionService.Instance.Subscribe("api_req_map/start", delegate
            {
                Info = SortieInfo.Current;
                Type = DisplayType.Sortie;
            });

            SessionService.Instance.Subscribe("api_req_member/get_practice_enemyinfo", delegate
            {
                Info = KanColleGame.Current.Sortie;
                Type = DisplayType.Practice;
            });

            SessionService.Instance.Subscribe("api_port/port", _ =>
            {
                Info = null;
                Type = DisplayType.None;
            });
        }
    }
}
