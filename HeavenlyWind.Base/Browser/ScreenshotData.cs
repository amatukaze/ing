namespace Sakuno.KanColle.Amatsukaze.Browser
{
    public class ScreenshotData
    {
        public int Width { get; }
        public int Height { get; }

        public byte[] BitmapData { get; set; }

        public ScreenshotData(int rpWidth, int rpHeight)
        {
            Width = rpWidth;
            Height = rpHeight;
        }
    }
}
