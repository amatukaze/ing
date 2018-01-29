using Sakuno.KanColle.Amatsukaze.Data.Localization;
using Sakuno.KanColle.Amatsukaze.Game.MasterData;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public static class LocalizationExtensions
    {
        public static string GetName(this ILocalizationService service, ShipInfo ship)
            => service.GetLocalized(LocalizationCategory.Ship, ship.Id.ToString());

        public static string GetName(this ILocalizationService service, ShipTypeInfo type)
            => service.GetLocalized(LocalizationCategory.ShipType, type.Id.ToString());

        public static string GetName(this ILocalizationService service, EquipmentInfo equipment)
            => service.GetLocalized(LocalizationCategory.Equipment, equipment.Id.ToString());

        public static string GetName(this ILocalizationService service, EquipmentTypeInfo type)
            => service.GetLocalized(LocalizationCategory.EquipmentType, type.Id.ToString());

        public static string GetString(this ILocalizationService service, AdmiralRank rank)
            => service.GetLocalized(LocalizationCategory.AdmiralRank, ((int)rank).ToString());

        public static string GetString(this ILocalizationService service, ShipSpeed speed)
            => service.GetLocalized(LocalizationCategory.ShipSpeed, ((int)speed).ToString());
    }
}
