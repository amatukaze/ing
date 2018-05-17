using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Models
{
    abstract class ZoomInfo : ModelBase
    {
        public double Zoom { get; }

        protected bool r_IsSelected;
        public bool IsSelected
        {
            get { return r_IsSelected; }
            internal set
            {
                r_IsSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public ICommand Command { get; }

        internal protected ZoomInfo(double rpZoom, ICommand rpCommand)
        {
            Zoom = rpZoom;

            Command = rpCommand;
        }
    }
}
