namespace Sakuno.ING.Game;

public interface IApiMessageProvider
{
    IObservable<ApiMessage> ApiMessages { get; }
}
