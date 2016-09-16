using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root
{
    [Api("api_port/port")]
    class PortParser : ApiParser<RawPort>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawPort rpData)
        {
            Game.MasterInfo.WaitForInitialization();

            var rSortie = Game.Sortie;
            if (rSortie != null && !(rSortie is PracticeInfo))
            {
                rSortie.ReturnTime = rpInfo.Timestamp;
                Game.RaiseReturnedFromSortie(rSortie);
            }
            Game.Sortie = null;

            Game.Port.UpdateAdmiral(rpData.Basic);
            Game.Port.UpdatePort(rpData);
        }
    }
}
