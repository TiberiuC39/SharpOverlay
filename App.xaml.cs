using SharpOverlay.Models;
using SharpOverlay.Services;
using System;
using System.Windows;
using System.Windows.Media;
using Velopack;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       public static Settings appSettings = new Settings();

        [STAThread]
        private static void Main(string[] args)
        {
            VelopackApp.Build().Run();

            JotService.tracker.Apply(appSettings);

            if (appSettings.GeneralSettings.UseHardwareAcceleration) {
                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
            }
            else
            {
                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            }

            App app = new();
            app.InitializeComponent();
            app.Run();

        }


    }
}
