using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.ING.Composition
{
    public class Compositor : IDisposable
    {
        private readonly Compositor parent;
        private Dictionary<Type, Type> impls = new Dictionary<Type, Type>();
        private Dictionary<Type, (ConstructorInfo creation, bool singleton)> creations = new Dictionary<Type, (ConstructorInfo, bool)>();
        private Dictionary<Type, object> sharedInstances = new Dictionary<Type, object>();

        public Compositor(IEnumerable<Export> source, Compositor parent = null)
        {
            this.parent = parent;

            var eagar = new List<Type>();
            foreach (var export in source ?? throw new ArgumentNullException(nameof(source)))
            {
                if (!export.ContractType.IsAssignableFrom(export.ImplementationType))
                    throw new ArgumentException($"{export.ContractType} is not assignable from {export.ImplementationType}.");
                if (!export.LazyCreate)
                    if (!export.SingleInstance)
                        throw new ArgumentException("Transient object must be lazy.");
                    else
                        eagar.Add(export.ContractType);

                var ctor = export.ImplementationType.GetConstructors();
                ConstructorInfo candidate;
                if (ctor.Length == 1)
                    candidate = ctor[0];
                else
                {
                    var valid = ctor.Where(x => x.GetCustomAttribute<CompositionConstructorAttribute>() != null).ToArray();
                    if (valid.Length == 1)
                        candidate = valid[0];
                    else
                        throw new CompositionException($"Can't decide constructor for {export.ImplementationType}.");
                }

                impls.Add(export.ContractType, export.ImplementationType);
                creations[export.ImplementationType] = (candidate, export.SingleInstance);
            }

            foreach (var type in eagar)
                Resolve(type);
        }

        public void AttachInstance<T>(T instance) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            var impl = instance.GetType();
            impls.Add(typeof(T), impl);
            sharedInstances[impl] = instance;
        }

        public void Dispose()
        {
            var temp = sharedInstances;
            creations = null;
            sharedInstances = null;
            foreach (var obj in sharedInstances.Values)
                (obj as IDisposable)?.Dispose();
        }

        private object ResolveImpl(Type type, Stack<Type> dependencies)
        {
            if (sharedInstances == null)
                throw new ObjectDisposedException(nameof(Compositor));

            if (dependencies == null)
                dependencies = new Stack<Type>();
            else if (dependencies.Contains(type))
                throw new CompositionException("Circular dependency detected when resolving type "
                    + type + ". Route: " + string.Join(" -> ", dependencies));

            if (sharedInstances.TryGetValue(type, out object shared))
                return shared;

            dependencies.Push(type);

            if (impls.TryGetValue(type, out Type impl))
            {
                var (ctor, singleton) = creations[impl];
                var parameters = ctor.GetParameters();
                var p = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    p[i] = ResolveImpl(parameters[i].ParameterType, dependencies);
                    if (p[i] == null)
                        throw new CompositionException($"Can't resolve parameter {parameters[i].ParameterType} for {type}.");
                }

                var created = ctor.Invoke(p);
                if (singleton)
                    sharedInstances.Add(type, created);

                dependencies.Pop();
                return created;
            }

            var fromParent = parent?.ResolveImpl(type, dependencies);
            dependencies.Pop();
            return fromParent;
        }

        public object Resolve(Type type) => ResolveImpl(type, null);

        public T Resolve<T>() where T : class => (T)ResolveImpl(typeof(T), null);

        public static Compositor Default { get; private set; }
        public static void SetDefault(Compositor instance)
        {
            if (Default != null)
                throw new InvalidOperationException("Default compositor can be set only once.");
            Default = instance ?? throw new ArgumentNullException(nameof(instance));
        }
    }

    public class CompositionException : Exception
    {
        public CompositionException(string message) : base(message) { }
    }
}
