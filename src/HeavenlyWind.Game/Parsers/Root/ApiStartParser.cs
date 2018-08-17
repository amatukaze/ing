using System;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root
{
    [Api("api_start2/getData")]
    class ApiStartParser : ApiParser<RawMasterInfo>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawMasterInfo rpData)
        {
            Game.MasterInfo.Update(rpData);
            Game.IsStarted = true;

            Logger.Write(LoggingLevel.Info, StringResources.Instance.Main.Log_Welcome);
        }
    }
}
