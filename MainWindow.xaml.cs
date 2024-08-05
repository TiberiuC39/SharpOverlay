
using HandyControl.Controls;
using HandyControl.Tools;
using SharpOverlay.Models;
using SharpOverlay.Services;
using iRacingSdkWrapper;
using MaterialDesignThemes.Wpf;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static MaterialDesignThemes.Wpf.Theme.ToolBar;
using System.Reflection;
using Window = System.Windows.Window;
using Dark.Net;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public SdkWrapper iracingWrapper = iRacingDataService.Wrapper;
        public UpdateManager mgr = new UpdateManager(new GithubSource("https://github.com/tiberiuc39/sharpoverlay", null, false));
        public MainWindow()
        {
            ConfigHelper.Instance.SetLang("en");
            Services.JotService.tracker.Track(App.appSettings);
            Services.JotService.tracker.PersistAll();
            DarkNet.Instance.SetWindowThemeWpf(this, Dark.Net.Theme.Auto);
            InitializeComponent();
            HandleOverlayStatus();
            this.DataContext = App.appSettings;
            iracingWrapper.Start();
            if(App.appSettings.IsUpdate)
            {
                updateButton.Visibility = Visibility.Visible;  
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
           await CheckForUpdate();
        }
        private async Task CheckForUpdate()
        {
            // check for new version
            var newVersion = mgr.CheckForUpdatesAsync().Result;
            if (newVersion == null)
                return; // no update available

            updateButton.Visibility = Visibility.Visible;

        }
        private async Task UpdateApp()
        {
            var newVersion = await mgr.CheckForUpdatesAsync();
            await mgr.DownloadUpdatesAsync(newVersion);

            // install new version and restart app
            mgr.ApplyUpdatesAndRestart(newVersion);
        }
        public void HandleOverlayStatus()
        {
            OverlaysService.UpdateEnabledStatus();
            foreach (Overlay o in OverlaysService.overlays)
            {
                if (o.IsEnabled && !o.IsOpen)
                {
                    o.Window = (Window)Activator.CreateInstance(o.Type);
                    var showMethod = o.Window.GetType().GetMethod("Show");
                    showMethod.Invoke(o.Window, null);
                    o.IsOpen = true;
                }
                if (!o.IsEnabled && o.IsOpen)
                {
                    var closeMethod = o.Window.GetType().GetMethod("Close");
                    closeMethod.Invoke(o.Window, null);
                    o.Window = null;
                    o.IsOpen = false;
                }
            }
        }


        private void showColorPicker(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var picker = SingleOpenHelper.CreateControl<HandyControl.Controls.ColorPicker>();
            picker.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(95, 64, 64, 64));
            var window = new PopupWindow
            {
                PopupElement = picker
            };
            picker.Confirmed += delegate { button.Background = picker.SelectedBrush; window.Close(); };
            picker.Canceled += delegate { window.Close(); };
            window.Show(button, false);

        }

        private void windowToggle(object sender, RoutedEventArgs e)
        {
            HandleOverlayStatus();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var sInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        private async void updateButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateApp();
        }
    }
}

