using System;

namespace Sakuno.ING.Composition
{
    public struct Export
    {
        public Type Implementation;
        public object Contract;
        public bool SingleInstance;
        public bool LazyCreate;
    }
}
