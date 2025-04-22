using Avalonia.Controls.Templates;
using Sakuno.ING.ViewModels;
using ViewModelViewHost = Sakuno.ING.Shell.Controls.ViewModelViewHost;

namespace Sakuno.ING.Shell;

internal class ViewLocator : IDataTemplate
{
    public bool Match(object? data) => data is ViewModelObject;

    public Control Build(object? param)
    {
        if (param is null)
            return new TextBlock() { Text = "(null)" };

        return new ViewModelViewHost() { ViewModel = param };
    }
}
