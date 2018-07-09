using System.Threading.Tasks;
using Sakuno.ING.IO;

namespace Sakuno.ING.Shell
{
    public interface IShellContext
    {
        Task<IFileFacade> OpenFileAsync(params string[] extensions);
        Task<IFolderFacade> PickFolderAsync();

        Task ShowMessageAsync(string detail, string title);
    }
}
