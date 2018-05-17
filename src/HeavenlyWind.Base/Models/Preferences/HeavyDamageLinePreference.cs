namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class HeavyDamageLinePreference : ModelBase
    {
        public Property<HeavyDamageLineType> Type { get; } = new Property<HeavyDamageLineType>("ui.hd_line.color", HeavyDamageLineType.Default);

        public Property<int> Width { get; } = new HeavyDamageLineWidthProperty();
    }
}
