using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Dark.Net;
using Presentation.Models;
using Presentation.Models.ViewModels;
using Presentation.Services;
using Velopack;
using Velopack.Sources;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UpdateManager mgr = new UpdateManager(new GithubSource("https://github.com/tiberiuc39/sharpoverlay", null, false));

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();

            JotService.tracker.Track(App.appSettings);
            JotService.tracker.PersistAll();
            DarkNet.Instance.SetWindowThemeWpf(this, Theme.Auto);
            InitializeComponent();
            HandleOverlayStatus();
            if (App.appSettings.IsUpdate)
            {
                updateButton.Visibility = Visibility.Visible;
            }
        }

        private async Task CheckForUpdate()
        {
            if (mgr.IsInstalled)
            {
                // check for new version
                var newVersion = await mgr.CheckForUpdatesAsync();
                if (newVersion == null)
                    return; // no update available

                updateButton.Visibility = Visibility.Visible;
            }
        }

        private async Task UpdateApp()
        {
            var newVersion = await mgr.CheckForUpdatesAsync();
            await mgr.DownloadUpdatesAsync(newVersion!);

            // install new version and restart app
            mgr.ApplyUpdatesAndRestart(newVersion!);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckForUpdate();
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            await UpdateApp();
        }

        public static void HandleOverlayStatus()
        {
            OverlaysService.UpdateEnabledStatus();
            foreach (Overlay o in OverlaysService.Overlays)
            {
                if (o.IsEnabled && o.Window is null)
                {
                    o.Window = (Window)Activator.CreateInstance(o.Type)!;
                }
                else if (!o.IsEnabled && o.Window is not null)
                {
                    o.Window.Close();
                    o.Window = null;
                }
            }
        }

        // Minimizes the window when the minimize button is clicked
        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        // Closes the application when the close button is clicked
        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        // Makes the window draggable when clicking and holding on the custom title bar
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var sInfo = new ProcessStartInfo(e.Uri.ToString())
            {
                UseShellExecute = true,
            };

            Process.Start(sInfo);
        }
    }
}
