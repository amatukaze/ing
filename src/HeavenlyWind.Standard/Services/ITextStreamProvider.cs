using System.Collections.Generic;
using System.IO;
using Sakuno.KanColle.Amatsukaze.Messaging;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface ITextStreamProvider : IProducer<KeyValuePair<string, Stream>>
    {
    }
}
