namespace Sakuno.ING.Shell
{
    public interface IShell
    {
        event System.Action Exited;

        void Run();
        void SwitchWindow(string windowId);
        void ShowViewWithParameter<T>(string viewId, T parameter);
    }
}
