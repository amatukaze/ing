using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Shell;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.KanColle.Amatsukaze.ViewModels.Layout;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    internal class Shell : IShell
    {
        public async void Run()
        {
            started = true;

            StorageFile layoutFile = null;
            try
            {
                layoutFile = await ApplicationData.Current.RoamingFolder.GetFileAsync("layout.xml");
            }
            catch (FileNotFoundException)
            {
                // load default
            }

            if (layoutFile != null)
            {
                XmlDocument xml = new XmlDocument();
                using (var stream = await layoutFile.OpenStreamForReadAsync())
                    xml.Load(stream);
                Layout = new LayoutRoot().FromXml(xml);
            }
            else
            {
                Layout = new LayoutRoot();
                Layout.Entries.Add(new RelativeLayout());
                // load browser only
            }

            Window.Current.Content = main = new MainPage(new MainWindowVM(), this);
            Rearrange();
            Window.Current.Activate();
        }

        internal readonly Dictionary<string, ViewDescriptor> Views = new Dictionary<string, ViewDescriptor>();
        internal LayoutRoot Layout;
        private bool started;
        private MainPage main;

        public void RegisterView(ViewDescriptor descriptor)
        {
            if (started) throw new InvalidOperationException("Shell already started.");
            Views.Add(descriptor.Id, descriptor);
        }

        internal void Rearrange()
        {
            foreach (var view in CoreApplication.Views)
                if (!view.IsMain)
                    view.CoreWindow.Close();

            main.MainContent.Content = null;
            main.ViewSwitcher.Children.Clear();
            foreach (var entry in Layout.Entries)
            {
                if (main.MainContent.Content == null)
                    main.MainContent.Content = BuildLayout(entry);
                else
                {
                    var button = new Button { Content = entry.Title, Tag = entry };
                    button.Tapped += async (sender, e) =>
                    {
                        var le = (sender as Button).Tag as LayoutBase;
                        var coreView = CoreApplication.CreateNewView();
                        int coreViewId = 0;

                        await coreView.Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                        {
                            var view = ApplicationView.GetForCurrentView();
                            coreViewId = view.Id;

                            Window.Current.Content = BuildLayout(le);
                            view.Consolidated += (_, __) => Window.Current.Content = null;
                            view.Title = le.Title;
                            var titleBar = view.TitleBar;

                            Window.Current.Activate();
                        });
                        await ApplicationViewSwitcher.TryShowAsStandaloneAsync(coreViewId);
                    };
                    main.ViewSwitcher.Children.Add(button);
                }
            }
        }

        private Brush borderBrush = new SolidColorBrush(Colors.Gray);
        private UIElement BuildLayout(LayoutBase layout)
        {
            switch (layout)
            {
                case TabLayout tab:
                    var pivot = new Pivot { Name = tab.Id };
                    foreach (var child in tab.Children)
                        pivot.Items.Add(new PivotItem
                        {
                            Header = child.Title,
                            Content = BuildLayout(child)
                        });
                    return pivot;
                case RelativeLayout relative:
                    var panel = new RelativePanel();
                    foreach (var child in relative.Children)
                        panel.Children.Add(BuildLayout(child));
                    foreach (var relation in relative.Relations)
                    {
                        var item = panel.FindName(relation.Item) as UIElement;
                        switch (relation.Type)
                        {
                            case RelativeRelationType.LeftOf:
                                RelativePanel.SetLeftOf(item, relation.Base);
                                break;
                            case RelativeRelationType.RightOf:
                                RelativePanel.SetRightOf(item, relation.Base);
                                break;
                            case RelativeRelationType.Above:
                                RelativePanel.SetAbove(item, relation.Base);
                                break;
                            case RelativeRelationType.Below:
                                RelativePanel.SetBelow(item, relation.Base);
                                break;
                            case RelativeRelationType.AlignLeft:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignLeftWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignLeftWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignRight:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignRightWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignRightWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignTop:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignTopWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignTopWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignBottom:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignBottomWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignBottomWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignHorizontal:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignHorizontalCenterWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignHorizontalCenterWithPanel(item, true);
                                break;
                            case RelativeRelationType.AlignVertical:
                                if (relation.Base != null)
                                    RelativePanel.SetAlignVerticalCenterWith(item, relation.Base);
                                else
                                    RelativePanel.SetAlignVerticalCenterWithPanel(item, true);
                                break;
                        }
                    }
                    return panel;
                case LayoutItem item:
                    return new Border
                    {
                        Margin = new Thickness(2),
                        Padding = new Thickness(2),
                        BorderThickness = new Thickness(2),
                        BorderBrush = borderBrush
                    };
                default:
                    return null;
            }
        }
    }
}
