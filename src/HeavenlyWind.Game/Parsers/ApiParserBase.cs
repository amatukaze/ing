using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    abstract class ApiParserBase
    {
        public KanColleGame Game => KanColleGame.Current;

        public string Api { get; internal set; }

        internal event Action<ApiInfo> BeforeProcessStarted;
        internal event Action<ApiInfo> AfterProcessCompleted;

        internal abstract void Process(ApiInfo rpInfo);

        protected void OnBeforeProcessStarted(ApiInfo rpInfo) => BeforeProcessStarted?.Invoke(rpInfo);

        protected void OnAfterProcessCompleted(ApiInfo rpInfo) => AfterProcessCompleted?.Invoke(rpInfo);
    }
}
