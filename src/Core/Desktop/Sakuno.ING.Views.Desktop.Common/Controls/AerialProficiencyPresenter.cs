using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public sealed class AerialProficiencyPresenter : Control
    {
        private const string Proficiency1Path = "M0,0 L0,20 2,20 2,0";
        private const string Proficiency2Path = "M0,0 L0,20 2,20 2,0 M4,0 L4,20 6,20 6,0";
        private const string Proficiency3Path = "M0,0 L0,20 2,20 2,0 M4,0 L4,20 6,20 6,0 M8,0 L8,20 10,20 10,0";
        private const string Proficiency4Path = "M0,0 L5,20 7,20 2,0";
        private const string Proficiency5Path = "M0,0 L5,20 7,20 2,0 M5,0 10,20 12,20 7,0";
        private const string Proficiency6Path = "M0,0 L5,20 7,20 2,0 M5,0 10,20 12,20 7,0 M10,0 15,20 17,20 12,0 ";
        private const string Proficiency7Path = "M0,0 L5,10 0,20 2,20 7,10 2,0 M5,0 L10,10 5,20 7,20 12,10 7,0";

        public static readonly DependencyProperty ProficiencyProperty =
            DependencyProperty.Register(nameof(Proficiency), typeof(int), typeof(AerialProficiencyPresenter), new PropertyMetadata(0, Update));
        public int Proficiency
        {
            get => (int)GetValue(ProficiencyProperty);
            set => SetValue(ProficiencyProperty, value);
        }

        private readonly Path _path = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        public AerialProficiencyPresenter()
        {
            AddVisualChild(_path);
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _path;

        private static readonly Geometry?[] _geometries = new Geometry?[7];
        private static Brush? _brush123;
        private static Brush? _brush4567;
        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = (AerialProficiencyPresenter)d;
            var proficiency = (int)e.NewValue;

            if (proficiency < 0 || proficiency > 7)
                throw new InvalidOperationException("Unsupported proficiency: " + proficiency);

            Geometry data;
            Brush fill;

            if (proficiency == 0)
            {
                data = Geometry.Empty;
                fill = Brushes.Transparent;
            }
            else
            {
                data = GetGeometry(proficiency);
                fill = GetFill(proficiency);
            }

            i._path.Data = data;
            i._path.Fill = fill;

        }
        private static Geometry GetGeometry(int proficiency)
        {
            if (_geometries[proficiency - 1] is not Geometry result)
            {
                result = StreamGeometry.Parse(proficiency switch
                {
                    1 => Proficiency1Path,
                    2 => Proficiency2Path,
                    3 => Proficiency3Path,
                    4 => Proficiency4Path,
                    5 => Proficiency5Path,
                    6 => Proficiency6Path,
                    7 => Proficiency7Path,

                    _ => throw new Exception(),
                });
                result.Freeze();

                _geometries[proficiency - 1] = result;
            }

            return result;
        }
        private static Brush GetFill(int proficiency)
        {
            if (proficiency > 0 && proficiency < 4)
            {
                if (_brush123 is null)
                {
                    _brush123 = new SolidColorBrush(Color.FromRgb(154, 181, 208));
                    _brush123.Freeze();
                }

                return _brush123;
            }

            if (_brush4567 is null)
            {
                _brush4567 = new SolidColorBrush(Color.FromRgb(211, 153, 6));
                _brush4567.Freeze();
            }

            return _brush4567;
        }
    }
}
