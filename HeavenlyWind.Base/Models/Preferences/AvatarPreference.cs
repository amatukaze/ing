using System.Globalization;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class AvatarPreference : ModelBase
    {
        public Property<bool> EnabledInFleetDetail { get; }
        public Property<bool> EnabledInBattleInfo { get; }
        public Property<bool> EnabledInShipOverview { get; }
        public Property<bool> EnabledInEquipmentOverview { get; }

        public Property<AvatarShape> Shape { get; } = new Property<AvatarShape>("ui.avatar.shape", AvatarShape.RoundedSquare);

        public AvatarPreference()
        {
            var rCultures = StringResources.GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture);
            var rEastAsianCultures = new[] { "ja", "zh-Hans", "zh-Hant", "ko" };
            var rIsEastAsianCulture = !rCultures.Intersect(rEastAsianCultures).Any();

            EnabledInFleetDetail = new Property<bool>("ui.avatar.enabled.fleet_detail", rIsEastAsianCulture);
            EnabledInBattleInfo = new Property<bool>("ui.avatar.enabled.battle_info", rIsEastAsianCulture);
            EnabledInShipOverview = new Property<bool>("ui.avatar.enabled.ship_overview", rIsEastAsianCulture);
            EnabledInEquipmentOverview = new Property<bool>("ui.avatar.enabled.equipment_overview", rIsEastAsianCulture);
        }
    }
}
