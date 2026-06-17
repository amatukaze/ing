namespace Sakuno.ING.ViewModels.Homeport;

public class MaterialViewModel : ViewModelObject
{
    public IObservable<int> Current { get; }
    public IObservable<int> Difference { get; }

    public MaterialViewModel(IObservable<int> source)
    {
        Current = source.ObserveOn(RxSchedulers.MainThreadScheduler);
        Difference = source.Throttle(TimeSpan.FromSeconds(1L), RxSchedulers.TaskpoolScheduler)
            .Scan(new DiffState(), (state, current) =>
            {
                state.Previous = state.Current;
                state.Current = current;

                return state;
            })
            .Select(state => state.Current - state.Previous)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
    }

    private sealed class DiffState
    {
        public int Previous { get; set; }
        public int Current { get; set; }
    }
}
