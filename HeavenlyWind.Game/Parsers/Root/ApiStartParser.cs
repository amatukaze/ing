using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root
{
    [Api("api_start2")]
    class ApiStartParser : ApiParser<RawMasterInfo>
    {
        public override void Process(RawMasterInfo rpData)
        {
            Game.MasterInfo.Update(rpData);

            Logger.Write(LoggingLevel.Info, StringResources.Instance.Main.Log_Welcome);
        }
    }
}
