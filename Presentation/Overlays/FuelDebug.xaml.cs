using System.Windows;
using Core.Events;
using Core.Services.FuelCalculator;

namespace Presentation.Overlays
{
    /// <summary>
    /// Interaction logic for FuelDebugWindow.xaml
    /// </summary>
    public partial class FuelDebugWindow : Window
    {
        private readonly IFuelService _service;
        public FuelDebugWindow(IFuelService dataService)
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
