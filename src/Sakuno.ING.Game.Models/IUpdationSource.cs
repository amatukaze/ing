using System;

namespace Sakuno.ING.Game
{
    public interface IUpdationSource
    {
        event Action Updated;
    }
}
