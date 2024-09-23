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
        private readonly IFuelCalculator _fuelCalculator;

        public FuelCalculatorWindow()
        {
            _fuelCalculator = new FuelCalculatorService();
            _fuelCalculator.FuelUpdated += OnFuelUpdate;
            
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
    }
}
