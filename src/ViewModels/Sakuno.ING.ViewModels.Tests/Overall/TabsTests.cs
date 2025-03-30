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

        for (var i = 0; i < _vm.Fleets.Count; i++)
        {
            var currentFleet = _vm.Fleets[i];

            currentFleet.SelectCommand.Execute().Subscribe();

            Assert.Equal((FleetId)(i + 1), currentFleet.Id);

            foreach (var fleet in _vm.Fleets.Except([currentFleet]))
                Assert.False(fleet.IsSelected);

            Assert.True(currentFleet.IsSelected);
        }
    }
}
