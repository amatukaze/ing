using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.Battle
{
    class EnemyShip : IParticipant
    {
        public ShipInfo Info { get; }

        public int Level { get; }
        public IList<ShipSlot> Slots { get; }

        public bool IsMVP => false;
        public Ship Ship => null;
        public ShipSlot ExtraSlot => null;

        public EnemyShip(int rpID, int rpLevel, int[] rpEquipment = null)
        {
            Info = KanColleGame.Current.MasterInfo.Ships[rpID];

            Level = rpLevel;
            if (rpEquipment != null)
                Slots = rpEquipment.TakeWhile(r => r != -1).Select((r, i) =>
                {
                    var rMaxPlaneCount = i <= Info.SlotCount ? Info.PlaneCountInSlot[i] : 0;
                    return new ShipSlot(Equipment.GetDummy(r), rMaxPlaneCount, rMaxPlaneCount);
                }).ToList();
        }

        public override string ToString() => $"{Info} Lv. {Level}";
    }
}
