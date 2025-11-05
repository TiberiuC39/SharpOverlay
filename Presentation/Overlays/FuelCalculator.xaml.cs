using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Core.Events;
using Core.Models;
using Core.Services.FuelCalculator;
using Core.Services.FuelCalculator.Strategies;
using Presentation.Events;
using Presentation.Models;
using Presentation.Services;

namespace Presentation.Overlays
{
    /// <summary>
    /// Interaction logic for FuelCalculator.xaml
    /// </summary>
    public partial class FuelCalculatorWindow : Window
    {
        private readonly IFuelService _fuelService;
        private readonly WindowStateService _windowStateService;

        private readonly FuelSettings _settings = App.appSettings.FuelSettings;

        private FuelDebugWindow? _fuelDebugWindow;

        public FuelCalculatorWindow()
        {
            _fuelService = new FuelCalculatorService();
            _fuelService.FuelUpdated += OnFuelUpdate;

            _windowStateService = new WindowStateService(_fuelService.SimReader, _settings);
            _windowStateService.WindowStateChanged += OnWindowStateChange;
            _windowStateService.Initialize();

            JotService.tracker.Track(this);

            Topmost = true;

            InitializeComponent();
            this.DataContext = new FuelViewModel
            {
                Strategies = new ObservableCollection<StrategyViewModel>{
                    new StrategyViewModel{ Name = "FULL"},
                    new StrategyViewModel{ Name = "LAST", RefuelAmount = 1},
                    new StrategyViewModel{ Name = "5L"},
                },
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            _windowStateService.WindowStateChanged -= OnWindowStateChange;
            _windowStateService.Dispose();

            _fuelService.FuelUpdated -= OnFuelUpdate;
            _fuelService.Dispose();

            base.OnClosed(e);
        }

        private void OnWindowStateChange(object? sender, WindowStateEventArgs e)
        {
            if (e.IsOpen || e.IsInTestMode)
            {
                Show();
            }
            else
            {
                Hide();
            }

            if (e.IsInDebugMode)
            {
                if (_fuelDebugWindow is null)
                {
                    _fuelDebugWindow = new FuelDebugWindow(_fuelService);
                }

                _fuelDebugWindow.Show();
            }
            else if (_fuelDebugWindow is not null && !e.IsInDebugMode)
            {
                _fuelService.FuelUpdated -= _fuelDebugWindow!.ExecuteOnFuelUpdated;
                _fuelDebugWindow.Hide();
                _fuelDebugWindow = null;
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
            if (_fuelDebugWindow is not null)
            {
                _fuelService.FuelUpdated -= _fuelDebugWindow!.ExecuteOnFuelUpdated;
                _fuelDebugWindow.Hide();
                _fuelDebugWindow = null;
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (FontSize / e.NewSize.Height != 0.125)
            {
                FontSize = e.NewSize.Height * 0.125;
            }
        }
    }
}
