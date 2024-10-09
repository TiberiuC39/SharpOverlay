using SharpOverlay.Services;
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
        private readonly IFuelCalculator _fuelCalculator;
        private FuelDebugWindow? _fuelDebugWindow;

        public FuelCalculatorWindow()
        {
            _fuelCalculator = new FuelCalculatorService();
            _fuelCalculator.FuelUpdated += OnFuelUpdate;

            App.appSettings.FuelSettings.PropertyChanged += HandlePropertyChange;

            JotService.tracker.Track(this);

            Topmost = true;

            InitializeComponent();
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

        private void HandlePropertyChange(object? sender, PropertyChangedEventArgs e)
        {
            if (App.appSettings.FuelSettings.TestMode)
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
