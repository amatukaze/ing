namespace Sakuno.ING.Game.Models.Combat
{
    public class LandBaseDefenceBattle : BattleBase
    {
        public bool MaterialsLost { get; }
        public bool PlanesLost { get; }

        public LandBaseDefenceBattle(MasterDataRoot masterData, RawBattle raw)
            : base(new Side(raw.Ally))
        {
            Enemy = new Side(masterData, raw.Enemy, true);

            if (raw.LandBaseDefencePhase != null)
                phases.Add(new AerialPhase(1, masterData, Ally, Enemy, raw.LandBaseDefencePhase));

            Ally.UpdateDamageRate();
            Enemy.UpdateDamageRate();

            MaterialsLost = raw.LandBaseMaterialsLost;
            PlanesLost = raw.LandBasePlanesLost;
        }
    }
}
