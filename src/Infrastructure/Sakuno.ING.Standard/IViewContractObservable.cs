namespace Sakuno.ING;

public interface IViewContractObservable
{
    IObservable<string?> ViewContractObservable { get; }
}
