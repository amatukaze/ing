using System;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink
{
    public class ScreenshotRequest
    {
        public string Id { get; }

        TaskCompletionSource<string> _tcs;

        public Task<string> Task => _tcs.Task;

        public ScreenshotRequest()
        {
            Id = "ScreenshotRequest_" + DateTimeOffset.Now.Ticks.ToString();

            _tcs = new TaskCompletionSource<string>();
        }

        public void Complete(string dataUrl)
        {
            _tcs.SetResult(dataUrl);
        }
    }
}
