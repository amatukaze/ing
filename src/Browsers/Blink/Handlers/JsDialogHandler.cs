using CefSharp;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink.Handlers;

internal class JsDialogHandler : IJsDialogHandler
{
    public bool OnJSDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
    {
        if (messageText is "エラーが発生したため、ページ更新します。")
            suppressMessage = true;

        return false;
    }

    public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload, IJsDialogCallback callback)
    {
        return false;
    }

    public void OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser)
    {
    }

    public void OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser)
    {
    }
}
