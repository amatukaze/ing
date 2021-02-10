using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Homeport
{
    public sealed class ShipViewModel : ReactiveObject
    {
        public PlayerShip Model { get; }

        public IReadOnlyCollection<SlotViewModel> Slots { get; }

        public ShipViewModel(PlayerShip ship)
        {
            Model = ship;

            Slots = ship.Slots.AsObservableChangeSet().Transform(r => new SlotViewModel(r)).Bind();
        }
    }
}
