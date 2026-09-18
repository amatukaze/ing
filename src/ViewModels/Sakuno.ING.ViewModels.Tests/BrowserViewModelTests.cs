using System.Windows.Input;

namespace Sakuno.ING.ViewModels.Tests;

public class BrowserViewModelTests
{
    private static readonly Uri GameUri = new(BrowserViewModel.GameUrl);

    private readonly BrowserViewModel _vm = new();

    [Fact]
    public void InitialStateNavigatesToGameUrl()
    {
        Assert.Equal(GameUri, _vm.Source);
        Assert.False(_vm.IsNavigating);
        Assert.False(_vm.CanGoBack);
        Assert.False(_vm.CanGoForward);
    }

    [Fact]
    public void GoHomeRestoresGameUrl()
    {
        _vm.Source = new Uri("https://www.dmm.com/");

        _vm.GoHome.Execute(Unit.Default).Subscribe();

        Assert.Equal(GameUri, _vm.Source);
    }

    [Fact]
    public void GoBackAndGoForwardCanExecuteReflectNavigationState()
    {
        Assert.False(((ICommand)_vm.GoBack).CanExecute(null));
        Assert.False(((ICommand)_vm.GoForward).CanExecute(null));

        _vm.OnNavigationCompleted(new Uri("https://www.dmm.com/"), canGoBack: true, canGoForward: false, isSuccess: true);

        Assert.True(((ICommand)_vm.GoBack).CanExecute(null));
        Assert.False(((ICommand)_vm.GoForward).CanExecute(null));
    }

    [Fact]
    public void SuccessfulNavigationSyncsAddress()
    {
        _vm.OnNavigationStarted();
        Assert.True(_vm.IsNavigating);

        var uri = new Uri("https://games.dmm.com/detail/kancolle");
        _vm.OnNavigationCompleted(uri, canGoBack: true, canGoForward: false, isSuccess: true);

        Assert.Equal(uri.ToString(), _vm.Address);
        Assert.True(_vm.CanGoBack);
        Assert.False(_vm.CanGoForward);
        Assert.False(_vm.IsNavigating);
    }

    [Fact]
    public void FailedNavigationKeepsAddress()
    {
        _vm.OnNavigationStarted();

        _vm.OnNavigationCompleted(new Uri("https://accounts.dmm.com/"), canGoBack: false, canGoForward: false, isSuccess: false);

        Assert.Equal(BrowserViewModel.GameUrl, _vm.Address);
        Assert.False(_vm.IsNavigating);
    }

    [Fact]
    public void NavigateSetsSourceFromAddress()
    {
        _vm.Address = "https://www.dmm.com/";

        _vm.Navigate.Execute(Unit.Default).Subscribe();

        Assert.Equal(new Uri("https://www.dmm.com/"), _vm.Source);
    }

    [Fact]
    public void NavigateIgnoresInvalidAddress()
    {
        var originalSource = _vm.Source;

        _vm.Address = "not a url";
        _vm.Navigate.Execute(Unit.Default).Subscribe();

        Assert.Equal(originalSource, _vm.Source);
    }
}
