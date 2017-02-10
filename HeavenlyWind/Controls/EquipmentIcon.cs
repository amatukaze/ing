using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class EquipmentIcon : Control
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(EquipmentIconType), typeof(EquipmentIcon),
            new UIPropertyMetadata(EquipmentIconType.None, (s, e) => ((EquipmentIcon)s).UpdateContent((EquipmentIconType)e.NewValue)));
        public EquipmentIconType Type
        {
            get { return (EquipmentIconType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        static ControlTemplate r_DefaultTemplate;
        static Dictionary<int, ControlTemplate> r_Templates;

        FrameworkElement r_Content;

        static EquipmentIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EquipmentIcon), new FrameworkPropertyMetadata(typeof(EquipmentIcon)));

            CreateDefaultTemplate();

            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                LoadTemplates();
        }
        static void CreateDefaultTemplate()
        {
            var rTextBlock = new FrameworkElementFactory(typeof(TextBlock));
            rTextBlock.SetValue(TextBlock.TextProperty, "?");
            rTextBlock.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
            rTextBlock.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            rTextBlock.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Ideal);

            var rViewBox = new FrameworkElementFactory(typeof(Viewbox));
            rViewBox.AppendChild(rTextBlock);

            r_DefaultTemplate = new ControlTemplate() { VisualTree = rViewBox };
        }
        static void LoadTemplates()
        {
            byte[] rContent;
            if (!DataStore.TryGet("equipment_icon", out rContent))
                r_Templates = new Dictionary<int, ControlTemplate>();
            else
            {
                var rReader = new JsonTextReader(new StreamReader(new MemoryStream(rContent)));
                var rData = JObject.Load(rReader);

                var rSharedResources = new ResourceDictionary();

                foreach (var rResource in ((JObject)rData["shared"]).Properties())
                    rSharedResources.Add(rResource.Name, XamlReader.Parse((string)rResource.Value));

                r_Templates = ((JObject)rData["type"]).Properties().ToDictionary(
                    r => int.Parse(r.Name),
                    r =>
                    {
                        var rXaml = "<ControlTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">" + (string)r.Value + "</ControlTemplate>";
                        var rResult = (ControlTemplate)XamlReader.Parse(rXaml);
                        rResult.Resources = rSharedResources;

                        return rResult;
                    });
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            r_Content = Template.FindName("PART_Content", this) as FrameworkElement;

            UpdateContent(Type);
        }

        void UpdateContent(EquipmentIconType rpType)
        {
            if (r_Content == null || r_Templates == null)
                return;

            ControlTemplate rTemplate;
            if (!r_Templates.TryGetValue((int)rpType, out rTemplate))
                rTemplate = r_DefaultTemplate;

            r_Content.SetCurrentValue(TemplateProperty, rTemplate);
        }
    }
}
