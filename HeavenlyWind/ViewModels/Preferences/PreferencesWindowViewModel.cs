using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.ViewModels.Plugins;
using Sakuno.SystemInterop.Dialogs;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Preferences
{
    public class PreferencesWindowViewModel : WindowViewModel
    {
        public static PreferencesWindowViewModel Instance { get; } = new PreferencesWindowViewModel();

        public IList<SystemFont> SystemFonts { get; }

        public IList<PluginViewModel> LoadedPlugins { get; }

        public bool IsAutoRotationSupported => CurrentDockExtension.IsAutoRotationSupported;

        public ICommand OpenFolderPickerCommand { get; }

        public ICommand OpenCustomSoundFileDialogCommand { get; }

        PreferencesWindowViewModel()
        {
            var rSystemFonts = Fonts.SystemFontFamilies.Select(r => new SystemFont(r)).ToList();
            var rCurrentFont = Preference.Instance.UI.Font;
            if (!rSystemFonts.Any(r => r.FontFamily.Source == rCurrentFont))
                rSystemFonts.Insert(0, new SystemFont(rCurrentFont));

            SystemFonts = rSystemFonts.AsReadOnly();

            LoadedPlugins = PluginService.Instance.LoadedPlugins.Select(r => new PluginViewModel(r)).ToList().AsReadOnly();

            OpenFolderPickerCommand = new DelegatedCommand<string>(rpType =>
            {
                using (var rFolderPicker = new CommonOpenFileDialog() { FolderPicker = true })
                {
                    if (rFolderPicker.Show() == CommonFileDialogResult.OK)
                    {
                        var rPath = rFolderPicker.Filename;

                        switch (rpType)
                        {
                            case "Cache":
                                Preference.Instance.Cache.Path.Value = rPath;
                                break;

                            case "Screenshot":
                                Preference.Instance.Browser.Screenshot.Path.Value = rPath;
                                break;
                        }
                    }
                }
            });

            OpenCustomSoundFileDialogCommand = new DelegatedCommand<string>(rpType =>
            {
                using (var rDialog = new CommonOpenFileDialog())
                {
                    rDialog.FileTypes.Add(new CommonFileDialogFileType(StringResources.Instance.Main.PreferenceWindow_Notification_SoundFileType, "wav;mp3;aac;wma"));

                    if (rDialog.Show() == CommonFileDialogResult.OK)
                    {
                        var rFilename = rDialog.Filename;

                        switch (rpType)
                        {
                            case "General":
                                Preference.Instance.Notification.SoundFilename.Value = rFilename;
                                break;

                            case "HeavyDamage":
                                Preference.Instance.Notification.HeavyDamageWarningSoundFilename.Value = rFilename;
                                break;
                        }
                    }
                }
            });
        }
    }
}
