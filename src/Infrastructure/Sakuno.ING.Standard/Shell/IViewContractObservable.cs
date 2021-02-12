using System;

namespace Sakuno.ING.Shell
{
    public interface IViewContractObservable
    {
        IObservable<string?> ViewContractObservable { get; }
    }
}
