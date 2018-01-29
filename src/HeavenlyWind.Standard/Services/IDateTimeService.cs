using System;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IDateTimeService : IBindable
    {
        DateTimeOffset Now { get; }
    }
}
