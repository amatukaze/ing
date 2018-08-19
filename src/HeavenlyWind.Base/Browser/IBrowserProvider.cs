using System;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Browser
{
    public interface IBrowserProvider
    {
        void Initialize(object parameter);

        IBrowser CreateBrowserInstance();

        void SetPort(int port);

        void ClearCache();
        void ClearCookie();
    }
}
