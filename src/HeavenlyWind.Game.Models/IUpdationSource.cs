using System;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public interface IUpdationSource
    {
        event Action Updated;
    }
}
