namespace Sakuno.ING.Notification
{
    public interface INotifier : IIdentifiable<string>
    {
        bool IsSupported { get; }
        void Initialize();
        void Show(string title, string content, string sound);
    }
}
