using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Overviews
{
    class ShipCollectionView : ItemsView<ShipViewModel>
    {
        ShipOverviewViewModel r_Owner;

        IDTable<ShipViewModel> r_Ships = new IDTable<ShipViewModel>();
        protected override IEnumerable<ShipViewModel> Source => r_Ships.Values;

        public string SortingColumn { get; set; }

        public ShipCollectionView(ShipOverviewViewModel rpOwner)
        {
            r_Owner = rpOwner;
        }

        protected override void BeforeRefresh() =>
            r_Ships.UpdateRawData(KanColleGame.Current.Port.Ships.Values,
                r => new ShipViewModel(r, r_Owner.TypeMaps[r.Info.Type]),
                (r, _) => r.UpdateType(r_Owner.TypeMaps));

        protected override bool Filter(ShipViewModel rpItem) =>
            rpItem.Type.IsSelected &&
            (!r_Owner.ExceptExpeditionShips || (rpItem.Ship.State & ShipState.Expedition) == 0) &&
            (!r_Owner.ExceptSparklingShips || (rpItem.Ship.Condition < 50)) &&
            (!r_Owner.ExceptLevel1Ships || (rpItem.Ship.Level > 1));

        protected override IEnumerable<ShipViewModel> Sort(IEnumerable<ShipViewModel> rpItems)
        {
            switch (SortingColumn)
            {
                case "ID":
                    return rpItems.OrderBy(r => r.Ship.ID);

                case "Name":
                    return rpItems.OrderBy(r => r.Ship.Info.Name);

                case "ShipLocking":
                    return rpItems.OrderByDescending(r => r.Ship.RawData.LockingTag != 0).ThenBy(r => r.Ship.RawData.LockingTag);

                case "Level":
                    return rpItems.OrderByDescending(r => r.Ship.Level).ThenBy(r => r.Ship.SortNumber).ThenBy(r => r.Ship.ID);

                case "Condition":
                    return rpItems.OrderByDescending(r => r.Ship.Condition);

                case "Firepower":
                    return rpItems.OrderByDescending(r => r.Ship.Status.FirepowerBase.Current);
                case "Torpedo":
                    return rpItems.OrderByDescending(r => r.Ship.Status.TorpedoBase.Current);
                case "AA":
                    return rpItems.OrderByDescending(r => r.Ship.Status.AABase.Current);
                case "Armor":
                    return rpItems.OrderByDescending(r => r.Ship.Status.ArmorBase.Current);
                case "Luck":
                    return rpItems.OrderByDescending(r => r.Ship.Status.LuckBase.Current);

                case "Evasion":
                    return rpItems.OrderByDescending(r => r.Ship.Status.Evasion);
                case "ASW":
                    return rpItems.OrderByDescending(r => r.Ship.Status.ASW);
                case "LoS":
                    return rpItems.OrderByDescending(r => r.Ship.Status.LoS);

                case "RepairTime":
                    return rpItems.OrderByDescending(r => r.Ship.RepairTime);

                default:
                    return rpItems;
            }
        }

        protected override void DisposeManagedResources()
        {
            r_Owner = null;
            r_Ships = null;

            base.DisposeManagedResources();
        }
    }
}
