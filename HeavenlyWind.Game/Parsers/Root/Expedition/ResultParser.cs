using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.ComponentModel;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Expedition
{
    [Api("api_req_mission/result")]
    class ResultParser : ApiParser<RawExpeditionResult>
    {
        public override void Process(RawExpeditionResult rpData)
        {
            var rFleet = Game.Port.Fleets[int.Parse(Requests["api_deck_id"])];
            var rExpeditionID = Game.MasterInfo.Expeditions.Values.Single(r => r.Name == rpData.Name).ID;

            var rLogContent = string.Format(StringResources.Instance.Main.Log_ExpeditionResult,
                rFleet.ID, rFleet.Name, rExpeditionID, rpData.Name, GetStringFromExpeditionResult(rpData.Result));
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
