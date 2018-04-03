using Sakuno.KanColle.Amatsukaze.Messaging;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface ITextStreamProvider : IProducer<TextMessage>
    {
        bool Enabled { get; set; }
    }
}
