using System.Collections.Generic;

namespace Sakuno.ING.Composition
{
    public readonly struct WithMeta<T>
        where T : class
    {
        public WithMeta(T value, IDictionary<string, object> metaData)
        {
            Value = value;
            MetaData = metaData;
        }

        public T Value { get; }
        public IDictionary<string, object> MetaData { get; }
    }
}
