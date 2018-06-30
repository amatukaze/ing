using System.IO;
using System.Threading.Tasks;

namespace Sakuno.ING.Shell
{
    public interface IShell
    {
        void Run();
        void SwitchWindow(string windowId);

        ValueTask<FileInfo> OpenFileAsync(params string[] extensions);
        ValueTask<DirectoryInfo> PickFolderAsync();

        ValueTask ShowMessageAsync(string detail, string title);
    }
}
