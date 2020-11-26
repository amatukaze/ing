namespace Sakuno.ING.Game.Models
{
    public partial class PlayerShip
    {
        partial void UpdateCore(RawShip raw)
        {
            static ShipModernizationStatus Combine(ShipModernizationStatus current, ShipModernizationStatus master) =>
                new ShipModernizationStatus
                (
                    min: master.Min,
                    max: master.Max,
                    displaying: current.Displaying,
                    improved: current.Improved
                );

            Info = _owner.MasterData.ShipInfos[raw.ShipInfoId];

            Fuel = (raw.Fuel, Info.FuelConsumption);
            Bullet = (raw.Bullet, Info.BulletConsumption);
            Firepower = Combine(raw.Firepower, Info.Firepower);
            Torpedo = Combine(raw.Torpedo, Info.Torpedo);
            AntiAir = Combine(raw.AntiAir, Info.AntiAir);
            Armor = Combine(raw.Armor, Info.Armor);
            Luck = Combine(raw.Luck, Info.Luck);
        }
    }
}
