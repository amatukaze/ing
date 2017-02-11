using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.Controls
{
    public class ShipAvatar : Control
    {
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register("Shape", typeof(AvatarShape), typeof(ShipAvatar),
            new UIPropertyMetadata());
        public AvatarShape Shape
        {
            get { return (AvatarShape)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public static readonly DependencyProperty IDProperty = DependencyProperty.Register(nameof(ID), typeof(int), typeof(ShipAvatar),
            new UIPropertyMetadata(Int32Util.Zero, (s, e) => ((ShipAvatar)s).OnShipChanged((int)e.NewValue)));

        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        static readonly DependencyPropertyKey BrushPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Brush), typeof(Brush), typeof(ShipAvatar),
            new UIPropertyMetadata(null));
        public static readonly DependencyProperty BrushProperty = BrushPropertyKey.DependencyProperty;
        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            private set { SetValue(BrushPropertyKey, value); }
        }

        static string r_DefaultRootDirectory = Path.Combine(ProductInfo.RootDirectory, "Resources", "Avatars", "Ships") + "\\";
        static string r_ModRootDirectory = Path.Combine(ProductInfo.RootDirectory, "Mods", "ShipAvatars") + "\\";

        static Dictionary<int, WeakReference<ImageBrush>> r_CachedAvatars = new Dictionary<int, WeakReference<ImageBrush>>();

        static ShipAvatar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShipAvatar), new FrameworkPropertyMetadata(typeof(ShipAvatar)));
        }

        void OnShipChanged(int rpID)
        {
            if (rpID == 0)
            {
                Brush = null;
                return;
            }

            var rIsModAvailable = File.Exists(r_ModRootDirectory + rpID + "_n.png");
            if (!rIsModAvailable && !File.Exists(r_DefaultRootDirectory + rpID + "_n.png"))
            {
                Brush = null;
                return;
            }

            WeakReference<ImageBrush> rBrushWeakReference;
            if (!r_CachedAvatars.TryGetValue(rpID, out rBrushWeakReference))
                r_CachedAvatars.Add(rpID, rBrushWeakReference = new WeakReference<ImageBrush>(null));

            ImageBrush rBrush;
            if (rBrushWeakReference.TryGetTarget(out rBrush))
            {
                Brush = rBrush;
                return;
            }

            BitmapSource rImage;
            if (!rIsModAvailable)
                rImage = BitmapFrame.Create(new Uri("pack://siteoforigin:,,,/Resources/Avatars/Ships/" + rpID + "_n.png"));
            else
                rImage = BitmapFrame.Create(new Uri("pack://siteoforigin:,,,/Mods/ShipAvatars/" + rpID + "_n.png"));
            rImage.Freeze();

            rBrush = new ImageBrush(rImage);
            rBrush.Freeze();

            rBrushWeakReference.SetTarget(rBrush);

            Brush = rBrush;
        }
    }
}
