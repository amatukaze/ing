using System;

namespace Sakuno.KanColle.Amatsukaze.Extensibility
{
    public interface IToolPane
    {
        string Name { get; }

        Lazy<object> View { get; }
    }
}
