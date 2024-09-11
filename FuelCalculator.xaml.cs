using SharpOverlay.Services;
using System.Windows;
using System.Windows.Input;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for FuelCalculator.xaml
    /// </summary>
    public partial class FuelCalculatorWindow : Window
    {
        private readonly FuelCalculatorService _fuelService;

        public FuelCalculatorWindow()
        {
            _fuelService = new FuelCalculatorService();

            InitializeComponent();
            _fuelService.FuelUpdated += OnFuelUpdate;
            Topmost = true;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void OnFuelUpdate(object? sender, FuelEventArgs e)
        {
            this.DataContext = e.ViewModel;
        }
    }
}
