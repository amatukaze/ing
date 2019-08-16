using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.ViewModels.Logging;

namespace Sakuno.ING.Views.Desktop.Logging
{
    internal class MaterialsChartPathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = (int)parameter;
            if (!(value is MaterialsChartData chartData)) return null;

            var duration = chartData.End - chartData.Start;
            Point SelectPoint(MaterialsChangeEntity p)
            // TODO: TimeSpan.operator/ exists in netstandard 2.1
                => new Point((p.TimeStamp - chartData.Start).TotalDays / duration.TotalDays,
                    id switch
                    {
                        0 => p.Materials.Fuel / 300000.0,
                        1 => p.Materials.Bullet / 300000.0,
                        2 => p.Materials.Steel / 300000.0,
                        3 => p.Materials.Bauxite / 300000.0,
                        4 => p.Materials.InstantBuild / 3000.0,
                        5 => p.Materials.InstantRepair / 3000.0,
                        6 => p.Materials.Development / 3000.0,
                        7 => p.Materials.Improvement / 3000.0,
                        _ => 0
                    });

            var figure = new PathFigure
            {
                IsClosed = false,
                StartPoint = SelectPoint(chartData.Entities[0])
            };

            foreach (var p in chartData.Entities)
                figure.Segments.Add(new LineSegment
                {
                    Point = SelectPoint(p)
                });

            return new PathGeometry
            {
                Figures =
                {
                    new PathFigure { Segments = { new LineSegment() } },
                    figure,
                    new PathFigure { StartPoint = new Point(1, 1), Segments = { new LineSegment { Point = new Point(1, 1) } } }
                }
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
