using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.ComponentModel;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Expedition
{
    [Api("api_req_mission/result")]
    class ResultParser : ApiParser<RawExpeditionResult>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawExpeditionResult rpData)
        {
            var rFleet = Game.Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];
            var rExpedition = rFleet.ExpeditionStatus.Expedition;

            var rLogContent = string.Format(StringResources.Instance.Main.Log_ExpeditionResult,
                rFleet.ID, rFleet.Name, rExpedition.ID, rExpedition.TranslatedName, GetStringFromExpeditionResult(rpData.Result));

            var rBucketCount = 0;
            if (rpData.RewardItems[0] == 1)
                rBucketCount = rpData.Item1.Count;
            else if (rpData.RewardItems[1] == 1)
                rBucketCount = rpData.Item2.Count;

            if (rBucketCount > 0)
                rLogContent += " [icon]bucket[/icon] " + rBucketCount;

            Logger.Write(LoggingLevel.Info, rLogContent);
        }

        static string GetStringFromExpeditionResult(ExpeditionResult rpResult)
        {
            switch (rpResult)
            {
                case ExpeditionResult.Failure: return StringResources.Instance.Main.Result_Failure;
                case ExpeditionResult.Success: return StringResources.Instance.Main.Result_Success;
                case ExpeditionResult.GreatSuccess: return StringResources.Instance.Main.Result_GreatSuccess;

                default: throw new InvalidEnumArgumentException(nameof(rpResult), (int)rpResult, typeof(ExpeditionResult));
            }
        }
    }
}
