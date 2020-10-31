using Sakuno.ING.Game.Events;
using System;

namespace Sakuno.ING.Game
{
    public partial class GameProvider
    {
        public IObservable<MasterDataUpdate> MasterDataUpdated { get; private set; }
    }
}
