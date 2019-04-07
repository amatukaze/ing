using System.Windows;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Views.Desktop.Documents
{
    public class ShipBasicNameText : TranslatableText
    {
        public static readonly DependencyProperty SourceProperty
            = DependencyProperty.Register(nameof(Source), typeof(ShipName), typeof(ShipBasicNameText),
                new PropertyMetadata((d, e) =>
                {
                    var c = (ShipBasicNameText)d;
                    var value = e.NewValue as ShipName;
                    if (Translate)
                        c.Text = value?.BasicTranslation;
                    else
                        c.Text = value?.BasicName;
                }));

        public ShipName Source
        {
            get => (ShipName)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
    }
}
