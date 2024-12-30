using SharpOverlay.Services.FuelServices;
using System.Windows;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for FuelDebugWindow.xaml
    /// </summary>
    public partial class FuelDebugWindow : Window
    {
        private readonly IFuelCalculator _service;
        public FuelDebugWindow(IFuelCalculator dataService)
        {
            _service = dataService;

            _service.FuelUpdated += ExecuteOnFuelUpdated;

            Topmost = true;

            InitializeComponent();
        }

        public void ExecuteOnFuelUpdated(object? sender, FuelEventArgs e)
        {
            DataContext = e.ViewModel;
        }
    }
}
