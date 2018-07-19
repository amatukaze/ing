using System.ComponentModel;

namespace Sakuno.ING.Game
{
    public class TextTranslationGroup : IBindable
    {
        public string Origin { get; internal set; }
        public string Translation { get; internal set; }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { }
            remove { }
        }

        public override string ToString() => Origin ?? Translation;
    }
}
