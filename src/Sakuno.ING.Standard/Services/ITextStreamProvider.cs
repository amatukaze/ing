using Sakuno.ING.Messaging;

namespace Sakuno.ING.Services
{
    public interface ITextStreamProvider : ITimedMessageProvider<TextMessage>
    {
        bool Enabled { get; set; }
    }
}
