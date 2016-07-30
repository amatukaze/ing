using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Models;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.KanColle.Amatsukaze.Internal
{
    class OldPreference
    {
        [JsonProperty("language")]
        [OldPreferenceMapping("main.language")]
        public string Language { get; private set; }

        [JsonProperty("language_extra")]
        [OldPreferenceMapping("main.language_extra")]
        public string ExtraResourceLanguage { get; private set; }

        [JsonProperty("update")]
        public UpdatePreference Update { get; private set; }

        [JsonProperty("network")]
        public NetworkPreference Network { get; private set; }

        [JsonProperty("ui")]
        public UserInterfacePreference UI { get; private set; }

        [JsonProperty("game")]
        public GamePreference Game { get; private set; }

        [JsonProperty("cache")]
        public CachePreference Cache { get; private set; }

        [JsonProperty("browser")]
        public BrowserPreference Browser { get; private set; }

        [JsonProperty("notification")]
        public NotificationPreference Notification { get; private set; }

        [JsonProperty("windows")]
        public WindowsPreference Windows { get; private set; }

        [JsonProperty("layout")]
        public LayoutPreference Layout { get; private set; } = new LayoutPreference();

        [JsonProperty("other")]
        public OtherPreference Other { get; private set; } = new OtherPreference();

        public class UpdatePreference
        {
            [JsonProperty("notification")]
            [OldPreferenceMapping("update.notification")]
            public UpdateNotificationMode NotificationMode { get; private set; }

            [JsonProperty("channel")]
            [OldPreferenceMapping("update.channel")]
            public UpdateChannel UpdateChannel { get; private set; }
        }
        public class NetworkPreference
        {
            [JsonProperty("port")]
            [OldPreferenceMapping("network.port")]
            public int Port { get; private set; }

            [JsonProperty("allowremoterequests")]
            [OldPreferenceMapping("network.allow_remote_requests")]
            public bool AllowRequestsFromOtherDevices { get; private set; }

            [JsonProperty("upstreamproxy")]
            public UpstreamProxyPreference UpstreamProxy { get; private set; }
        }
        public class UpstreamProxyPreference
        {
            [JsonProperty("enabled")]
            [OldPreferenceMapping("network.upstream_proxy.enabled")]
            public bool Enabled { get; private set; }

            [JsonProperty("host")]
            [OldPreferenceMapping("network.upstream_proxy.host")]
            public string Host { get; private set; }

            [JsonProperty("port")]
            [OldPreferenceMapping("network.upstream_proxy.port")]
            public int Port { get; private set; }

            [JsonProperty("http_only")]
            [OldPreferenceMapping("network.upstream_proxy.http_only")]
            public bool HttpOnly { get; private set; }
        }
        public class UserInterfacePreference
        {
            [JsonProperty("font")]
            [OldPreferenceMapping("ui.font")]
            public string Font { get; private set; }

            [JsonProperty("zoom")]
            [OldPreferenceMapping("ui.zoom")]
            public double Zoom { get; private set; }

            [JsonProperty("hd_line")]
            public HeavyDamageLinePreference HeavyDamageLine { get; private set; }

            [JsonProperty("use_game_material_icons")]
            [OldPreferenceMapping("ui.use_game_material_icons")]
            public bool UseGameMaterialIcons { get; private set; }
        }
        public class HeavyDamageLinePreference
        {
            [JsonProperty("color")]
            [OldPreferenceMapping("ui.hd_line.color")]
            public HeavyDamageLineType Type { get; private set; }

            [JsonProperty("width")]
            [OldPreferenceMapping("ui.hd_line.width")]
            public int Width { get; private set; }
        }
        public class GamePreference
        {
            [JsonProperty("main_los_formula")]
            [OldPreferenceMapping("game.formula.los")]
            public FleetLoSFormula MainFleetLoSFormula { get; private set; }

            [JsonProperty("main_fp_formula")]
            [OldPreferenceMapping("game.formula.fighter_power")]
            public FleetFighterPowerFormula MainFighterPowerFormula { get; private set; }

            [JsonProperty("fatigue_ceiling")]
            [OldPreferenceMapping("game.fatigue_ceiling")]
            public int FatigueCeiling { get; private set; }

            [JsonProperty("show_battle_info")]
            [OldPreferenceMapping("game.sortie.show_battle_info")]
            public bool ShowBattleInfo { get; private set; }

            [JsonProperty("show_drop")]
            [OldPreferenceMapping("game.sortie.show_drop")]
            public bool ShowDrop { get; private set; }
        }
        public class CachePreference
        {
            [JsonProperty("mode")]
            [OldPreferenceMapping("cache.mode")]
            public CacheMode Mode { get; private set; }

            [JsonProperty("path")]
            [OldPreferenceMapping("cache.path")]
            public string Path { get; private set; }
        }
        public class BrowserPreference
        {
            [JsonProperty("zoom")]
            [OldPreferenceMapping("browser.zoom")]
            public double Zoom { get; private set; }

            [JsonProperty("homepage")]
            [OldPreferenceMapping("browser.homepage")]
            public string Homepage { get; private set; }

            [JsonProperty("layoutengine")]
            [OldPreferenceMapping("browser.layout_engine")]
            public string CurrentLayoutEngine { get; private set; }

            [JsonProperty("flash")]
            public FlashPreference Flash { get; private set; }

            [JsonProperty("screenshot")]
            public ScreenshotPreference Screenshot { get; private set; }

            [JsonProperty("action_bar_placement")]
            [OldPreferenceMapping("browser.action_bar.placement")]
            public bool ActionBarPlacement { get; private set; }

            [JsonProperty("action_bar_stick_to_browser")]
            [OldPreferenceMapping("browser.action_bar.stick_to_browser")]
            public bool ActionBarStickToBrowser { get; private set; }
        }
        public class FlashPreference
        {
            [JsonProperty("quality")]
            [OldPreferenceMapping("browser.flash.quality")]
            public FlashQuality Quality { get; private set; }

            [JsonProperty("rendermode")]
            [OldPreferenceMapping("browser.flash.render_mode")]
            public FlashRenderMode RenderMode { get; private set; }
        }
        public class ScreenshotPreference
        {
            [JsonProperty("destination")]
            [OldPreferenceMapping("browser.screenshot.path")]
            public string Destination { get; private set; }

            [JsonProperty("filenameformat")]
            [OldPreferenceMapping("browser.screenshot.filename_format")]
            public string FilenameFormat { get; private set; }
            [JsonProperty("imageformat")]
            [OldPreferenceMapping("browser.screenshot.image_format")]
            public ScreenshotImageFormat ImageFormat { get; private set; }
        }
        public class NotificationPreference
        {
            [JsonProperty("expedition")]
            [OldPreferenceMapping("notification.expedition")]
            public bool Expedition { get; private set; }

            [JsonProperty("repair")]
            [OldPreferenceMapping("notification.repair")]
            public bool Repair { get; private set; }

            [JsonProperty("construction")]
            [OldPreferenceMapping("notification.construction")]
            public bool Construction { get; private set; }

            [JsonProperty("heavily_damaged")]
            [OldPreferenceMapping("notification.heavy_damage")]
            public bool HeavilyDamagedWarning { get; private set; }

            [JsonProperty("recovery_from_fatigue")]
            [OldPreferenceMapping("notification.recovery_from_fatigue")]
            public bool RecoveryFromFatigue { get; private set; }

            [JsonProperty("anchorage_repair")]
            [OldPreferenceMapping("notification.anchorage_repair")]
            public bool AnchorageRepair { get; private set; }

            [JsonProperty("sound")]
            [OldPreferenceMapping("notification.sound")]
            public NotificationSound Sound { get; private set; }
            [JsonProperty("sound_filename")]
            [OldPreferenceMapping("notification.sound_filename")]
            public string SoundFilename { get; private set; }

            [JsonProperty("sound_hd")]
            [OldPreferenceMapping("notification.sound_hd")]
            public NotificationSound HeavilyDamagedWarningSound { get; private set; }
            [JsonProperty("sound_filename_hd")]
            [OldPreferenceMapping("notification.sound_filename_hd")]
            public string HeavilyDamagedWarningSoundFilename { get; private set; }
        }
        public class WindowsPreference
        {
            [JsonProperty("landscape")]
            [OldPreferenceMapping("main.windows")]
            public JToken LandscapeArray { get; private set; }
        }
        public class LayoutPreference : ModelBase
        {
            [JsonProperty("landscape")]
            [OldPreferenceMapping("ui.layout.lanscape")]
            public Dock LandscapeDock { get; private set; }

            [JsonProperty("portrait")]
            [OldPreferenceMapping("ui.layout.portrait")]
            public Dock PortraitDock { get; private set; }
        }
        public class OtherPreference
        {
            [JsonProperty("panic_key")]
            public PanicKeyPreference PanicKey { get; private set; }

            [JsonProperty("session_tool")]
            public SessionToolPreference SessionTool { get; private set; }
        }
        public class PanicKeyPreference
        {
            [JsonProperty("enabled")]
            [OldPreferenceMapping("other.panic_key.enabled")]
            public bool Enabled { get; private set; }

            [JsonProperty("modifier_keys")]
            [OldPreferenceMapping("other.panic_key.modifier_keys")]
            public int ModifierKeys { get; private set; }

            [JsonProperty("key")]
            [OldPreferenceMapping("other.panic_key.key")]
            public int Key { get; private set; }
        }
        public class SessionToolPreference
        {
            [JsonProperty("start_recording_on_app_startup")]
            [OldPreferenceMapping("other.session_tool.start_recording_on_app_startup")]
            public bool StartRecordingOnAppStartup { get; private set; }
        }
    }
}
