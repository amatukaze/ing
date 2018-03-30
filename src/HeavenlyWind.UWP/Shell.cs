using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Shell;
using Sakuno.KanColle.Amatsukaze.ViewModels;
using Sakuno.KanColle.Amatsukaze.ViewModels.Layout;
using Windows.Storage;
using Windows.UI.Xaml;

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

            Window.Current.Content = new MainPage(new MainWindowVM(), this);
            Window.Current.Activate();
        }

        internal readonly Dictionary<string, ViewDescriptor> Views = new Dictionary<string, ViewDescriptor>();
        internal LayoutRoot Layout;
        private bool started;

        public void RegisterView(ViewDescriptor descriptor)
        {
            if (started) throw new InvalidOperationException("Shell already started.");
            Views.Add(descriptor.Id, descriptor);
        }
    }
}
