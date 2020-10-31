using System;

namespace Sakuno.ING.Messaging
{
    public interface IApiMessageSource
    {
        IObservable<ApiMessage> ApiMessageSource { get; }
    }
}
