using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Catalog
{
    [ExportView("ShipCatalog")]
    public sealed partial class ShipCatalogView : UserControl
    {
        private readonly AdvancedCollectionView AllShipsView;

        public ShipCatalogView(NavalBase navalBase)
        {
            AllShipsView = new AdvancedCollectionView(navalBase.AllShips.DefaultView, isLiveShaping: true);
            InitializeComponent();
        }

        private void Catalog_Sorting(object sender, DataGridColumnEventArgs e)
        {
            AllShipsView.SortDescriptions.Clear();

            var nextDirection = e.Column.SortDirection switch
            {
                DataGridSortDirection.Descending => DataGridSortDirection.Ascending,
                DataGridSortDirection.Ascending => (DataGridSortDirection?)null,
                _ => DataGridSortDirection.Descending
            };
            var comparer = e.Column.Tag switch
            {
                "Id" => CreateComparer(x => x.Id),
                "Level" => CreateComparer(x => x.Leveling),
                "Morale" => CreateComparer(x => x.Morale),
                "RepairingTime" => CreateComparer(x => x.RepairingTime),
                "Firepower" => CreateComparer(x => x.Firepower.Current),
                "Torpedo" => CreateComparer(x => x.Torpedo.Current),
                "AntiAir" => CreateComparer(x => x.AntiAir.Current),
                "Armor" => CreateComparer(x => x.Armor.Current),
                "Evasion" => CreateComparer(x => x.Evasion.Current),
                "AntiSubmarine" => CreateComparer(x => x.AntiSubmarine.Current),
                "LineOfSight" => CreateComparer(x => x.LineOfSight.Current),
                "Luck" => CreateComparer(x => x.Luck.Current),
                _ => null
            };

            static Comparer<HomeportShip> CreateComparer<T>(Func<HomeportShip, T> keySelector)
                where T : IComparable<T>
                => Comparer<HomeportShip>.Create((x, y) =>
                    keySelector(x).CompareTo(keySelector(y)) switch
                    {
                        0 => x.CompareTo(y),
                        int other => other
                    });

            if (nextDirection is DataGridSortDirection sd && comparer != null)
            {
                AllShipsView.SortDescriptions.Add(
                    new SortDescription(sd switch
                    {
                        DataGridSortDirection.Ascending => SortDirection.Ascending,
                        DataGridSortDirection.Descending => SortDirection.Descending,
                        _ => (SortDirection)sd
                    }, comparer));
                e.Column.SortDirection = sd;
            }
            else
            {
                AllShipsView.SortDescriptions.Add(
                     new SortDescription(SortDirection.Ascending, Comparer<HomeportShip>.Default));
                e.Column.SortDirection = null;
            }
        }
    }
}
