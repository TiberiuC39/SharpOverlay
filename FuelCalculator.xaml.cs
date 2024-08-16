using iRacingSdkWrapper;
using SharpOverlay.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for FuelCalculator.xaml
    /// </summary>
    public partial class FuelCalculator : Window
    {
        private readonly iRacingTelemetryService _telemetryService;

        public FuelCalculator()
        {
            _telemetryService = new iRacingTelemetryService();

            this.DataContext = _telemetryService.GetFuelStatistics();
            InitializeComponent();
            _telemetryService.HookUpToTelemetryEvent(RefreshDataContext);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void RefreshDataContext(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            this.DataContext = _telemetryService.GetFuelStatistics();
        }
    }
}
