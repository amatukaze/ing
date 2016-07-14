using System;
using System.Collections.Concurrent;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    class Property
    {
        public static readonly ConcurrentDictionary<Type, Property> Cache = new ConcurrentDictionary<Type, Property>();

        public Delegate Getter { get; }
        public Delegate Setter { get; }

        public Property(Delegate rpGetter, Delegate rpSetter)
        {
            Getter = rpGetter;
            Setter = rpSetter;
        }
    }
}
