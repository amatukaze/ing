using CefSharp;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink.Handlers
{
    class KeyboardHandler : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            var key = KeyInterop.KeyFromVirtualKey(windowsKeyCode);

            if (type == KeyType.KeyUp && key is Key.F12)
            {
                browser.ShowDevTools();
                return true;
            }

            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser chromiumWebBrowser, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (type is KeyType.Char)
                return false;

            var key = KeyInterop.KeyFromVirtualKey(windowsKeyCode);

            return key is Key.Tab or Key.F10;
        }
    }
}
