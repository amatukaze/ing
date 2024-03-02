using CefSharp;
using CefSharp.Enums;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink.Handlers
{
    class DragHandler : IDragHandler
    {
        public bool OnDragEnter(IWebBrowser chromiumWebBrowser, global::CefSharp.IBrowser browser, IDragData dragData, DragOperationsMask mask)
        {
            return true;
        }

        public void OnDraggableRegionsChanged(IWebBrowser chromiumWebBrowser, CefSharp.IBrowser browser, IFrame frame, IList<DraggableRegion> regions)
        {
        }
    }
}
