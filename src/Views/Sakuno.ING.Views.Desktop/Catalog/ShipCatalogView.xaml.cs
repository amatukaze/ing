using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Catalog
{
    [ExportView("ShipCatalog")]
    public partial class ShipCatalogView : UserControl
    {
        public ShipCatalogView()
        {
            InitializeComponent();
        }

        private void Catalog_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var lcv = (sender as DataGrid)?.ItemsSource as ListCollectionView;
            if (lcv is null) return;
            var nextDirection = e.Column.SortDirection switch
            {
                ListSortDirection.Descending => ListSortDirection.Ascending,
                ListSortDirection.Ascending => (ListSortDirection?)null,
                _ => ListSortDirection.Descending
            };
            bool reverse = nextDirection == ListSortDirection.Descending;
            var comparer = e.Column.SortMemberPath switch
            {
                "Id" => CreateComparer(x => x.Id, reverse),
                "Level" => CreateComparer(x => x.Leveling, reverse),
                "Morale" => CreateComparer(x => x.Morale, reverse),
                "RepairingTime" => CreateComparer(x => x.RepairingTime, reverse),
                "Firepower" => CreateComparer(x => x.Firepower.Current, reverse),
                "Torpedo" => CreateComparer(x => x.Torpedo.Current, reverse),
                "AntiAir" => CreateComparer(x => x.AntiAir.Current, reverse),
                "Armor" => CreateComparer(x => x.Armor.Current, reverse),
                "Evasion" => CreateComparer(x => x.Evasion.Current, reverse),
                "AntiSubmarine" => CreateComparer(x => x.AntiSubmarine.Current, reverse),
                "LineOfSight" => CreateComparer(x => x.LineOfSight.Current, reverse),
                "Luck" => CreateComparer(x => x.Luck.Current, reverse),
                _ => null
            };

            static Comparer<HomeportShip> CreateComparer<T>(Func<HomeportShip, T> keySelector, bool reverse)
                where T : IComparable<T>
            {
                if (reverse)
                    return Comparer<HomeportShip>.Create((x, y) =>
                        keySelector(y).CompareTo(keySelector(x)) switch
                        {
                            0 => y.CompareTo(x),
                            int other => other
                        });
                else
                    return Comparer<HomeportShip>.Create((x, y) =>
                        keySelector(x).CompareTo(keySelector(y)) switch
                        {
                            0 => x.CompareTo(y),
                            int other => other
                        });
            }

            if (nextDirection is ListSortDirection sd && comparer != null)
            {
                lcv.CustomSort = comparer;
                e.Column.SortDirection = sd;
            }
            else
            {
                lcv.CustomSort = null;
                e.Column.SortDirection = null;
            }
            e.Handled = true;
        }
    }
}
