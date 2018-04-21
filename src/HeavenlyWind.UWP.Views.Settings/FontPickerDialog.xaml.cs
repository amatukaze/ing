using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.Settings
{
    internal sealed partial class FontPickerDialog : ContentDialog
    {
        private readonly FontSelection[] Source;
        public FontSelection SelectedFont { get; private set; }

        public FontPickerDialog(IEnumerable<FontSelection> source)
        {
            Source = source.ToArray();
            this.InitializeComponent();
        }
    }
}
