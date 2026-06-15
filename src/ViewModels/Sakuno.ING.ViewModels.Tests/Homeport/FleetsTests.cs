namespace Sakuno.ING.ViewModels.Tests.Homeport;

public class FleetsTests
{
    private readonly Subject<IReadOnlyList<IFleetUpdated>> _fleetsUpdatedSubject = new();
    private readonly FleetsViewModel _vm;

    public FleetsTests()
    {
        var provider = Substitute.For<IGameProvider>();
        provider.FleetsUpdated.Returns(_fleetsUpdatedSubject);

        _vm = new FleetsViewModel(new PlayerDataService(provider));
    }

    [Fact]
    public void ItemsPopulatedAfterUpdatedEvent()
    {
        _fleetsUpdatedSubject.OnNext(Utils.GenerateMocks<IFleetUpdated, FleetId>(1, 4).ToArray());

        Assert.Equal(4, _vm.Fleets.Count);
    }
}
