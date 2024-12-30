using iRacingSdkWrapper;
using SharpOverlay.Models;
using SharpOverlay.Services.Base;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for Wind.xaml
    /// </summary>
    public partial class Wind : Window
    {
        private const int _defaultTickRate = 10;
        private readonly SimReader _simReader = new SimReader(_defaultTickRate);
        private Settings appSettings = App.appSettings;

        private readonly Color StartColor = Color.FromArgb(255, 0, 255, 0);
        private readonly Color CenterColor = Color.FromArgb(255, 255, 255, 0);
        private readonly Color EndColor = Color.FromArgb(255, 255, 0, 0);

        private double windDir;
        private float yawNorth;
        private float windSpeed;
        private float lastWindSpeed = 999;
        public Wind()
        {
            InitializeComponent();
            Services.JotService.tracker.Track(this);

            _simReader.OnTelemetryUpdated += Wrapper_TelemetryUpdated;
            appSettings.WindSettings.PropertyChanged += settings_TestMode;

            WindSpeedLabel.Foreground = new SolidColorBrush(StartColor);
            WindDirIcon.Foreground = new SolidColorBrush(StartColor);
        }

        private void settings_TestMode(object? sender, PropertyChangedEventArgs e)
        {
            if (appSettings.WindSettings.IsInTestMode)
            {
                WindWindow.BorderThickness = new Thickness(5);
            }
            else
            {
                WindWindow.BorderThickness = new Thickness(0);
            }
        }

        private void Wrapper_TelemetryUpdated(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {

            windDir = (e.TelemetryInfo.WindDir.Value + Math.PI / 180);
            yawNorth = e.TelemetryInfo.YawNorth.Value;
            windSpeed = e.TelemetryInfo.WindVel.Value * 3.6f;

            double finalAngle = (windDir - yawNorth) * 57.2958 + 180;
            ((RotateTransform)((TransformGroup)WindDirIcon.RenderTransform).Children[0]).Angle = finalAngle;
            if (Math.Abs(windSpeed - lastWindSpeed) > 1)
            {
                lastWindSpeed = windSpeed;
                SetColor(windSpeed / 40);
                if (appSettings.WindSettings.UseMph)
                {
                    WindSpeedLabel.Content = (int)(windSpeed * 0.62);
                }
                else
                {
                    WindSpeedLabel.Content = (int)windSpeed;
                }
            }
        }

        private void SetColor(double percentage)
        {
            var correctColor = GradientPick(percentage, StartColor, CenterColor, EndColor);
            WindDirIcon.Foreground = new SolidColorBrush(correctColor);
            WindSpeedLabel.Foreground = new SolidColorBrush(correctColor);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Maintain a 1:1 aspect ratio
            if (e.WidthChanged)
            {
                this.Height = this.ActualWidth;
            }
            else if (e.HeightChanged)
            {
                this.Width = this.ActualHeight;
            }
        }

        public byte LinearInterp(byte start, byte end, double percentage) => (byte)(start + (int)Math.Round(percentage * (end - start)));
        public Color ColorInterp(Color start, Color end, double percentage) =>
            Color.FromArgb(LinearInterp(start.A, end.A, percentage),
                           LinearInterp(start.R, end.R, percentage),
                           LinearInterp(start.G, end.G, percentage),
                           LinearInterp(start.B, end.B, percentage));
        public Color GradientPick(double percentage, Color Start, Color Center, Color End)
        {
            if (percentage < 0.5)
                return ColorInterp(Start, Center, percentage / 0.5);
            else if (percentage == 0.5)
                return Center;
            else if (percentage > 1)
                return End;
            else
                return ColorInterp(Center, End, (percentage - 0.5) / 0.5);

        }
    }
}
