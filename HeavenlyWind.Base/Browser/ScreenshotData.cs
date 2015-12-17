namespace Sakuno.KanColle.Amatsukaze.Browser
{
    public class ScreenshotData
    {
        public int Width { get; }
        public int Height { get; }
        public int BitCount { get; }

        public byte[] BitmapData { get; set; }

        public ScreenshotData(int rpWidth, int rpHeight, int rpBitCount)
        {
            Width = rpWidth;
            Height = rpHeight;
            BitCount = rpBitCount;
        }
    }
}
