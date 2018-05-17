namespace Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle
{
    interface ISupportingFire
    {
        int SupportingFireType { get; set; }
        RawSupportingFirePhase SupportingFire { get; set; }
    }
}
