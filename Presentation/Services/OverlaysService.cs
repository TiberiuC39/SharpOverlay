using Presentation.Models;
using Presentation.Overlays;

namespace Presentation.Services
{
    public static class OverlaysService
    {
        public static IList<Overlay> Overlays { get; private set; }
        static OverlaysService()
        {
            Overlays = new List<Overlay>();
            Overlays.Add(new Overlay(typeof(InputGraph), App.appSettings.InputGraphSettings.IsEnabled, false));
            Overlays.Add(new Overlay(typeof(BarSpotter), App.appSettings.BarSpotterSettings.IsEnabled, false));
            Overlays.Add(new Overlay(typeof(Wind), App.appSettings.WindSettings.IsEnabled, false));
            Overlays.Add(new Overlay(typeof(FuelCalculatorWindow), App.appSettings.FuelSettings.IsEnabled, false));
        }

        public static void UpdateEnabledStatus()
        {
            Overlays[0].IsEnabled = App.appSettings.InputGraphSettings.IsEnabled;
            Overlays[0].IsEnabled = App.appSettings.BarSpotterSettings.IsEnabled;
            Overlays[2].IsEnabled = App.appSettings.WindSettings.IsEnabled;
            Overlays[3].IsEnabled = App.appSettings.FuelSettings.IsEnabled;
        }
    }

}
