﻿using SharpOverlay.Models;
using Jot;
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
                .Property(p => p.InputGraphSettings.ShowClutch, false)
                .Property(p => p.InputGraphSettings.BackgroundColor, new SolidColorBrush(Color.FromArgb(80, 0, 0, 0)))
                .Property(p => p.InputGraphSettings.ThrottleColor, new SolidColorBrush(Colors.Green))
                .Property(p => p.InputGraphSettings.BrakeColor, new SolidColorBrush(Colors.Red))
                .Property(p => p.InputGraphSettings.ClutchColor, new SolidColorBrush(Colors.Blue))
                .Property(p => p.InputGraphSettings.ABSColor, new SolidColorBrush(Colors.Yellow))
                .Property(p => p.InputGraphSettings.SteeringColor, new SolidColorBrush(Colors.Gray))
                .Property(p => p.InputGraphSettings.ShowABS, false)
                .Property(p => p.InputGraphSettings.LineWidth, 3)
                .Property(p => p.InputGraphSettings.ShowPercentageThrottle, false)
                .Property(p => p.InputGraphSettings.ShowPercentageBrake, false)
                .Property(p => p.InputGraphSettings.ShowPercentageClutch, false)
                .Property(p => p.WindSettings.IsEnabled, false)
                .Property(p => p.WindSettings.UseMph, false)
              
                .Property(p => p.FuelSettings.IsEnabled, false)
               
                .Property(p => p.GeneralSettings.UseHardwareAcceleration, false)
                .PersistOn("PropertyChanged", p => p.GeneralSettings)

                .PersistOn("PropertyChanged", p => p.BarSpotterSettings)
                .PersistOn("PropertyChanged", p => p.InputGraphSettings)
                .PersistOn("PropertyChanged", p => p.WindSettings)
                .PersistOn("PropertyChanged", p => p.FuelSettings);
                
        }
    }
}
