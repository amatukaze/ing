using Sakuno.ING.ViewModels.Homeport.Overall;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels.Tests.Overall;

public class TabsTests
{
    private readonly Subject<IReadOnlyList<IFleetUpdated>> _fleetsUpdatedSubject = new();
    private readonly TabsViewModel _vm;

    public TabsTests()
    {
        var gameProvider = Substitute.For<IGameProvider>();
        gameProvider.FleetsUpdated.Returns(_fleetsUpdatedSubject);

        _vm = new TabsViewModel(new PlayerDataService(gameProvider));
    }

    [Fact]
    public void SelectFleet()
    {
        FleetId fleetId = default;
        _vm.SelectedFleetId.Subscribe(value => fleetId = value);

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
