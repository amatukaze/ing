using System.Security.Principal;
using System.Threading;

namespace Sakuno.SystemInterop
{
    public static class Security
    {
        public static bool IsAdministrator { get; private set; }

        static Security()
        {
            Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            var rPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;
            IsAdministrator = rPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
