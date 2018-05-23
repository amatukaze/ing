using System;

namespace Sakuno.ING.Services
{
    public interface IDateTimeService : IBindable
    {
        DateTimeOffset Now { get; }
    }
}
