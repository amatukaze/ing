namespace Sakuno.ING.Game.Models.MasterData
{
    public partial class ShipInfo
    {
        partial void UpdateCore(RawShipInfo raw)
        {
            Type = _owner.ShipTypes[raw.TypeId];
            RemodelTo = _owner.ShipInfos[raw.RemodelTo];
        }
    }
}
