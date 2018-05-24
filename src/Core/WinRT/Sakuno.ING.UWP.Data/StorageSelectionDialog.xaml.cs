using System;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace Sakuno.ING.UWP.Data
{
    internal sealed partial class StorageSelectionDialog : ContentDialog
    {
        private StorageFolder CustomFolder;
        public StorageFolder SelectedFolder =>
            AppDataRadioButton.IsChecked == true ?
            ApplicationData.Current.RoamingFolder :
            CustomFolder;

        public StorageSelectionDialog()
        {
            this.InitializeComponent();
            CustomTextBox.Text = ApplicationData.Current.RoamingFolder.Path;
        }

        private async void SelectCustomPath(object sender, TappedRoutedEventArgs e)
        {
            var picker = new FolderPicker
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };
            picker.FileTypeFilter.Add("*");
            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                CustomFolder = folder;
                CustomTextBox.Text = folder.Path;
            }
        }
    }
}
