using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class AirForceGroupViewModel : ReactiveObject
    {
        public AirForceGroup Model { get; }

        public IReadOnlyCollection<AirForceSquadronViewModel> Squadrons { get; }

        public AirForceGroupViewModel(AirForceGroup airForceGroup)
        {
            Model = airForceGroup;

            Squadrons = airForceGroup.Squadrons.DefaultViewSource.Transform(r => new AirForceSquadronViewModel(r)).Bind();
        }
    }
}
