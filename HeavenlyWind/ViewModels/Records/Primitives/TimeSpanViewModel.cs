namespace Sakuno.KanColle.Amatsukaze.ViewModels.Records.Primitives
{
    abstract class TimeSpanViewModel : ModelBase
    {
        public TimeSpanType Type { get; }

        public string TimeSpanStart { get; protected set; }
        public string TimeSpanEnd { get; protected set; }

        protected TimeSpanViewModel(TimeSpanType rpType)
        {
            Type = rpType;
        }
    }
}
