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
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register(nameof(Shape), typeof(AvatarShape), typeof(ShipAvatar),
            new UIPropertyMetadata());
        public AvatarShape Shape
        {
            get { return (AvatarShape)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        public static readonly DependencyProperty IDProperty = DependencyProperty.Register(nameof(ID), typeof(int), typeof(ShipAvatar),
            new UIPropertyMetadata(Int32Util.Zero, (s, e) => ((ShipAvatar)s).UpdateAvatar()));

        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }

        public static readonly DependencyProperty IsDamagedProperty = DependencyProperty.Register(nameof(IsDamaged), typeof(bool), typeof(ShipAvatar),
            new UIPropertyMetadata(BooleanUtil.False, (s, e) => ((ShipAvatar)s).UpdateAvatar()));

        public bool IsDamaged
        {
            get { return (bool)GetValue(IsDamagedProperty); }
            set { SetValue(IsDamagedProperty, value); }
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

        static SortedList<int, WeakReference<ImageBrush>> r_CachedNormalAvatars = new SortedList<int, WeakReference<ImageBrush>>();
        static SortedList<int, WeakReference<ImageBrush>> r_CachedDamagedAvatars = new SortedList<int, WeakReference<ImageBrush>>();

        static ShipAvatar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ShipAvatar), new FrameworkPropertyMetadata(typeof(ShipAvatar)));
        }

        void UpdateAvatar()
        {
            var rID = ID;
            var rIsDamaged = IsDamaged;

            if (rID == 0)
            {
                Brush = null;
                return;
            }

            foreach (var rInfo in GetAvatarInfos(rID, rIsDamaged))
            {
                if (!File.Exists(rInfo.Filename))
                    continue;

                var rCache = !rIsDamaged ? r_CachedNormalAvatars : r_CachedDamagedAvatars;

                WeakReference<ImageBrush> rBrushWeakReference;
                if (!rCache.TryGetValue(rID, out rBrushWeakReference))
                    rCache.Add(rID, rBrushWeakReference = new WeakReference<ImageBrush>(null));

                ImageBrush rBrush;
                if (rBrushWeakReference.TryGetTarget(out rBrush))
                {
                    Brush = rBrush;
                    return;
                }

                LoadAvatar(rInfo.Uri, rBrushWeakReference);
                return;
            }

            Brush = null;
        }
        void LoadAvatar(string rpUri, WeakReference<ImageBrush> rpWeakReference)
        {
            var rImage = BitmapFrame.Create(new Uri(rpUri));
            rImage.Freeze();

            var rBrush = new ImageBrush(rImage);
            rBrush.Freeze();

            rpWeakReference.SetTarget(rBrush);

            Brush = rBrush;
        }

        static IEnumerable<AvatarFileInfo> GetAvatarInfos(int rpID, bool rpCheckDamaged)
        {
            const string NormalSubfix = "_n.png";
            const string DamagedSubfix = "_d.png";

            if (rpCheckDamaged)
            {
                yield return new AvatarFileInfo(r_ModRootDirectory + rpID + DamagedSubfix, "pack://siteoforigin:,,,/Mods/ShipAvatars/" + rpID + DamagedSubfix);
                yield return new AvatarFileInfo(r_DefaultRootDirectory + rpID + DamagedSubfix, "pack://siteoforigin:,,,/Resources/Avatars/Ships/" + rpID + DamagedSubfix);
            }

            yield return new AvatarFileInfo(r_ModRootDirectory + rpID + NormalSubfix, "pack://siteoforigin:,,,/Mods/ShipAvatars/" + rpID + NormalSubfix);
            yield return new AvatarFileInfo(r_DefaultRootDirectory + rpID + NormalSubfix, "pack://siteoforigin:,,,/Resources/Avatars/Ships/" + rpID + NormalSubfix);
        }

        struct AvatarFileInfo
        {
            public string Filename { get; }
            public string Uri { get;}

            public AvatarFileInfo(string rpFilename, string rpUri)
            {
                Filename = rpFilename;
                Uri = rpUri;
            }
        }
    }
}
