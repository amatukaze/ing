using Xunit;

namespace Sakuno.ING.Game.Models.Tests
{
    public class FleetCompositionTests
    {
        private readonly Fixture _fixture = new();

        public FleetCompositionTests()
        {
            _fixture.Process("api_port/port", "Composition.port.json");
        }

        [Fact]
        public void AppendShip()
        {
            _fixture.Process("api_req_hensei/change", "Composition.request_append", null);

            Assert.Equal(5, _fixture.NavalBase.Fleets[(FleetId)1].Ships.Count);
        }

        [Fact]
        public void Remove()
        {
            _fixture.Process("api_req_hensei/change", "Composition.request_remove", null);

            var ships = _fixture.NavalBase.Fleets[(FleetId)1].Ships;

            Assert.Equal(3, ships.Count);
            Assert.Equal((ShipId)1, ships[0].Id);
            Assert.Equal((ShipId)2, ships[1].Id);
            Assert.Equal((ShipId)4, ships[2].Id);
        }

        [Fact]
        public void FlagshipOnly()
        {
            _fixture.Process("api_req_hensei/change", "Composition.request_flagship_only", null);

            var ships = _fixture.NavalBase.Fleets[(FleetId)1].Ships;

            Assert.Single(ships);
            Assert.Equal((ShipId)1, ships[0].Id);
        }

        [Fact]
        public void Exchange()
        {
            _fixture.Process("api_req_hensei/change", "Composition.request_exchange", null);

            var firstShips = _fixture.NavalBase.Fleets[(FleetId)1].Ships;
            var secondShips = _fixture.NavalBase.Fleets[(FleetId)2].Ships;

            Assert.Equal(4, firstShips.Count);
            Assert.Equal(2, secondShips.Count);
            Assert.Equal((ShipId)5, firstShips[1].Id);
            Assert.Equal((ShipId)2, secondShips[0].Id);
        }
    }
}
