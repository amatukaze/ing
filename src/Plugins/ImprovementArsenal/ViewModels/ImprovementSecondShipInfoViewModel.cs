using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Plugin.ImprovementArsenal.ViewModels
{
    class ImprovementSecondShipInfoViewModel : ModelBase
    {
        public ShipInfo Info { get; }

        public IList<ImprovementDay> Days { get; }

        bool r_IsVisible;
        public bool IsVisible
        {
            get { return r_IsVisible; }
            set
            {
                if (r_IsVisible != value)
                {
                    r_IsVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        public ImprovementSecondShipInfoViewModel(int rpID, int rpDays)
        {
            if (rpID != 0)
                Info = KanColleGame.Current.MasterInfo.Ships[rpID];

            Days = Enumerable.Range(0, 7).Select(r => ImprovementDay.Get(r, (rpDays & (1 << r)) != 0)).ToArray();
        }
    }
}
