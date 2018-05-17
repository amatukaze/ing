using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class FleetResupplyInfo : ModelBase
    {
        Fleet r_Fleet;

        public bool NeedResupply { get; private set; }

        public int Fuel { get; private set; }
        public int Bullet { get; private set; }
        public int Plane { get; private set; }

        internal FleetResupplyInfo(Fleet rpFleet)
        {
            r_Fleet = rpFleet;
        }

        internal void Update()
        {
            var rFuel = 0;
            var rBullet = 0;
            var rPlane = 0;

            foreach (var rShip in r_Fleet.Ships)
            {
                var rFuelConsumption = rShip.Fuel.Maximum - rShip.Fuel.Current;
                var rBulletConsumption = rShip.Bullet.Maximum - rShip.Bullet.Current;
                if (rShip.IsMarried)
                {
                    rFuelConsumption = (int)(rFuelConsumption * .85);
                    rBulletConsumption = (int)(rBulletConsumption * .85);
                }

                rFuel += rFuelConsumption;
                rBullet += rBulletConsumption;

                rPlane += rShip.Slots.Sum(r => r.MaxPlaneCount - r.PlaneCount);
            }

            NeedResupply = rFuel > 0 || rBullet > 0 || rPlane > 0;
            Fuel = rFuel;
            Bullet = rBullet;
            Plane = rPlane;

            OnPropertyChanged(nameof(NeedResupply));
            OnPropertyChanged(nameof(Fuel));
            OnPropertyChanged(nameof(Bullet));
            OnPropertyChanged(nameof(Plane));
        }
    }
}
