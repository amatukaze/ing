using ReactiveUI;

namespace Sakuno.ING.ViewModels.Homeport
{
    public class LockedDockViewModel : ReactiveObject, IDockViewModel
    {
        public int Id { get; }

        public LockedDockViewModel(int id)
        {
            Id = id;
        }
    }
}
