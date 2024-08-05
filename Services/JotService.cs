using SharpOverlay.Models;
using Jot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SharpOverlay.Services
{
    public static class JotService
    {
        public static Tracker tracker = new Tracker();

        static JotService()
        {
            tracker.Configure<Window>()
            .Id(w => w.Name)
            .Properties(w => new { w.Top, w.Width, w.Height, w.Left, w.WindowState })
            .PersistOn(nameof(Window.Closing))
            .StopTrackingOn(nameof(Window.Closing));

            tracker.Configure<Settings>()
                .Property(p => p.BarSpotterSettings.IsEnabled, false)
                .Property(p => p.BarSpotterSettings.BarWidth, 20)
                .Property(p => p.BarSpotterSettings.BarLength, 450)
                .Property(p => p.BarSpotterSettings.BarColor, new SolidColorBrush(Colors.Orange))
                .Property(p => p.BarSpotterSettings.ThreeWideBarColor, new SolidColorBrush(Colors.Red))
                .Property(p => p.InputGraphSettings.IsEnabled, false)
                .Property(p => p.InputGraphSettings.UseRawValues, true)
                .Property(p => p.InputGraphSettings.ShowClutch, true)
                .Property(p => p.InputGraphSettings.ThrottleColor, new SolidColorBrush(Colors.Green))
                .Property(p => p.InputGraphSettings.BrakeColor, new SolidColorBrush(Colors.Red))
                .Property(p => p.InputGraphSettings.ClutchColor, new SolidColorBrush(Colors.Blue))
                .Property(p => p.InputGraphSettings.LineWidth, 3)
                .PersistOn("PropertyChanged", p => p.BarSpotterSettings)
                .PersistOn("PropertyChanged", p => p.InputGraphSettings);
                
        }
    }
}
