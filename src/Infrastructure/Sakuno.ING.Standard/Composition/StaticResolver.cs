using System;

namespace Sakuno.ING.Composition
{
    public static class StaticResolver
    {
        private static IResolver _instance;
        public static IResolver Instance
        {
            get => _instance;
            set
            {
                if (_instance != null)
                    throw new InvalidOperationException("Resolver instance can be only set at initializion.");

                _instance = value;
            }
        }
    }
}
