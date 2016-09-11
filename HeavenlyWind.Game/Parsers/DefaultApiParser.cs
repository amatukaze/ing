namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    sealed class DefaultApiParser : ApiParserBase
    {
        internal override void Process(ApiInfo rpInfo)
        {
            OnBeforeProcessStarted(rpInfo);
            OnAfterProcessCompleted(rpInfo);
        }
    }
}
