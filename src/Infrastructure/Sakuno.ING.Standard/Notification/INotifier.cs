using System;

namespace Sakuno.ING.Notification
{
    public interface INotifier : IIdentifiable<string>
    {
        bool IsSupported { get; }
        void Initialize();
        void Deinitialize();
        void AddSchedule(string id, string title, string content, DateTimeOffset time);
        void RemoveSchedule(string id);
    }
}
