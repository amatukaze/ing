namespace Sakuno.KanColle.Amatsukaze.Game.Services.Records
{
    internal static class RecordBaseExtension
    {
        public static T ConnectAndReturn<T>(this T rpRecordGroup) where T : RecordBase
        {
            rpRecordGroup.Connect();

            return rpRecordGroup;
        }
    }
}
