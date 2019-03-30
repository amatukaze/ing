using Sakuno.ING.Game.Models;
using Sakuno.ING.ViewModels.Homeport;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Controls
{
    public sealed class LoSIndicator : Control
    {
        public LoSIndicator()
        {
            DefaultStyleKey = typeof(LoSIndicator);
        }

        public FleetLoSVM ViewModel { get; } = new FleetLoSVM();

        public LineOfSight Effective
        {
            get => ViewModel.Effective;
            set => ViewModel.Effective = value;
        }

        public int Simple
        {
            get => ViewModel.Simple;
            set => ViewModel.Simple = value;
        }
    }
}
