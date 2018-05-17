using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.UserInterface;
using Sakuno.UserInterface.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class OverviewScreenshotToolViewModel : ModelBase
    {
        internal ScreenshotToolViewModel Owner { get; }

        public IList<int> Numbers { get; } = Enumerable.Range(1, 6).ToList();

        ScreenshotRegion r_Type = ScreenshotRegion.ShipDetail;
        public ScreenshotRegion Type
        {
            get { return r_Type; }
            set
            {
                if (r_Type != value)
                {
                    r_Type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }

        int r_Column;
        public int Column
        {
            get { return r_Column; }
            set
            {
                if (r_Column != value)
                    UpdatePreviewBox(value, r_Row);
            }
        }

        int r_Row;
        public int Row
        {
            get { return r_Row; }
            set
            {
                if (r_Row != value)
                    UpdatePreviewBox(r_Column, value);
            }
        }

        public int PreviewBoxWidth { get; private set; }
        public int PreviewBoxHeight { get; private set; }

        public IList<ScreenshotToolPreviewViewModel> Previews { get; private set; }

        public ICommand SaveCommand { get; }

        internal OverviewScreenshotToolViewModel(ScreenshotToolViewModel rpOwner)
        {
            Owner = rpOwner;

            SaveCommand = new DelegatedCommand(Save);

            UpdatePreviewBox(2, 3);
        }

        void UpdatePreviewBox(int rpColumn, int rpRow)
        {
            PreviewBoxWidth = 78 * rpColumn + 2;
            PreviewBoxHeight = 61 * rpRow + 2;

            Previews = Enumerable.Range(0, rpColumn * rpRow).Select(r => new ScreenshotToolPreviewViewModel(this)).ToList();

            r_Column = rpColumn;
            r_Row = rpRow;
            OnPropertyChanged(nameof(Previews));
            OnPropertyChanged(nameof(Column));
            OnPropertyChanged(nameof(Row));
            OnPropertyChanged(nameof(PreviewBoxWidth));
            OnPropertyChanged(nameof(PreviewBoxHeight));
        }

        void Save()
        {
            var rRect = ScreenshotToolViewModel.Regions[Type];

            var rDrawingVisual = new DrawingVisual();
            using (var rDrawingContext = rDrawingVisual.RenderOpen())
                for (var i = 0; i < Previews.Count; i++)
                {
                    var rPreview = Previews[i];

                    var rLeft = i % r_Column * rRect.Width;
                    var rTop = i / r_Column * rRect.Height;
                    var rDestinationRect = new Rect(rLeft, rTop, rRect.Width, rRect.Height);

                    if (rPreview.Screenshot != null)
                        rDrawingContext.DrawImage(rPreview.Screenshot, rDestinationRect);
                    else
                        rDrawingContext.DrawRectangle(Brushes.Black, null, rDestinationRect);
                }

            var rWidth = r_Column * rRect.Width;
            var rHeight = r_Row * rRect.Height;
            var rBitmap = new RenderTargetBitmap(rWidth, rHeight, 96, 96, PixelFormats.Default);
            rBitmap.Render(rDrawingVisual);

            if (Owner.OutputToClipboard)
                ScreenshotService.Instance.OutputToClipboard(rBitmap);
            else
                ScreenshotService.Instance.OutputAsFile(rBitmap);
        }
    }
}
