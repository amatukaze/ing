using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.ViewModels.Tests.Overall;

public class TabsTests
{
    private readonly Subject<IReadOnlyList<IFleetUpdated>> _fleetsUpdatedSubject = new();
    private readonly TabsViewModel _vm;
    private readonly FleetSelectionState _fleetSelectionState;

    public TabsTests()
    {
        var gameProvider = Substitute.For<IGameProvider>();
        gameProvider.FleetsUpdated.Returns(_fleetsUpdatedSubject);

        var playerDataService = new PlayerDataService(gameProvider);
        _fleetSelectionState = new(playerDataService);
        _vm = new TabsViewModel(playerDataService, _fleetSelectionState);
    }

    [Fact]
    public void SelectFleet()
    {
        FleetId fleetId = default;
        _fleetSelectionState.SelectedId.Subscribe(value => fleetId = value);

        _fleetsUpdatedSubject.OnNext(Utils.GenerateMocks<IFleetUpdated, FleetId>(1, 4).ToArray());

        Assert.Equal((FleetId)1, fleetId);

        var isSelected1 = new ObservableCollector<bool>();
        var isSelected2 = new ObservableCollector<bool>();
        var isSelected3 = new ObservableCollector<bool>();
        var isSelected4 = new ObservableCollector<bool>();

        _vm.Fleets[0].IsSelected.Subscribe(isSelected1);
        _vm.Fleets[1].IsSelected.Subscribe(isSelected2);
        _vm.Fleets[2].IsSelected.Subscribe(isSelected3);
        _vm.Fleets[3].IsSelected.Subscribe(isSelected4);

        foreach (var currentFleet in _vm.Fleets)
            currentFleet.SelectCommand.Execute().Subscribe();

        Assert.Equal([true, false, false, false], isSelected1.Values);
        Assert.Equal([false, true, false, false], isSelected2.Values);
        Assert.Equal([false, false, true, false], isSelected3.Values);
        Assert.Equal([false, false, false, true], isSelected4.Values);
    }
}
