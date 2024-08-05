using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public static void UpdateEnabledStatus()
        {
            overlays[0].IsEnabled = App.appSettings.InputGraphSettings.IsEnabled;
            overlays[1].IsEnabled = App.appSettings.BarSpotterSettings.IsEnabled;
        }
    }

}
