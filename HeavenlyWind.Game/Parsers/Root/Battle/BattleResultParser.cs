using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_sortie/battleresult")]
    [Api("api_req_combined_battle/battleresult")]
    class BattleResultParser : ApiParser<RawBattleResult>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawBattleResult rpData)
        {
            if (!Preference.Instance.Game.ShowDrop)
                return;

            var rMasterInfo = KanColleGame.Current.MasterInfo;

            string rDroppedShip = null;
            if (rpData.DroppedShip != null)
            {
                var rShip = rMasterInfo.Ships[rpData.DroppedShip.ID];

                if (rShip.Rarity < 4)
                    rDroppedShip = rShip.TranslatedName;
                else if (rShip.Rarity < 7)
                    rDroppedShip = "[b]" + rShip.TranslatedName + "[/b]";
                else
                    rDroppedShip = "[b][color=yellow]" + rShip.TranslatedName + "[/color][/b]";
            }

            if (rDroppedShip == null && rpData.DroppedItem == null)
                return;

            if (rDroppedShip != null && rpData.DroppedItem != null)
                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_ShipAndItem_Dropped,
                    rDroppedShip, rMasterInfo.Items[rpData.DroppedItem.ID].TranslatedName));
            else
                Logger.Write(LoggingLevel.Info, string.Format(StringResources.Instance.Main.Log_ShipOrItem_Dropped,
                    rpData.DroppedShip != null ? rDroppedShip : rMasterInfo.Items[rpData.DroppedItem.ID].TranslatedName));
        }
    }
}
