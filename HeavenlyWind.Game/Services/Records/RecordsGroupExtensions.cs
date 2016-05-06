namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    internal static class RecordsGroupExtensions
    {
        public static T ConnectAndReturn<T>(this T rpRecordsGroup) where T : RecordsGroup
        {
            rpRecordsGroup.Connect();

            return rpRecordsGroup;
        }
    }
}
