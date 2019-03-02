namespace Sakuno.ING.Game.Models.Combat
{
    public enum AttackType
    {
        Unknown,
        DayShelling,
        DayDoubleShelling,
        /// <summary>Main gun - Secondary gun</summary>
        DayCutInMS,
        /// <summary>Main gun - Radar</summary>
        DayCutInMR,
        /// <summary>Main gun - AP shell</summary>
        DayCutInMAp,
        /// <summary>Main gun - Main gun</summary>
        DayCutInMM,
        DayAerialCutIn,
        NelsonTouch,
        NagatoShoot,
        MutsuShoot,
        DayTorpedo,
        NightShelling,
        NightDoubleShelling,
        NightTorpedo,
        /// <summary>Main gun - Torpedo</summary>
        NightCutInMT,
        /// <summary>Torpedo - Torpedo</summary>
        NightCutInTT,
        /// <summary>Main gun - Main gun - Secondary gun</summary>
        NightCutInMMS,
        /// <summary>Main gun - Main gun - Main gun</summary>
        NightCutInMMM,
        NightAerialCutIn,
        /// <summary>Main gun - Torpedo - Radar</summary>
        NightDestroyerMTR,
        /// <summary>Torpedo - Radar - Personnel</summary>
        NightDestroyerTRP,
        AerialTorpedo,
        AerialBomb,
        AerialBoth,
        SupportShelling,
        SupportTorpedo,
        SupportAerial
    }
}
