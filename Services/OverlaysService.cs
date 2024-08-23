using SharpOverlay.Models;
using System.Collections.Generic;

namespace SharpOverlay.Services
{
    public static class OverlaysService
    {
        public static IList<Overlay> overlays { get; private set; }
        static OverlaysService()
        {
            overlays = new List<Overlay>();
            overlays.Add(new Overlay(typeof(InputGraph), App.appSettings.InputGraphSettings.IsEnabled, false));
            overlays.Add(new Overlay(typeof(BarSpotter), App.appSettings.BarSpotterSettings.IsEnabled, false));
            overlays.Add(new Overlay(typeof(Wind), App.appSettings.WindSettings.IsEnabled, false));
            overlays.Add(new Overlay(typeof(FuelCalculatorWindow), App.appSettings.FuelSettings.IsEnabled, false));
        }

        public static void UpdateEnabledStatus()
        {
            overlays[0].IsEnabled = App.appSettings.InputGraphSettings.IsEnabled;
            overlays[1].IsEnabled = App.appSettings.BarSpotterSettings.IsEnabled;
            overlays[2].IsEnabled = App.appSettings.WindSettings.IsEnabled;
            overlays[3].IsEnabled = App.appSettings.FuelSettings.IsEnabled;
        }
    }

}
