namespace Sakuno.ING.ViewModels.Tests;

public class ConstructionDocksTests
{
    private readonly Subject<IReadOnlyList<IConstructionDockUpdated>> _constructionDocksUpdatedSubject = new();
    private readonly ConstructionDocksViewModel _vm;

    public ConstructionDocksTests()
    {
        var provider = Substitute.For<IGameProvider>();
        provider.ConstructionDocksUpdated.Returns(_constructionDocksUpdatedSubject);

        _vm = new ConstructionDocksViewModel(new PlayerDataService(provider));
    }

    [Fact]
    public void ZeroCountAtFirst()
    {
        Assert.Empty(_vm.ConstructionDocks);
    }

    [Fact]
    public void NonZeroCountWithData()
    {
        _constructionDocksUpdatedSubject.OnNext(Utils.GenerateMocks<IConstructionDockUpdated, ConstructionDockId>(1, 4).ToArray());

        Assert.Equal(4, _vm.ConstructionDocks.Count);
    }
}
