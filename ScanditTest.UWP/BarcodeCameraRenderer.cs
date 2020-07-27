using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Scandit.BarcodePicker;
using Scandit.Recognition;
using ScanditTest;
using ScanditTest.UWP;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.System.Display;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(BarcodeCamera), typeof(BarcodeCameraRenderer))]
namespace ScanditTest.UWP
{
    public class BarcodeCameraRenderer : ViewRenderer<BarcodeCamera, BarcodePicker>
    {
        private BarcodePicker barcodePicker;
        private BarcodeCamera barcodeCamera;
        private readonly DisplayRequest displayRequest = new DisplayRequest();

        /// <summary>
        /// Raises the <see cref="E:ElementChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="ElementChangedEventArgs{CameraPreview}"/> instance containing the event data.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<BarcodeCamera> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null && Control == null)
            {
                displayRequest.RequestActive();
                barcodeCamera = e.NewElement;
                barcodePicker = GetBarcodePicker();
                barcodePicker.DidScan += Picker_DidScan;

                if (e.NewElement.CameraEnabled)
                {
                    _ = StartCameraAsync();
                }

                SetNativeControl(barcodePicker);
            }
            else
            {
                displayRequest.RequestRelease();
                barcodePicker.StopScanningAndCloseCameraAsync();
            }
        }

        /// <summary>
        /// Called when [element property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "ScanningEnabled")
            {
                BarcodeCamera barcodeCam = sender as BarcodeCamera;
                if (barcodeCam.ScanningEnabled)
                {
                    _ = barcodePicker.ResumeScanningAsync();
                }
                else
                {
                    _ = barcodePicker.PauseScanningAsync();
                }
            }
            else if (e.PropertyName == "CameraEnabled")
            {
                BarcodeCamera barcodeCam = sender as BarcodeCamera;
                if (barcodeCam.CameraEnabled)
                {
                    _ = barcodePicker.OpenCameraAndStartScanningAsync();
                }
                else
                {
                    _ = barcodePicker.StopScanningAndCloseCameraAsync();
                }
            }
        }

        private BarcodePicker GetBarcodePicker()
        {
            ScanSettings settings = new ScanSettings();
            settings.EnableSymbology(BarcodeSymbology.Qr);
            settings.EnableSymbology(BarcodeSymbology.Code39);
            settings.EnableSymbology(BarcodeSymbology.Code128);

            settings.Symbologies[BarcodeSymbology.Code128].ActiveSymbolCounts = new ushort[]
            {
                7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23
            };

            settings.Symbologies[BarcodeSymbology.Code39].ActiveSymbolCounts = new ushort[]
            {
                7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23,
            };

            settings.CodeDuplicateFilter = 5000;
            Size viewFinderSize = new Size();
            viewFinderSize.Height = 0.32;
            viewFinderSize.Width = 0.75;

            BarcodePicker picker = new BarcodePicker(barcodeCamera.ApiKey, settings)
            {
                BeepEnabled = false,
                ScanOverlay = new ScanOverlay()
                {
                    GuiStyle = GuiStyle.Rectangle,
                    ViewFinderSizePortrait = viewFinderSize,
                    ViewFinderSizeLandscape = viewFinderSize
                }
            };

            return picker;
        }

        private void Picker_DidScan(ScanSession session)
        {
            foreach (Barcode barcode in session.NewlyRecognizedCodes)
            {
                barcodeCamera.Scanned?.Execute(barcode.Data);
                session.RejectCode(barcode);
            }
        }

        private async Task StartCameraAsync()
        {
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (devices.Count == 0)
            {
                return;
            }

            DeviceInformation camera = GetDesired(devices);
            await barcodePicker.OpenCameraAndStartScanningAsync(camera);
        }

        private DeviceInformation GetDesired(DeviceInformationCollection devices)
        {
            Enum.TryParse(barcodeCamera.Panel, true, out Panel panel);
            //return devices.FirstOrDefault(IsEnclosure(panel))
            //    ?? devices.FirstOrDefault(IsEnclosureUnknown())
            //    ?? devices[0];
            return devices[0];
        }

        private static Func<DeviceInformation, bool> IsEnclosureUnknown()
        {
            return d => d.EnclosureLocation is null || d.EnclosureLocation.Panel == Panel.Unknown;
        }

        private static Func<DeviceInformation, bool> IsEnclosure(Panel panel)
        {
            return d => d.EnclosureLocation?.Panel == panel;
        }
    }
}
