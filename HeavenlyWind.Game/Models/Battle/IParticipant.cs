using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public interface IParticipant
    {
        ShipInfo Info { get; }
        bool IsAbyssalShip { get; }

        int Level { get; }
        IList<ShipSlot> Slots { get; }
    }
}
