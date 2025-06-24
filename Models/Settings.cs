using System.ComponentModel;
using System.Windows.Media;
using Colors = System.Windows.Media.Colors;

namespace SharpOverlay.Models
{
    public abstract class BaseSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                OnPropertyChanged(nameof(IsOpen));
            }
        }

        private bool _isInTestMode;
        public bool IsInTestMode
        {
            get => _isInTestMode;
            set
            {
                _isInTestMode = value;
                OnPropertyChanged(nameof(IsInTestMode));
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
                OnPropertyChanged(nameof(BarWidth));
            }
        }

        public double BarLength
        {
            get => _barLength;
            set
            {
                _barLength = value;
                OnPropertyChanged(nameof(BarLength));
            }
        }

        public SolidColorBrush? BarColor
        {
            get => _barColor;
            set
            {
                _barColor = value;
                OnPropertyChanged(nameof(BarColor));
            }
        }

        public SolidColorBrush? ThreeWideBarColor
        {
            get => _threeWideBarColor;
            set
            {
                _threeWideBarColor = value;
                OnPropertyChanged(nameof(ThreeWideBarColor));
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
                OnPropertyChanged(nameof(ShowClutch));
            }
        }


        private bool _useRawValues;
        public bool UseRawValues
        {
            get => _useRawValues;
            set
            {
                _useRawValues = value;
                OnPropertyChanged(nameof(UseRawValues));
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
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }

        public SolidColorBrush? ThrottleColor

        {
            get => _throttleColor;
            set
            {
                _throttleColor = value;
                OnPropertyChanged(nameof(ThrottleColor));
            }
        }

        public SolidColorBrush? BrakeColor
        {
            get => _brakeColor;
            set
            {
                _brakeColor = value;
                OnPropertyChanged(nameof(BrakeColor));
            }
        }

        public SolidColorBrush? ClutchColor
        {
            get => _clutchColor;
            set
            {
                _clutchColor = value;
                OnPropertyChanged(nameof(ClutchColor));
            }
        }

        public SolidColorBrush? SteeringColor
        {
            get => _steeringColor;
            set
            {
                _steeringColor = value;
                OnPropertyChanged(nameof(SteeringColor));
            }
        }

        public SolidColorBrush? ABSColor
        {
            get => _ABSColor;
            set
            {
                _ABSColor = value;
                OnPropertyChanged(nameof(ABSColor));
            }
        }

        private int _lineWidth;

        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                OnPropertyChanged(nameof(LineWidth));
            }
        }

        private bool _showABS;

        public bool ShowABS
        {
            get => _showABS;
            set
            {
                _showABS = value;
                OnPropertyChanged(nameof(ShowABS));
            }
        }

        private bool _showPercentageBrake;
        public bool ShowPercentageBrake
        {
            get => _showPercentageBrake;
            set
            {
                _showPercentageBrake = value;
                OnPropertyChanged(nameof(ShowPercentageBrake));
            }
        }

        private bool _showPercentageThrottle;
        public bool ShowPercentageThrottle
        {
            get => _showPercentageThrottle;
            set
            {
                _showPercentageThrottle = value;
                OnPropertyChanged(nameof(_showPercentageThrottle));
            }
        }

        private bool _showPercentageClutch;
        public bool ShowPercentageClutch
        {
            get => _showPercentageClutch;
            set
            {
                _showPercentageClutch = value;
                OnPropertyChanged(nameof(ShowPercentageClutch));
            }
        }

        private bool _showPercentageSteering;
        public bool ShowPercentageSteering
        {
            get => _showPercentageSteering;
            set
            {
                _showPercentageSteering = value;
                OnPropertyChanged(nameof(ShowPercentageSteering));
            }
        }

        private bool _showSteering;

        public bool ShowSteering
        {
            get => _showSteering;
            set
            {
                _showSteering = value;
                OnPropertyChanged(nameof(ShowSteering));
            }
        }

        public InputGraphSettings()
        {
            ThrottleColor = new SolidColorBrush(Colors.Green);
            BrakeColor = new SolidColorBrush(Colors.Red);
            ClutchColor = new SolidColorBrush(Colors.Blue);
            SteeringColor = new SolidColorBrush(Colors.Yellow);
            UseRawValues = true;
            ShowClutch = true;
            ShowPercentageThrottle = true;
            ShowPercentageBrake = true;
            ShowPercentageClutch = true;
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
                OnPropertyChanged(nameof(UseMph));
            }
        }
    }

    public class FuelSettings : BaseSettings
    {
        private bool _IsInPositioningMode;

        public bool IsInPositioningMode
        {
            get => _IsInPositioningMode;
            set
            {
                _IsInPositioningMode = value;
                OnPropertyChanged(nameof(IsInPositioningMode));
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

    public class Settings : INotifyPropertyChanged
    {
        public GeneralSettings GeneralSettings { get; set; }
        public BarSpotterSettings BarSpotterSettings { get; set; }
        public InputGraphSettings InputGraphSettings { get; set; }
        public WindSettings WindSettings { get; set; }
        public FuelSettings FuelSettings { get; set; }
        public bool IsUpdate { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

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

