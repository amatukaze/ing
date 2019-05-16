namespace Sakuno.ING.Views.Desktop.Documents
{
    public class TranslatedResourceText : TranslatableText
    {
        private string _key;
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                var split = value.Split('/');
                if (Translate)
                    SetCurrentValue(TextProperty, Localization.GetLocalized(split[0], split[1]));
                else
                    SetCurrentValue(TextProperty, Localization.GetUnlocalized(split[0], split[1]));
            }
        }
    }
}
