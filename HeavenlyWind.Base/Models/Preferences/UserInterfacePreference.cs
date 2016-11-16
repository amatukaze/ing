using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Models.Preferences
{
    public class UserInterfacePreference
    {
        public Property<string> Font { get; } = new UIFontProperty();

        public Property<ConfirmationMode> CloseConfirmationMode { get; } = new Property<ConfirmationMode>("ui.close_confirmation", ConfirmationMode.DuringSortie);

        public Property<Dock> LandscapeDock { get; } = new Property<Dock>("ui.layout.lanscape", Dock.Left);
        public Property<Dock> PortraitDock { get; } = new Property<Dock>("ui.layout.portrait", Dock.Top);

        public Property<bool> LockTabs { get; } = new Property<bool>("ui.layout.lock_tabs", false);

        public Property<double> Zoom { get; } = new Property<double>("ui.zoom", 1.0);

        public HeavyDamageLinePreference HeavyDamageLine { get; } = new HeavyDamageLinePreference();

        public Property<bool> UseGameMaterialIcons { get; } = new Property<bool>("ui.use_game_material_icons");

        public Property<bool> ShowFatigueInSortie { get; } = new Property<bool>("ui.show_fatigue_in_sortie");

        public Property<bool> UseGameIconsInModernizationMessage { get; } = new Property<bool>("ui.modernization_message_use_game_icons");
        public Property<bool> ShowStatusGrowthInModernizationMessage { get; } = new Property<bool>("ui.modernization_message_show_status_growth");

        public Property<StatusBarSortieInfoPlacement> StatusBarSortieInfo { get; } = new Property<StatusBarSortieInfoPlacement>("ui.startusbar.sortie_info_placement", StatusBarSortieInfoPlacement.Right);
    }
}
