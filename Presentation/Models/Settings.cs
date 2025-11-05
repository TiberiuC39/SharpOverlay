using System.ComponentModel;
using System.Windows.Media;
using Colors = System.Windows.Media.Colors;

namespace Presentation.Models
{
    public interface IBaseSettings : INotifyPropertyChanged
    {
        bool IsEnabled { get; set; }
        bool IsOpen { get; set; }
        bool IsInTestMode { get; set; }
    }

    public abstract class BaseSettings : IBaseSettings
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                bool prevState = _isEnabled;
                _isEnabled = value;
                if (prevState != value)
                {
                    MainWindow.HandleOverlayStatus();
                }
                // OnPropertyChange(nameof(IsEnabled));
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                OnPropertyChange(nameof(IsOpen));
            }
        }

        private bool _isInTestMode;
        public bool IsInTestMode
        {
            get => _isInTestMode;
            set
            {
                _isInTestMode = value;
                OnPropertyChange(nameof(IsInTestMode));
            }
        }

        private bool _isInDebugMode;
        public bool IsInDebugMode
        {
            get => _isInDebugMode;
            set
            {
                _isInDebugMode = value;
                OnPropertyChange(nameof(IsInDebugMode));
            }
        }
    }

    public class BarSpotterSettings : BaseSettings
    {

        private double _barWidth;

        private double _barLength;

        private SolidColorBrush? _barColor;

        private SolidColorBrush? _threeWideBarColor;


        public double BarWidth
        {
            get => _barWidth;
            set
            {
                _barWidth = value;
                OnPropertyChange(nameof(BarWidth));
            }
        }

        public double BarLength
        {
            get => _barLength;
            set
            {
                _barLength = value;
                OnPropertyChange(nameof(BarLength));
            }
        }

        public SolidColorBrush? BarColor
        {
            get => _barColor;
            set
            {
                _barColor = value;
                OnPropertyChange(nameof(BarColor));
            }
        }

        public SolidColorBrush? ThreeWideBarColor
        {
            get => _threeWideBarColor;
            set
            {
                _threeWideBarColor = value;
                OnPropertyChange(nameof(ThreeWideBarColor));
            }
        }
    }

    public class InputGraphSettings : BaseSettings
    {
        private bool _showClutch;
        public bool ShowClutch
        {
            get => _showClutch;
            set
            {
                _showClutch = value;
                OnPropertyChange(nameof(ShowClutch));
            }
        }


        private bool _useRawValues;
        public bool UseRawValues
        {
            get => _useRawValues;
            set
            {
                _useRawValues = value;
                OnPropertyChange(nameof(UseRawValues));
            }
        }

        private SolidColorBrush? _backgroundColor;

        private SolidColorBrush? _throttleColor;

        private SolidColorBrush? _brakeColor;

        private SolidColorBrush? _clutchColor;

        private SolidColorBrush? _steeringColor;

        private SolidColorBrush? _ABSColor;

        public SolidColorBrush? BackgroundColor

        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChange(nameof(BackgroundColor));
            }
        }

        public SolidColorBrush? ThrottleColor

        {
            get => _throttleColor;
            set
            {
                _throttleColor = value;
                OnPropertyChange(nameof(ThrottleColor));
            }
        }

        public SolidColorBrush? BrakeColor
        {
            get => _brakeColor;
            set
            {
                _brakeColor = value;
                OnPropertyChange(nameof(BrakeColor));
            }
        }

        public SolidColorBrush? ClutchColor
        {
            get => _clutchColor;
            set
            {
                _clutchColor = value;
                OnPropertyChange(nameof(ClutchColor));
            }
        }

        public SolidColorBrush? SteeringColor
        {
            get => _steeringColor;
            set
            {
                _steeringColor = value;
                OnPropertyChange(nameof(SteeringColor));
            }
        }

        public SolidColorBrush? ABSColor
        {
            get => _ABSColor;
            set
            {
                _ABSColor = value;
                OnPropertyChange(nameof(ABSColor));
            }
        }

        private int _lineWidth;

        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                OnPropertyChange(nameof(LineWidth));
            }
        }

        private bool _showABS;

        public bool ShowABS
        {
            get => _showABS;
            set
            {
                _showABS = value;
                OnPropertyChange(nameof(ShowABS));
            }
        }

        private bool _showPercentageBrake;
        public bool ShowPercentageBrake
        {
            get => _showPercentageBrake;
            set
            {
                _showPercentageBrake = value;
                OnPropertyChange(nameof(ShowPercentageBrake));
            }
        }

        private bool _showPercentageThrottle;
        public bool ShowPercentageThrottle
        {
            get => _showPercentageThrottle;
            set
            {
                _showPercentageThrottle = value;
                OnPropertyChange(nameof(_showPercentageThrottle));
            }
        }

        private bool _showPercentageClutch;
        public bool ShowPercentageClutch
        {
            get => _showPercentageClutch;
            set
            {
                _showPercentageClutch = value;
                OnPropertyChange(nameof(ShowPercentageClutch));
            }
        }

        private bool _showPercentageSteering;
        public bool ShowPercentageSteering
        {
            get => _showPercentageSteering;
            set
            {
                _showPercentageSteering = value;
                OnPropertyChange(nameof(ShowPercentageSteering));
            }
        }

        private bool _showSteering;

        public bool ShowSteering
        {
            get => _showSteering;
            set
            {
                _showSteering = value;
                OnPropertyChange(nameof(ShowSteering));
            }
        }

        public InputGraphSettings()
        {
            ThrottleColor = new SolidColorBrush(Colors.Green);
            BrakeColor = new SolidColorBrush(Colors.Red);
            ClutchColor = new SolidColorBrush(Colors.Blue);
            SteeringColor = new SolidColorBrush(Colors.Gray);
            ABSColor = new SolidColorBrush(Colors.Yellow);
            UseRawValues = true;
            ShowClutch = false;
            ShowPercentageThrottle = false;
            ShowPercentageBrake = false;
            ShowPercentageClutch = false;
        }
    }

    public class WindSettings : BaseSettings
    {
        private bool _useMph;

        public bool UseMph
        {
            get => _useMph;
            set
            {
                _useMph = value;
                OnPropertyChange(nameof(UseMph));
            }
        }
    }

    public class FuelSettings : BaseSettings
    {
        private SolidColorBrush? _backgroundColor;

        public SolidColorBrush? BackgroundColor

        {
            get => _backgroundColor;
            set
            {
                _backgroundColor = value;
                OnPropertyChange(nameof(BackgroundColor));
            }
        }
    }

    public class GeneralSettings : INotifyPropertyChanged
    {
        private bool _useHardwareAcceleration;

        public bool UseHardwareAcceleration
        {
            get => _useHardwareAcceleration;
            set
            {
                _useHardwareAcceleration = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UseHardwareAcceleration)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class Settings
    {
        public GeneralSettings GeneralSettings { get; set; }
        public BarSpotterSettings BarSpotterSettings { get; set; }
        public InputGraphSettings InputGraphSettings { get; set; }
        public WindSettings WindSettings { get; set; }
        public FuelSettings FuelSettings { get; set; }
        public bool IsUpdate { get; set; }

        public Settings()
        {
            GeneralSettings = new GeneralSettings();
            InputGraphSettings = new InputGraphSettings();
            BarSpotterSettings = new BarSpotterSettings();
            WindSettings = new WindSettings();
            FuelSettings = new FuelSettings();
        }
    }
}

