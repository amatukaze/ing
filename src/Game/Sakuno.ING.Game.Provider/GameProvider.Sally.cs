using Sakuno.ING.Game.Models;
using System;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<RawMap[]> MapsUpdated { get; private set; }
    }
}
