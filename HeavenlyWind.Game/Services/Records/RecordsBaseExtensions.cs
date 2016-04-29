namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    internal static class RecordsBaseExtensions
    {
        public static T ConnectAndReturn<T>(this T rpRecordsGroup) where T : RecordsBase
        {
            rpRecordsGroup.Connect();

            return rpRecordsGroup;
        }
    }
}
