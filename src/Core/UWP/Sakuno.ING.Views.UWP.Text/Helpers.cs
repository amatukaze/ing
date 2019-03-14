using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Views.UWP
{
    public static class Helpers
    {
        public static bool IsNotNull(object obj) => obj != null;
        public static bool Not(bool value) => !value;

        private static readonly bool translate
            = Compositor.Static<LocaleSetting>().TranslateContent.InitialValue;
        public static string SelectName(TextTranslationGroup name)
            => translate ? name.Translation : name.Origin;
        public static string SelectShipName(ShipName name)
            => SelectName(name);

        static Helpers()
        {
            var localization = Compositor.Static<ILocalizationService>();
            admiralRankTexts = Enumerable.Range(1, 10)
                .Select(i => localization.GetLocalized("GameModel", "AdmiralRank_" + i))
                .ToArray();
            fireRangeTexts = Enumerable.Range(0, 5)
                .Select(i => localization.GetLocalized("GameModel", "FireRange_" + i))
                .ToArray();
            shipSpeedTexts = Enumerable.Range(0, 5)
                .Select(i => localization.GetLocalized("GameModel", "ShipSpeed_" + i * 5))
                .ToArray();
            dockEmpty = localization.GetLocalized("GameModel", "Dock_Empty");
            dockLocked = localization.GetLocalized("GameModel", "Dock_Locked");
        }

        private static readonly string[] admiralRankTexts;
        public static string FormatAdmiralRank(AdmiralRank rank) => rank switch
        {
            AdmiralRank.MarshalAdmiral => admiralRankTexts[0],
            AdmiralRank.Admiral => admiralRankTexts[1],
            AdmiralRank.ViceAdmiral => admiralRankTexts[2],
            AdmiralRank.RearAdmiral => admiralRankTexts[3],
            AdmiralRank.Captain => admiralRankTexts[4],
            AdmiralRank.Commander => admiralRankTexts[5],
            AdmiralRank.NoviceCommander => admiralRankTexts[6],
            AdmiralRank.LieutenantCommander => admiralRankTexts[7],
            AdmiralRank.ViceLieutenantCommander => admiralRankTexts[8],
            AdmiralRank.NoviceLieutenantCommander => admiralRankTexts[9],
            _ => null
        };

        private static readonly string[] fireRangeTexts;
        public static string FormatFireRange(FireRange range) => range switch
        {
            FireRange.None => fireRangeTexts[0],
            FireRange.Short => fireRangeTexts[1],
            FireRange.Medium => fireRangeTexts[2],
            FireRange.Long => fireRangeTexts[3],
            FireRange.VeryLong => fireRangeTexts[4],
            _ => null
        };

        private static readonly string[] shipSpeedTexts;
        public static string FormatShipSpeed(ShipSpeed speed) => speed switch
        {
            ShipSpeed.None => shipSpeedTexts[0],
            ShipSpeed.Slow => shipSpeedTexts[1],
            ShipSpeed.Fast => shipSpeedTexts[2],
            ShipSpeed.FastPlus => shipSpeedTexts[3],
            ShipSpeed.UltraFast => shipSpeedTexts[4],
            _ => null
        };

        private static readonly string dockEmpty, dockLocked;
        public static string FormatBuildingDockState(BuildingDockState state) => state switch
        {
            BuildingDockState.Empty => dockEmpty,
            BuildingDockState.Locked => dockLocked,
            _ => null
        };
        public static string FormatRepairingDockState(RepairingDockState state) => state switch
        {
            RepairingDockState.Empty => dockEmpty,
            RepairingDockState.Locked => dockLocked,
            _ => null
        };

        public static bool FleetStateEquals(FleetState left, FleetState right)
            => left == right;
    }
}
