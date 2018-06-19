using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.ING.Composition
{
    public class Compositor : IDisposable
    {
        private readonly Compositor parent;
        private Dictionary<object, Type> impls = new Dictionary<object, Type>();
        private Dictionary<Type, (ConstructorInfo creation, bool singleton)> creations = new Dictionary<Type, (ConstructorInfo, bool)>();
        private Dictionary<object, object> sharedInstances = new Dictionary<object, object>();

        public Compositor(IEnumerable<Export> source, Compositor parent = null)
        {
            this.parent = parent;

            var eagar = new List<object>();
            foreach (var export in source ?? throw new ArgumentNullException(nameof(source)))
            {
                if (!export.LazyCreate)
                    if (!export.SingleInstance)
                        throw new ArgumentException("Transient object must be lazy.");
                    else
                        eagar.Add(export.Contract);

                var ctor = export.Implementation.GetConstructors();
                ConstructorInfo candidate;
                if (ctor.Length == 1)
                    candidate = ctor[0];
                else
                {
                    var valid = ctor.Where(x => x.GetCustomAttribute<CompositionConstructorAttribute>() != null).ToArray();
                    if (valid.Length == 1)
                        candidate = valid[0];
                    else
                        throw new CompositionException($"Can't decide constructor for {export.Implementation}.");
                }

                impls.Add(export.Contract, export.Implementation);
                creations[export.Implementation] = (candidate, export.SingleInstance);
            }
            sharedInstances.Add(typeof(Compositor), this);

            foreach (var type in eagar)
                Resolve(type);
        }

        public void AttachInstance<T>(T instance) where T : class
        {
            sharedInstances[typeof(T)] = instance ?? throw new ArgumentNullException(nameof(instance));
        }

        public void Dispose()
        {
            var temp = sharedInstances;
            creations = null;
            sharedInstances = null;
            foreach (var obj in sharedInstances.Values)
                (obj as IDisposable)?.Dispose();
        }

        private object ResolveImpl(object contract, Stack<Type> dependencies)
        {
            if (sharedInstances == null)
                throw new ObjectDisposedException(nameof(Compositor));

            if (dependencies?.Contains(contract) == true)
                throw new CompositionException("Circular dependency detected when resolving type "
                    + contract + ". Route: " + string.Join(" -> ", dependencies));

            if (sharedInstances.TryGetValue(contract, out object attatched))
                return attatched;

            if (impls.TryGetValue(contract, out Type impl))
            {
                if (sharedInstances.TryGetValue(impl, out object shared))
                    return shared;

                if (dependencies == null)
                    dependencies = new Stack<Type>();
                dependencies.Push(impl);

                var (ctor, singleton) = creations[impl];
                var parameters = ctor.GetParameters();
                var p = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    p[i] = ResolveImpl(parameters[i].ParameterType, dependencies);
                    if (p[i] == null)
                        throw new CompositionException($"Can't resolve parameter {parameters[i].ParameterType} for {impl}.");
                }

                var created = ctor.Invoke(p);
                if (singleton)
                    sharedInstances.Add(impl, created);

                dependencies.Pop();
                return created;
            }

            var fromParent = parent?.ResolveImpl(contract, dependencies);
            return fromParent;
        }

        public object Resolve(object contract) => ResolveImpl(contract, null);

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
