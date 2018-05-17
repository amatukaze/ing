using Sakuno.KanColle.Amatsukaze.Game.Proxy;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class SessionViewModel : ModelBase
    {
        public NetworkSession Source { get; }

        public SessionViewModel(NetworkSession rpSession)
        {
            Source = rpSession;
        }
    }
}
