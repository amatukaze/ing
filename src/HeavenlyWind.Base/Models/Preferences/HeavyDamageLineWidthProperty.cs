namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    class HeavyDamageLineWidthProperty : Property<int>
    {
        public HeavyDamageLineWidthProperty() : base("ui.hd_line.width", 3) { }

        public override void SetDirectly(int rpValue) => base.SetDirectly(rpValue.Clamp(0, 22));
    }
}
