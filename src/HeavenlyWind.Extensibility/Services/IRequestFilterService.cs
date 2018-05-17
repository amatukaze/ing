using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Extensibility.Services
{
    public interface IRequestFilterService
    {
        void Register(Func<string, IDictionary<string, string>, bool> filter);
    }
}
