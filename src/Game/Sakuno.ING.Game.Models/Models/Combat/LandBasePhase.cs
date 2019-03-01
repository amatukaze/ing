namespace Sakuno.ING.Game.Models.Combat
{
    public class LandBasePhase : AerialPhase
    {
        public LandBasePhase(int index, MasterDataRoot masterData, Side ally, Side enemy, RawLandBaseAerialPhase raw)
            : base(index, masterData, ally, enemy, raw, false)
        {
            // add group and squadron
        }
    }
}
