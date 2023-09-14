using System;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Extensibility.Browser
{
    public interface IBrowserProvider
    {
        Task Initialize(bool disableHWA, int port);
        Task Shutdown();

        Task<IBrowser> CreateBrowserInstance();

        Task ClearCache();
        Task ClearCookie();
    }
}
