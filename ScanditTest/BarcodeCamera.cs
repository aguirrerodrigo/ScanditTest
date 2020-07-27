using System.Windows.Input;
using Xamarin.Forms;

namespace ScanditTest
{
    public class BarcodeCamera : View
    {
        public string ApiKey { get; private set; } = BarcodeCameraSettings.ApiKey;

        public static readonly BindableProperty PanelProperty = BindableProperty.Create(nameof(Panel), typeof(string), typeof(BarcodeCamera), "Rear");
        public string Panel
        {
            get { return (string)GetValue(PanelProperty); }
            set { SetValue(PanelProperty, value); }
        }

        public static readonly BindableProperty ScannedProperty = BindableProperty.Create(nameof(Scanned), typeof(ICommand), typeof(BarcodeCamera));
        public ICommand Scanned
        {
            get { return (ICommand)GetValue(ScannedProperty); }
            set { SetValue(ScannedProperty, value); }
        }

        public static readonly BindableProperty ScanningEnabledProperty = BindableProperty.Create(nameof(ScanningEnabled), typeof(bool), typeof(BarcodeCamera));
        public bool ScanningEnabled
        {
            get { return (bool)GetValue(ScanningEnabledProperty); }
            set { SetValue(ScanningEnabledProperty, value); }
        }

        public static readonly BindableProperty CameraEnabledProperty = BindableProperty.Create(nameof(CameraEnabled), typeof(bool), typeof(BarcodeCamera));
        public bool CameraEnabled
        {
            get { return (bool)GetValue(CameraEnabledProperty); }
            set { SetValue(CameraEnabledProperty, value); }
        }

        public BarcodeCamera()
        {
        }
    }
}
