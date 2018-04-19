namespace Sakuno.KanColle.Amatsukaze.ViewModels
{
    public sealed class LocalizableText : ManualNotifyObject
    {
        internal LocalizableText(string fallback) => this.fallback = fallback;
        private string fallback;

        private string _text;
        public string Text
        {
            get => string.IsNullOrEmpty(_text) ? fallback : _text;
            internal set
            {
                _text = value;
                NotifyAllPropertyChanged();
            }
        }

        public override string ToString() => Text;
    }
}
