using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.MasterData
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
