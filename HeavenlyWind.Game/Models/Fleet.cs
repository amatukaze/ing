using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public class Fleet : RawDataWrapper<RawFleet>, IID
    {
        public Port Port { get; }

        public int ID => RawData.ID;

        string r_Name;
        public string Name
        {
            get { return r_Name; }
            internal set
            {
                if (r_Name != value)
                {
                    r_Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        int[] r_ShipIDs;
        List<Ship> r_ShipList;
        ReadOnlyCollection<Ship> r_Ships;
        public ReadOnlyCollection<Ship> Ships
        {
            get { return r_Ships; }
            private set
            {
                if (r_Ships != value)
                {
                    r_Ships = value;
                    OnPropertyChanged(nameof(Ships));
                }
            }
        }

        public FleetStatus Status { get; }

        internal Fleet(Port rpPort, RawFleet rpRawData) : base(rpRawData)
        {
            Port = rpPort;

            Status = new FleetStatus(this);

            OnRawDataUpdated();
        }

        protected override void OnRawDataUpdated()
        {
            Name = RawData.Name;

            if (r_ShipIDs == null || !r_ShipIDs.SequenceEqual(RawData.Ship))
            {
                r_ShipIDs = RawData.Ship;
                r_ShipList = RawData.Ship.TakeWhile(r => r != -1).Select(r => Port.Ships[r]).ToList();
                Ships = r_ShipList.AsReadOnly();
            }

            Status.Update();
        }

        public override string ToString()
        {
            var rBuilder = new StringBuilder(64);

            rBuilder.Append($"ID = {ID}, Name = \"{Name}\", Ship");
            if (Ships.Count > 1)
                rBuilder.Append('s');
            rBuilder.Append(" = ").Append(Ships.Select(r => $"\"{r.Info.Name}\"").Join(", "));

            return rBuilder.ToString();
        }
    }
}
