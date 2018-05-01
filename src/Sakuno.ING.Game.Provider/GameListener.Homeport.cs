using System.Collections.Generic;
using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game
{
    partial class GameListener
    {
        public readonly IProducer<ITimedMessage<IHomeportUpdate>> HomeportUpdated;
        public readonly IProducer<ITimedMessage<IRawAdmiral>> AdmiralUpdated;
        public readonly IProducer<ITimedMessage<IMaterialsUpdate>> MaterialsUpdated;
        public readonly IProducer<ITimedMessage<IReadOnlyCollection<IRawRepairingDock>>> RepairingDockUpdated;
    }
}
