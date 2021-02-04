using ReactiveUI;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class AirForceSquadronViewModel : ReactiveObject
    {
        public AirForceSquadron Model { get; }

        public AirForceSquadronViewModel(AirForceSquadron squadron)
        {
            Model = squadron;
        }
    }
}
