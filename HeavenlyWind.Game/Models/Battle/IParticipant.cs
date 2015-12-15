using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public interface IParticipant
    {
        ShipInfo Info { get; }

        int Level { get; }
    }
}
