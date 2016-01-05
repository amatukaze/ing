using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_sortie/battleresult")]
    [Api("api_req_combined_battle/battleresult")]
    class BattleResultParser : ApiParser<RawBattleResult>
    {
        public override void Process(RawBattleResult rpData)
        {
            if (rpData.DroppedShip != null && rpData.DroppedItem != null)
                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_ShipAndItem_Dropped,
                    rpData.DroppedShip.Name, KanColleGame.Current.MasterInfo.Items[rpData.DroppedItem.ID].Name));
            else if (rpData.DroppedShip != null || rpData.DroppedItem != null)
                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_ShipOrItem_Dropped,
                    rpData.DroppedShip != null ? rpData.DroppedShip.Name : KanColleGame.Current.MasterInfo.Items[rpData.DroppedItem.ID].Name));
        }
    }
}
