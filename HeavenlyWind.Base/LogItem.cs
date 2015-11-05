using System;

namespace Sakuno.KanColle.Amatsukaze
{
    public class LogItem
    {
        public DateTime Time { get; }
        public LoggingLevel Level { get; }

        public string Content { get; }

        public LogItem(DateTime rpTime, LoggingLevel rpLevel, string rpContent)
        {
            Time = rpTime;
            Level = rpLevel;
            Content = rpContent;
        }

        public override string ToString() => $"[{Time} {Level}] {Content}";
    }
}
