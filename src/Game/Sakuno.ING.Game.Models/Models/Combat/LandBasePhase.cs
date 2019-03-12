namespace Sakuno.ING.Game.Models.Combat
{
    public class LandBasePhase : AerialPhase
    {
        public LandBasePhase(int index, MasterDataRoot masterData, Side enemy, RawLandBaseAerialPhase raw)
            : base(index, masterData, null, enemy, raw)
        { }
    }
}
