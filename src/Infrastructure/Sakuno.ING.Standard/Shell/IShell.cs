namespace Sakuno.ING.Shell
{
    public interface IShell
    {
        void Run();
        void SwitchWindow(string windowId);
        void ShowViewWithParameter<T>(string viewId, T parameter);
    }
}
