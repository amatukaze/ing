using System;

namespace Sakuno.ING.Composition
{
    public abstract class Compositor
    {
        private static Compositor? _default;
        public static Compositor Default => _default ?? throw new InvalidOperationException("Not initialized");

        protected Compositor()
        {
            _default ??= this;
        }

        public abstract T Resolve<T>() where T : class;
        public abstract object Resolve(Type type);
        public abstract object? ResolveViewOrDefault(string? viewId);
    }
}
