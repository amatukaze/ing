namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class FlashPreference
    {
        public Property<FlashQuality> Quality { get; } = new Property<FlashQuality>("browser.flash.quality", FlashQuality.Default);

        public Property<FlashRenderMode> RenderMode { get; } = new Property<FlashRenderMode>("browser.flash.render_mode", FlashRenderMode.Default);
    }
}
