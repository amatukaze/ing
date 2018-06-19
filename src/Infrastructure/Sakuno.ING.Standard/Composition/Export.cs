using System;

namespace Sakuno.ING.Composition
{
    public struct Export
    {
        public Type ImplementationType;
        public Type ContractType;
        public bool SingleInstance;
        public bool LazyCreate;
    }
}
