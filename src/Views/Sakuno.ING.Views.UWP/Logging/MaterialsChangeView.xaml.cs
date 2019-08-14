using System.Collections.Generic;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sakuno.ING.Views.UWP.Logging
{
    [ExportView("MaterialsChangeLogs")]
    public sealed partial class MaterialsChangeView : UserControl
    {
        private readonly MaterialsChangeLogsVM ViewModel;
        public MaterialsChangeView(MaterialsChangeLogsVM viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        public static Geometry ConvertShape(IReadOnlyList<MaterialsChangeEntity> data, int id)
        {
            var baseTime = data[0].TimeStamp;
            var baseAmount = data[0].Materials;
            var figure = new PathFigure { IsClosed = false };
            foreach (var p in data)
            {
                var diff = p.Materials - baseAmount;
                figure.Segments.Add(new LineSegment
                {
                    Point = new Point((p.TimeStamp - baseTime).TotalMinutes, id switch
                    {
                        0 => diff.Fuel,
                        1 => diff.Bullet,
                        2 => diff.Steel,
                        3 => diff.Bauxite,
                        4 => diff.InstantBuild,
                        5 => diff.InstantRepair,
                        6 => diff.Development,
                        7 => diff.Improvement,
                        _ => 0
                    })
                });
            }
            return new PathGeometry
            {
                Figures = { figure }
            };
        }
    }
}
