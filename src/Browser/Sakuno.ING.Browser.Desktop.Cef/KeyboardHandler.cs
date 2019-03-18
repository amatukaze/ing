using CefSharp;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    internal class KeyboardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, global::CefSharp.IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            if (type == KeyType.KeyUp && windowsKeyCode == 0x7B) //F12
            {
                browser.ShowDevTools();
                return true;
            }

            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, global::CefSharp.IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut) => false;
    }
}
