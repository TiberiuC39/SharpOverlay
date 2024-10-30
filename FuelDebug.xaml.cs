using SharpOverlay.Services;
using System;
using System.Windows;
using System.Windows.Input;

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
