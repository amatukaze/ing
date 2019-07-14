using System;
using System.Collections.Generic;
using Sakuno.ING.Composition;
using Sakuno.ING.Data;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Migrators;
using Sakuno.ING.Game.Logger.Migrators.INGLegacy;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Notification;
using Sakuno.ING.Http;
using Sakuno.ING.Localization;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Sakuno.ING.Timing;
using Sakuno.ING.Timing.NTP;
using Sakuno.ING.ViewModels.Combat;
using Sakuno.ING.ViewModels.Logging;
using Sakuno.ING.Views.UWP.ApiDebug;
using Sakuno.ING.Views.UWP.Catalog;
using Sakuno.ING.Views.UWP.Combat;
using Sakuno.ING.Views.UWP.Homeport;
using Sakuno.ING.Views.UWP.Logging;
using Sakuno.ING.Views.UWP.MasterData;
using Sakuno.ING.Views.UWP.Settings;

namespace Sakuno.ING.UWP
{
    internal class AotCompositor : Compositor
    {
        private static class Information<T>
        {
            public static T Static;
            public static Func<T> Factory;
        }

        public AotCompositor()
        {
            var ntp = Information<ITimingService>.Static = new NTPService();
            var data = Information<IDataService>.Static = new DataService();
            var settings = Information<ISettingsManager>.Static = new SettingsManager(data);
            var toastNotifier = new ToastNotifier();
            var browserSetting = Information<BrowserSetting>.Static = new BrowserSetting(settings);
            var layoutSetting = Information<LayoutSetting>.Static = new LayoutSetting(settings);
            var localeSetting = Information<LocaleSetting>.Static = new LocaleSetting(settings);
            var localization = Information<ILocalizationService>.Static = new LocalizationService(localeSetting);
            var shell = Information<IShell>.Static = new UWPShell(layoutSetting, localeSetting, localization);
            var shellContext = Information<IShellContextService>.Static = new UWPShellContextService(localization);
            var proxySetting = Information<ProxySetting>.Static = new ProxySetting(settings);
            var selector = new UWPHttpProviderSelector(shellContext, browserSetting, proxySetting);
            Information<IHttpProviderSelector>.Static = selector;
            var gameProvider = Information<GameProvider>.Static = new GameProvider(selector, settings);
            var notificationManager = Information<NotificationManager>.Static = new NotificationManager(new[] { toastNotifier }, settings, localization, shell, localeSetting);
            var navalBase = Information<NavalBase>.Static = new NavalBase(gameProvider, ntp, notificationManager);
            var logger = Information<Logger>.Static = new Logger(data, gameProvider, navalBase);
            var currentBattleVM = Information<CurrentBattleVM>.Static = new CurrentBattleVM(navalBase, shell);

            var migrators = new LogMigrator[]
            {
                Information<AdmiralRoomMigrator>.Static = new AdmiralRoomMigrator(),
                Information<ElectronicObserverMigrator>.Static = new ElectronicObserverMigrator(),
                Information<INGLegacyMigrator>.Static = new INGLegacyMigrator(),
                Information<LogbookMigrator>.Static = new LogbookMigrator(),
                Information<PoiMigrator>.Static = new PoiMigrator(),
            };

            Information<LogMigrationVM>.Factory = () => new LogMigrationVM(logger, migrators, shellContext, localization);
            Information<BattleLogsVM>.Factory = () => new BattleLogsVM(logger, navalBase, gameProvider, localization, shell);
            Information<EquipmentCreationLogsVM>.Factory = () => new EquipmentCreationLogsVM(logger, navalBase, localization);
            Information<ExpeditionCompletionLogsVM>.Factory = () => new ExpeditionCompletionLogsVM(logger, navalBase, localization);
            Information<ShipCreationLogsVM>.Factory = () => new ShipCreationLogsVM(logger, navalBase, localization);

            Information<ApiDebugView>.Factory = () => new ApiDebugView(gameProvider);
            Information<CurrentBattleView>.Factory = () => new CurrentBattleView(currentBattleVM);
            Information<AdmiralView>.Factory = () => new AdmiralView(navalBase);
            Information<DocksView>.Factory = () => new DocksView(navalBase);
            Information<ExpeditionView>.Factory = () => new ExpeditionView(navalBase);
            Information<FleetsView>.Factory = () => new FleetsView(navalBase);
            Information<LogMigrationView>.Factory = () => new LogMigrationView(Information<LogMigrationVM>.Factory());
            Information<BattleLogsView>.Factory = () => new BattleLogsView(Information<BattleLogsVM>.Factory());
            Information<EquipmentCreationLogsView>.Factory = () => new EquipmentCreationLogsView(Information<EquipmentCreationLogsVM>.Factory());
            Information<ExpeditionCompletionLogsView>.Factory = () => new ExpeditionCompletionLogsView(Information<ExpeditionCompletionLogsVM>.Factory());
            Information<ShipCreationLogsView>.Factory = () => new ShipCreationLogsView(Information<ShipCreationLogsVM>.Factory());
            Information<MasterDataView>.Factory = () => new MasterDataView(navalBase);
            Information<BrowserElement>.Factory = () => new BrowserElement(selector, layoutSetting);
            Information<ActiveQuestsView>.Factory = () => new ActiveQuestsView(navalBase);
            Information<MapHPView>.Factory = () => new MapHPView(navalBase);
            Information<ShipCatalogView>.Factory = () => new ShipCatalogView(navalBase);

            Information<LocaleSettingView>.Factory = () => new LocaleSettingView(localeSetting);
            Information<ProxySettingView>.Factory = () => new ProxySettingView(proxySetting);
#if DEBUG
            Information<DebugSettingView>.Factory = () => new DebugSettingView(selector);
#endif
            Information<NotificationSettingView>.Factory = () => new NotificationSettingView(notificationManager);
        }

        public override IReadOnlyDictionary<string, Type> ViewTypes { get; } = new Dictionary<string, Type>
        {
            ["ApiDebug"] = typeof(ApiDebugView),
            ["CurrentBattle"] = typeof(CurrentBattleView),
            ["Admiral"] = typeof(AdmiralView),
            ["Docks"] = typeof(DocksView),
            ["Expedition"] = typeof(ExpeditionView),
            ["Fleets"] = typeof(FleetsView),
            ["Browser"] = typeof(BrowserElement),
            ["LogMigration"] = typeof(LogMigrationView),
            ["ShipCreationLogs"] = typeof(ShipCreationLogsView),
            ["EquipmentCreationLogs"] = typeof(EquipmentCreationLogsView),
            ["ExpeditionCompletionLogs"] = typeof(ExpeditionCompletionLogsView),
            ["BattleLogs"] = typeof(BattleLogsView),
            ["BattleLogDetail"] = typeof(BattleLogDetailsView),
            ["ActiveQuests"] = typeof(ActiveQuestsView),
            ["CurrentBattleDetail"] = typeof(BattleDetailView),
            ["LandBaseDefenceDetail"] = typeof(BattleOverview),
            ["MapHP"] = typeof(MapHPView),
            ["ShipCatalog"] = typeof(ShipCatalogView)
        };

        public override IReadOnlyCollection<KeyValuePair<Type, SettingCategory>> SettingViews { get; } = new Dictionary<Type, SettingCategory>
        {
            [typeof(LocaleSettingView)] = SettingCategory.Appearance,
            [typeof(ProxySettingView)] = SettingCategory.Network,
#if DEBUG
            [typeof(DebugSettingView)] = SettingCategory.Browser,
#endif
            [typeof(NotificationSettingView)] = SettingCategory.Notification
        };

        public override IEnumerable<Type> GetSettingViewsForCategory(SettingCategory category) => category switch
        {
            SettingCategory.Appearance => new[] { typeof(LocaleSettingView) },
            SettingCategory.Network => new[] { typeof(ProxySettingView) },
#if DEBUG
            SettingCategory.Browser => new[] { typeof(DebugSettingView) },
#endif
            SettingCategory.Notification => new[] { typeof(NotificationSettingView) },
            _ => Array.Empty<Type>()
        };

        public override T Resolve<T>() => Information<T>.Static ?? Information<T>.Factory?.Invoke();
        public override object Resolve(Type type)
        {
            // only views should follow this path
            if (type == typeof(ApiDebugView))
                return Information<ApiDebugView>.Factory();
            else if (type == typeof(CurrentBattleView))
                return Information<CurrentBattleView>.Factory();
            else if (type == typeof(AdmiralView))
                return Information<AdmiralView>.Factory();
            else if (type == typeof(DocksView))
                return Information<DocksView>.Factory();
            else if (type == typeof(ExpeditionView))
                return Information<ExpeditionView>.Factory();
            else if (type == typeof(FleetsView))
                return Information<FleetsView>.Factory();
            else if (type == typeof(BrowserElement))
                return Information<BrowserElement>.Factory();
            else if (type == typeof(LogMigrationView))
                return Information<LogMigrationView>.Factory();
            else if (type == typeof(ShipCreationLogsView))
                return Information<ShipCreationLogsView>.Factory();
            else if (type == typeof(EquipmentCreationLogsView))
                return Information<EquipmentCreationLogsView>.Factory();
            else if (type == typeof(ExpeditionCompletionLogsView))
                return Information<ExpeditionCompletionLogsView>.Factory();
            else if (type == typeof(BattleLogsView))
                return Information<BattleLogsView>.Factory();
            else if (type == typeof(LocaleSettingView))
                return Information<LocaleSettingView>.Factory();
            else if (type == typeof(ProxySettingView))
                return Information<ProxySettingView>.Factory();
#if DEBUG
            else if (type == typeof(DebugSettingView))
                return Information<DebugSettingView>.Factory();
#endif
            else if (type == typeof(NotificationSettingView))
                return Information<NotificationSettingView>.Factory();
            else if (type == typeof(ActiveQuestsView))
                return Information<ActiveQuestsView>.Factory();
            else if (type == typeof(MapHPView))
                return Information<MapHPView>.Factory();
            else if (type == typeof(ShipCatalogView))
                return Information<ShipCatalogView>.Factory();
            else return null;
        }

        public override object ResolveWithParameter<TParam>(Type type, TParam parameter)
        {
            if (type == typeof(BattleLogDetailsView) && typeof(TParam) == typeof(BattleVM))
                return new BattleLogDetailsView((BattleVM)(object)parameter);
            else if (type == typeof(BattleDetailView) && typeof(BattleBase).IsAssignableFrom(typeof(TParam)))
                return new BattleDetailView((BattleBase)(object)parameter);
            else if (type == typeof(BattleOverview) && typeof(BattleBase).IsAssignableFrom(typeof(TParam)))
                return new BattleOverview((BattleBase)(object)parameter);
            else
                throw new InvalidOperationException("Compositor out of date");
        }

        public override object TryResolveView(string viewId) => viewId switch
        {
            "ApiDebug" => (object)Information<ApiDebugView>.Factory(),
            "CurrentBattle" => Information<CurrentBattleView>.Factory(),
            "Admiral" => Information<AdmiralView>.Factory(),
            "Docks" => Information<DocksView>.Factory(),
            "Expedition" => Information<ExpeditionView>.Factory(),
            "Fleets" => Information<FleetsView>.Factory(),
            "Browser" => Information<BrowserElement>.Factory(),
            "LogMigration" => Information<LogMigrationView>.Factory(),
            "ShipCreationLogs" => Information<ShipCreationLogsView>.Factory(),
            "EquipmentCreationLogs" => Information<EquipmentCreationLogsView>.Factory(),
            "ExpeditionCompletionLogs" => Information<ExpeditionCompletionLogsView>.Factory(),
            "BattleLogs" => Information<BattleLogsView>.Factory(),
            "ActiveQuests" => Information<ActiveQuestsView>.Factory(),
            "MapHP" => Information<MapHPView>.Factory(),
            "ShipCatalog" => Information<ShipCatalogView>.Factory(),
            _ => null
        };
    }
}
