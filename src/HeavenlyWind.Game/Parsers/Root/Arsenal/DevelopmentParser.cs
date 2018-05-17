using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Arsenal
{
    [Api("api_req_kousyou/createitem")]
    class DevelopmentParser : ApiParser<RawEquipmentDevelopment>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawEquipmentDevelopment rpData)
        {
            var rFuelConsumption = int.Parse(rpInfo.Parameters["api_item1"]);
            var rBulletConsumption = int.Parse(rpInfo.Parameters["api_item2"]);
            var rSteelConsumption = int.Parse(rpInfo.Parameters["api_item3"]);
            var rBauxiteConsumption = int.Parse(rpInfo.Parameters["api_item4"]);

            Game.Port.Materials.Update(rpData.Materials);

            string rLogContent;
            if (!rpData.Success)
                rLogContent = string.Format(StringResources.Instance.Main.Log_Development_Failure,
                    rFuelConsumption, rBulletConsumption, rSteelConsumption, rBauxiteConsumption);
            else
            {
                var rEquipment = new Equipment(new RawEquipment() { ID = rpData.Result.ID, EquipmentID = rpData.Result.EquipmentID });
                Game.Port.AddEquipment(rEquipment);

                var rInfo = rEquipment.Info;
                var rMessage = rInfo.Rarity > 0 ? StringResources.Instance.Main.Log_Development_Success_Rare : StringResources.Instance.Main.Log_Development_Success;

                rLogContent = string.Format(rMessage, rInfo.TranslatedName, rFuelConsumption, rBulletConsumption, rSteelConsumption, rBauxiteConsumption);
            }

            Logger.Write(LoggingLevel.Info, rLogContent);
        }
    }
}
