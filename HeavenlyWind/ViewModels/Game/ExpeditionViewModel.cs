using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class ExpeditionViewModel : ModelBase
    {
        public ExpeditionInfo Info { get; }
        ExpeditionInfo2 r_Info2;

        public string TimeString
        {
            get
            {
                var rTime = TimeSpan.FromMinutes(Info.Time);
                return ((int)rTime.TotalHours).ToString("D2") + rTime.ToString(@"\:mm");
            }
        }

        public ExpeditionInfo2.RawRewardResources RewardResources => r_Info2.RewardResources;
        public Tuple<ItemInfo, int> RewardItem1 => Info.RewardItem1ID != 0 ? Tuple.Create(KanColleGame.Current.MasterInfo.Items[Info.RewardItem1ID], Info.RewardItem1Count) : null;
        public Tuple<ItemInfo, int> RewardItem2 => Info.RewardItem2ID != 0 ? Tuple.Create(KanColleGame.Current.MasterInfo.Items[Info.RewardItem2ID], Info.RewardItem2Count) : null;

        public IList<ExpeditionResultPrediction> ResultPrediction { get; }

        internal ExpeditionViewModel(ExpeditionInfo rpInfo)
        {
            Info = rpInfo;
            r_Info2 = ExpeditionService.Instance.GetInfo(rpInfo.ID);

            ResultPrediction = KanColleGame.Current.Port.Fleets.Table.Values.Skip(1).Select(r => new ExpeditionResultPrediction(r_Info2, r)).ToList();
        }

        internal void Update(Fleet rpFleet) => ResultPrediction[rpFleet.ID - 2].Check();
    }
}
