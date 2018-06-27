using System.IO;
using System.Threading.Tasks;

namespace Sakuno.ING.Shell
{
    public interface IFileSystemPickerService
    {
        ValueTask<FileInfo> OpenFileAsync(params string[] extensions);
        ValueTask<DirectoryInfo> PickFolderAsync();
    }
}
