namespace Sakuno.ING.ViewModels.Tests.Homeport;

public class RepairDocksTests
{
    private readonly Subject<IReadOnlyList<IRepairDockUpdated>> _repairDocksUpdatedSubject = new();
    private readonly RepairDocksViewModel _vm;

    public RepairDocksTests()
    {
        var provider = Substitute.For<IGameProvider>();
        provider.RepairDocksUpdated.Returns(_repairDocksUpdatedSubject);

        _vm = new RepairDocksViewModel(new PlayerDataService(provider));
    }

    [Fact]
    public void ItemsPopulatedAfterUpdatedEvent()
    {
        _repairDocksUpdatedSubject.OnNext(Utils.GenerateMocks<IRepairDockUpdated, RepairDockId>(1, 4).ToArray());

        Assert.Equal(4, _vm.RepairDocks.Count);
    }
}
