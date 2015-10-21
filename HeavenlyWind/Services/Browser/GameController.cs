using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class GameController : ModelBase
    {
        public ICommand RestartGameCommand { get; }

        public GameController()
        {
            RestartGameCommand = new DelegatedCommand(RestartGame);
        }

        void RestartGame()
        {
            BrowserService.Instance.Navigator.Refresh();
        }

    }
}
