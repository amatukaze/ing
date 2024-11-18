using System;
using Common;
using Sakuno.ING.Composition;

namespace Common
{
    public interface ISingleton
    {
        void Report();
    }

    public interface IService
    {
        void Report();
    }
}

namespace NamespaceA
{
    [Export]
    public class Singleton
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Report() => Console.WriteLine($"Singleton - {Id}");
    }

    [Export(typeof(ISingleton))]
    internal class SingletonImpl : ISingleton
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Report() => Console.WriteLine($"SingletonImpl - {Id}");
    }

    [Export(typeof(IService), Singleton = false)]
    public class ServiceA : IService
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Report() => Console.WriteLine($"ServiceA - {Id}");
    }
}

namespace NamespaceB
{
    [Export(typeof(IService), Singleton = false)]
    public class ServiceB : IService
    {
        public Guid Id { get; } = Guid.NewGuid();

        public void Report() => Console.WriteLine($"ServiceB - {Id}");
    }
}
