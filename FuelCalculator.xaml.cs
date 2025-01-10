using SharpOverlay.Events;
using SharpOverlay.Models;
using SharpOverlay.Services;
using SharpOverlay.Services.Base;
using SharpOverlay.Services.FuelServices;
using SharpOverlay.Utilities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for FuelCalculator.xaml
    /// </summary>
    public partial class FuelCalculatorWindow : Window
    {
        private readonly FuelSettings _settings = App.appSettings.FuelSettings;

        private readonly IFuelCalculator _fuelCalculator;
        private readonly SimReader _simReader = new(DefaultTickRates.FuelCalculator);
        private readonly WindowStateService _windowStateService;
        private FuelDebugWindow? _fuelDebugWindow;

        public FuelCalculatorWindow()
        {
            _windowStateService = new WindowStateService(_simReader, _settings);
            _fuelCalculator = new FuelCalculatorService(_simReader);

            _fuelCalculator.FuelUpdated += OnFuelUpdate;

            _windowStateService.WindowStateChanged += _windowStateService_WindowStateChanged;

            _settings.PropertyChanged += OnPropertyChange;

            JotService.tracker.Track(this);

            Topmost = true;

            InitializeComponent();
        }

        private void _windowStateService_WindowStateChanged(object? sender, WindowStateEventArgs e)
        {
            if (e.IsOpen && e.IsEnabled)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Window_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void OnFuelUpdate(object? sender, FuelEventArgs e)
        {
            this.DataContext = e.ViewModel;
        }

        private void OnPropertyChange(object? sender, PropertyChangedEventArgs e)
        {
            if (_settings.IsInTestMode)
            {
                _fuelDebugWindow = new FuelDebugWindow(_fuelCalculator);
                _fuelDebugWindow.Show();
            }
            else if (_fuelDebugWindow is not null)
            {
                _fuelCalculator.FuelUpdated -= _fuelDebugWindow!.ExecuteOnFuelUpdated;
                _fuelDebugWindow.Hide();
                _fuelDebugWindow = null;
            }
        }
    }
}
