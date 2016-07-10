using Sakuno.KanColle.Amatsukaze.Game.Proxy;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    public class SessionViewModel : ModelBase
    {
        public NetworkSession Source { get; }

        public SessionViewModel(NetworkSession rpSession)
        {
            Source = rpSession;
        }
    }
}
