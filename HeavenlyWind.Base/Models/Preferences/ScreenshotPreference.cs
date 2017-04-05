namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class ScreenshotPreference : ModelBase
    {
        public Property<string> Path { get; } = new Property<string>("browser.screenshot.path", "Screenshots");
        public Property<string> FilenameFormat { get; } = new Property<string>("browser.screenshot.filename_format", "ing_{0:yyyyMMdd-HHmmss-ff}.{1}");
        public Property<ScreenshotImageFormat> ImageFormat { get; } = new Property<ScreenshotImageFormat>("browser.screenshot.image_format", ScreenshotImageFormat.Png);
    }
}
