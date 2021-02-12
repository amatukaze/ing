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

        public abstract T Resolve<T>(string? serviceKey = null) where T : class;
        public abstract object Resolve(Type type, string? serviceKey = null);
        public abstract T ResolveOrDefault<T>(string? serviceKey = null) where T : class;
        public abstract object ResolveOrDefault(Type type, string? serviceKey = null);

        public abstract object? ResolveViewOrDefault(string? viewId, string? serviceKey = null);
    }
}
