using Sakuno.ING.Messaging;

namespace Sakuno.ING.Services
{
    public interface ITextStreamProvider : IProducer<TextMessage>
    {
        bool Enabled { get; set; }
    }
}
