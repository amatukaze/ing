namespace Sakuno.KanColle.Amatsukaze.ViewModels.Preferences
{
    public class PreferencesWindowViewModel : WindowViewModel
    {
        public static PreferencesWindowViewModel Instance { get; } = new PreferencesWindowViewModel();

        PreferencesWindowViewModel()
        {
            Title = StringResources.Instance.Main.Window_Preferences;
        }
    }
}
