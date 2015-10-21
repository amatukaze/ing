using Sakuno.SystemInterop;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class GameController : ModelBase
    {
        BrowserVolume r_Volume;
        public BrowserVolume Volume
        {
            get { return r_Volume; }
            private set
            {
                if (r_Volume != value)
                {
                    r_Volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public ICommand RestartGameCommand { get; }

        public GameController()
        {
            if (OS.IsWin7OrLater)
                VolumeManager.Instance.NewSession += r =>
                {
                    Volume?.Dispose();
                    Volume = new BrowserVolume(r);
                };

            RestartGameCommand = new DelegatedCommand(RestartGame);
        }

        void RestartGame()
        {
            BrowserService.Instance.Navigator.Refresh();
        }

    }
}
