using Sakuno.ING.Composition;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Models;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP.Views.MasterData
{
    public sealed partial class MasterDataView : UserControl
    {
        private readonly MasterDataRoot MasterData = StaticResolver.Instance.Resolve<NavalBase>().MasterData;
        public MasterDataView()
        {
            this.InitializeComponent();
        }

        private static IBindableCollection<T> GetDefaultView<T>(ITable<T> source)
            where T : IIdentifiable
            => new BindableSnapshotCollection<T>(source, source.OrderBy(x => x.Id));
    }
}
