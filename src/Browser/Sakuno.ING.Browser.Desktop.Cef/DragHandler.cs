using System.Collections.Generic;
using CefSharp;
using CefSharp.Enums;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    using IBrowser = global::CefSharp.IBrowser;

    internal class DragHandler : IDragHandler
    {
        public bool OnDragEnter(IWebBrowser chromiumWebBrowser, IBrowser browser, IDragData dragData, DragOperationsMask mask) => true;
        public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<DraggableRegion> regions) { }
    }
}
