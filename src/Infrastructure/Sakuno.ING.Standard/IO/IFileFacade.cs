using System.IO;
using System.Threading.Tasks;

namespace Sakuno.ING.IO
{
    public interface IFileFacade : IFileSystemFacade
    {
        ValueTask<Stream> OpenReadAsync();
        ValueTask<string> GetAccessPathAsync();
    }
}
