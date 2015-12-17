namespace Sakuno.KanColle.Amatsukaze.Browser
{
    public interface IBrowserProvider
    {
        IBrowser CreateBrowserInstance();

        void SetPort(int rpPort);
        void ClearCache(bool rpClearCookie);
    }
}
