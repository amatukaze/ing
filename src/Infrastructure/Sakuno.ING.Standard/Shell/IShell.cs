using System.Threading.Tasks;
using Sakuno.ING.IO;

namespace Sakuno.ING.Shell
{
    public interface IShell
    {
        void Run();
        void SwitchWindow(string windowId);

        ValueTask<IFileFacade> OpenFileAsync(params string[] extensions);
        ValueTask<IFolderFacade> PickFolderAsync();

        ValueTask ShowMessageAsync(string detail, string title);
    }
}
