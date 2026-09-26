using Sakuno.ING.Bootstrap;

namespace Sakuno.ING.ViewModels.Bootstrap;

public class BootstrapStepViewModel : BindableObject
{
    private readonly IBootstrapStep _step;

    public string Name => _step.Name;
    public bool CanSkip => _step.CanSkip;

    public BootstrapStepStatus Status
    {
        get;
        internal set
        {
            if (SetField(ref field, value))
                NotifyPropertyChanged(nameof(StatusGlyph));
        }
    } = BootstrapStepStatus.Pending;

    public string StatusGlyph => Status switch
    {
        BootstrapStepStatus.Pending => "○",
        BootstrapStepStatus.Running => "…",
        BootstrapStepStatus.Succeeded => "✓",
        BootstrapStepStatus.Failed => "✗",
        BootstrapStepStatus.Skipped => "–",
        _ => string.Empty,
    };

    public string? Message
    {
        get;
        internal set => SetField(ref field, value);
    }

    internal IBootstrapStep Step => _step;

    internal BootstrapStepViewModel(IBootstrapStep step) => _step = step;
}
