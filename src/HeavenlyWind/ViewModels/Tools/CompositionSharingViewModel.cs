using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.SystemInterop;
using Sakuno.SystemInterop.Dialogs;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Tools
{
    class CompositionSharingViewModel : ModelBase
    {
        public IList<FleetViewModel> Fleets { get; }

        public CompositionSharingViewModel()
        {
            Fleets = KanColleGame.Current.Port.Fleets.Select(r => new FleetViewModel(r)).ToArray();
        }

        public void TakeScreenshot(object rpTabControl)
        {
            var rTabControl = rpTabControl as Control;
            if (rTabControl == null)
                return;

            var rContent = rTabControl.Template.FindName("PART_SelectedContentHost", rTabControl) as ContentPresenter;
            if (rContent == null)
                return;

            var rRenderTargetBitmap = new RenderTargetBitmap((int)(rContent.ActualWidth * DpiUtil.ScaleX), (int)(rContent.ActualHeight * DpiUtil.ScaleY), DpiUtil.DpiX, DpiUtil.DpiY, PixelFormats.Pbgra32);
            rRenderTargetBitmap.Render(rContent);

            var rScreenshotFilename = string.Empty;

            var rOutputToFile = Preference.Instance.Game.CompositionSharing_OutputScreenshotToFile.Value;
            if (!rOutputToFile)
                Clipboard.SetImage(rRenderTargetBitmap);
            else
            {
                using (var rSaveDialog = new CommonSaveFileDialog())
                {
                    rSaveDialog.FileTypes.Add(new CommonFileDialogFileType("PNG", "png"));

                    if (rSaveDialog.Show(WindowUtil.GetTopWindow()) == CommonFileDialogResult.Cancel)
                        return;

                    rScreenshotFilename = rSaveDialog.Filename;

                    if (!rScreenshotFilename.OICEndsWith(".png"))
                        rScreenshotFilename += ".png";
                }

                using (var rFile = File.OpenWrite(rScreenshotFilename))
                {
                    var rEncoder = new PngBitmapEncoder() { Frames = { BitmapFrame.Create(rRenderTargetBitmap) } };
                    rEncoder.Save(rFile);
                }
            }

            var rDialog = new TaskDialog()
            {
                Caption = StringResources.Instance.Main.Window_CompositionSharing,
                Instruction = StringResources.Instance.Main.CompositionSharing_TakeScreenshot_Clipboard_Message,
                Icon = TaskDialogIcon.Information,

                OwnerWindow = WindowUtil.GetTopWindow(),
                ShowAtTheCenterOfOwner = true,
            };

            if (!rOutputToFile)
                rDialog.Instruction = StringResources.Instance.Main.CompositionSharing_TakeScreenshot_Clipboard_Message;
            else
            {
                rDialog.Instruction = StringResources.Instance.Main.CompositionSharing_TakeScreenshot_File_Message;
                rDialog.EnableHyperlinks = true;
                rDialog.FooterIcon = TaskDialogIcon.Information;
                rDialog.Footer = $"<a href=\"{rScreenshotFilename}\">{rScreenshotFilename}</a>";

                EventHandler<string> rHyperlinkClicked = null;
                rHyperlinkClicked = delegate
                {
                    if (File.Exists(rScreenshotFilename))
                        Process.Start(rScreenshotFilename);
                };
                EventHandler rClosed = null;
                rClosed = delegate
                {
                    rDialog.HyperlinkClicked -= rHyperlinkClicked;
                    rDialog.Closed -= rClosed;
                };
                rDialog.HyperlinkClicked += rHyperlinkClicked;
                rDialog.Closed += rClosed;
            }

            rDialog.ShowAndDispose();
        }

        public void GenerateCode()
        {
            var rText = GenerateCodeCore();

            if (Preference.Instance.Game.CompositionSharing_AutoOpenBrowser.Value)
            {
                Process.Start("https://noro6.github.io/kc-web?predeck=" + rText);
                return;
            }

            Clipboard.SetDataObject(rText);

            var rDialog = new TaskDialog()
            {
                Caption = StringResources.Instance.Main.Window_CompositionSharing,
                Instruction = StringResources.Instance.Main.CompositionSharing_GenerateCode_CopyToClipboard_Message,
                Icon = TaskDialogIcon.Information,

                OwnerWindow = WindowUtil.GetTopWindow(),
                ShowAtTheCenterOfOwner = true,
            };

            rDialog.ShowAndDispose();
        }
        string GenerateCodeCore()
        {
            var rResult = new JObject() { ["version"] = 4 };

            var rFleetID = 1;

            foreach (var rFleetData in Fleets)
            {
                if (!rFleetData.IsSelected)
                    continue;

                var rFleet = new JObject();

                for (var i = 0; i < rFleetData.Source.Ships.Count; i++)
                {
                    var rShipData = rFleetData.Source.Ships[i];

                    var rSlots = new JObject();
                    var rShip = new JObject()
                    {
                        ["id"] = rShipData.Info.ID,
                        ["lv"] = rShipData.Level,
                        ["luck"] = rShipData.Status.Luck,
                        ["items"] = rSlots,
                    };

                    for (var j = 0; j < rShipData.Slots.Count; j++)
                    {
                        var rEquipmentData = rShipData.Slots[j].Equipment;

                        var rEquipment = new JObject()
                        {
                            ["id"] = rEquipmentData.Info.ID,
                            ["rf"] = rEquipmentData.Level,
                        };

                        if (rEquipmentData.Proficiency > 0)
                            rEquipment["mas"] = rEquipmentData.Proficiency;

                        rSlots["i" + (j + 1)] = rEquipment;
                    }

                    if (rShipData.ExtraSlot?.Equipment != null)
                    {
                        var rEquipmentData = rShipData.ExtraSlot.Equipment;

                        var rEquipment = new JObject()
                        {
                            ["id"] = rEquipmentData.Info.ID,
                            ["rf"] = rEquipmentData.Level,
                        };

                        if (rEquipmentData.Proficiency > 0)
                            rEquipment["mas"] = rEquipmentData.Proficiency;

                        rSlots[rShipData.Slots.Count == 4 ? "ix" : "i" + (rShipData.Slots.Count + 1)] = rEquipment; // WTF???
                    }

                    rFleet["s" + (i + 1)] = rShip;
                }

                rResult["f" + rFleetID++] = rFleet;
            }

            return rResult.ToString(Formatting.None);
        }

        public class FleetViewModel : ModelBase
        {
            public Fleet Source { get; }

            bool r_IsSelected;
            public bool IsSelected
            {
                get { return r_IsSelected; }
                set
                {
                    if (r_IsSelected != value)
                    {
                        r_IsSelected = value;
                        OnPropertyChanged();
                    }
                }
            }

            public FleetViewModel(Fleet rpFleet)
            {
                Source = rpFleet;

                if (rpFleet.ID == 1)
                    r_IsSelected = true;
            }
        }
    }
}
