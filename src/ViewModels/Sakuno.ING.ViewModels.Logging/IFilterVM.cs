namespace Sakuno.ING.ViewModels.Logging
{
    public interface IFilterVM : IBindable
    {
        string Name { get; }
        bool IsEnabled { get; set; }
        string SelectedText { get; set; }
        IBindableCollection<string> Candidates { get; }
    }
}
