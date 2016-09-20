namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    abstract class ApiParser : ApiParserBase
    {
        internal override sealed void Process(ApiInfo rpInfo)
        {
            OnBeforeProcessStarted(rpInfo);
            ProcessCore(rpInfo);
            OnAfterProcessCompleted(rpInfo);
        }
        public abstract void ProcessCore(ApiInfo rpInfo);
    }
    abstract class ApiParser<T> : ApiParserBase
    {
        internal override sealed void Process(ApiInfo rpInfo)
        {
            var rCoreDataJson = rpInfo.Json["api_data"];
            if (rCoreDataJson != null)
            {
                var rCoreData = rCoreDataJson.ToObject<T>();

                rpInfo.Data = rCoreData;

                OnBeforeProcessStarted(rpInfo);
                ProcessCore(rpInfo, rCoreData);
                OnAfterProcessCompleted(rpInfo);
            }
        }
        public abstract void ProcessCore(ApiInfo rpInfo, T rpData);
    }
}
