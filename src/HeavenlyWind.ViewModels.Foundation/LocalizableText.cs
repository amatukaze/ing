namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public sealed class LocalizableText : ManualNotifyObject
    {
        internal LocalizableText() { }

        private string _text;
        public string Text
        {
            get => _text;
            internal set
            {
                _text = value;
                NotifyAllPropertyChanged();
            }
        }

        public override string ToString() => Text;
    }
}
