using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;

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

            var multipleDevelopment = rpInfo.Parameters["api_multiple_flag"] == "1";

            Game.Port.Materials.Update(rpData.Materials);

            string rLogContent;
            if (!rpData.Success)
                rLogContent = string.Format(multipleDevelopment ? StringResources.Instance.Main.Log_MultipleDevelopment_Failure : StringResources.Instance.Main.Log_Development_Failure,
                    rFuelConsumption, rBulletConsumption, rSteelConsumption, rBauxiteConsumption);
            else
            {
                var names = new List<string>(rpData.Results.Length);

                foreach (var item in rpData.Results)
                {
                    if (item.EquipmentID <= 0)
                        continue;

                    var equipment = new Equipment(new RawEquipment() { ID = item.ID, EquipmentID = item.EquipmentID });
                    Game.Port.AddEquipment(equipment);

                    if (equipment.Info.Rarity < 2)
                        names.Add(equipment.Info.TranslatedName);
                    else
                        names.Add($"[b]{equipment.Info.TranslatedName}[/b]");
                }

                var developed = names.Join(StringResources.Instance.Main.Log_Modernization_Separator_Type1);
                var template = multipleDevelopment ? StringResources.Instance.Main.Log_MultipleDevelopment_Success : StringResources.Instance.Main.Log_Development_Success;

                rLogContent = string.Format(template, developed, rFuelConsumption, rBulletConsumption, rSteelConsumption, rBauxiteConsumption);
            }

            Logger.Write(LoggingLevel.Info, rLogContent);
        }
    }
}
