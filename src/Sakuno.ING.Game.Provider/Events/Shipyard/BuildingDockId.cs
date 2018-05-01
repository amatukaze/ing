namespace Sakuno.ING.Game.Events.Shipyard
{
    public readonly struct BuildingDockId
    {
        public readonly int DockId;

        public BuildingDockId(int dockId) => DockId = dockId;
    }
}
