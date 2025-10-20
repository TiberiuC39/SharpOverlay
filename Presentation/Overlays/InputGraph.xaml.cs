using Core.Events;
using Core.Models;
using Core.Services;
using Presentation.Events;
using Presentation.Models;
using Presentation.Services;
using ScottPlot.Plottables;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Presentation.Overlays
{
    /// <summary>
    /// Interaction logic for InputGraph.xaml
    /// </summary>
    public partial class InputGraph : Window
    {
        private DataStreamer throttleStreamer;
        private DataStreamer brakeStreamer;
        private DataStreamer clutchStreamer;
        private DataStreamer steeringStreamer;
        private DataStreamer absStreamer;

        private Input input = new Input();

        private readonly InputGraphSettings _settings = App.appSettings.InputGraphSettings;
        private readonly SimReader _simReader = new SimReader();
        private readonly WindowStateService _windowStateService;

        public InputGraph()
        {
            InitializeComponent();
            Services.JotService.tracker.Track(this);

            _windowStateService = new WindowStateService(_simReader, _settings);

            _simReader.OnTelemetryUpdated += IracingWrapper_TelemetryUpdated;
            _windowStateService.WindowStateChanged += OnWindowStateChange;
            _settings.PropertyChanged += Graph_HandleSettingUpdated;

            HookStreamer(ref throttleStreamer, _settings.ThrottleColor, true);
            HookStreamer(ref brakeStreamer, _settings.BrakeColor, true);
            HookStreamer(ref clutchStreamer, _settings.ClutchColor, _settings.ShowClutch);
            HookStreamer(ref steeringStreamer, _settings.SteeringColor, _settings.ShowSteering);
            HookStreamer(ref absStreamer, _settings.SteeringColor, true);

            PlotSetup();
            SetColorPercentageLabels();
        }

        private void OnWindowStateChange(object? sender, WindowStateEventArgs e)
        {
            if ((e.IsOpen || e.IsInTestMode) && e.IsEnabled)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Graph_HandleSettingUpdated(object? sender, PropertyChangedEventArgs e)
        {
            SetStreamerColorAndWidth(ref throttleStreamer, _settings.ThrottleColor);
            SetStreamerColorAndWidth(ref brakeStreamer, _settings.BrakeColor);
            SetStreamerColorAndWidth(ref clutchStreamer, _settings.ClutchColor);
            SetStreamerColorAndWidth(ref steeringStreamer, _settings.SteeringColor);
            SetStreamerColorAndWidth(ref absStreamer, _settings.ABSColor);

            //InputPlot.Plot.DataBackground.Color = TransformColor(App.appSettings.InputGraphSettings.BackgroundColor);
            InputPlot.Plot.FigureBackground.Color = TransformColor(_settings.BackgroundColor);
            InputPlot.Refresh();

            if (_settings.ShowClutch)
            {
                clutchStreamer.IsVisible = true;
            }
            else if (!_settings.ShowClutch)
            {
                clutchStreamer.IsVisible = false;
            }

            if (_settings.ShowSteering)
            {
                steeringStreamer.IsVisible = true;
            }
            else if (!_settings.ShowSteering)
            {
                steeringStreamer.IsVisible = false;
            }

            SetColorPercentageLabels();
        }

        private void UpdateInputs(TelemetryOutputDTO telemetryOutput)
        {
            if (_settings.UseRawValues)
            {
                input.Brake = telemetryOutput.BrakeRaw * 100;
                input.Throttle = telemetryOutput.ThrottleRaw * 100;
                input.Clutch = (1 - telemetryOutput.ClutchRaw) * 100;
            }
            else
            {
                input.Brake = telemetryOutput.Brake * 100;
                input.Throttle = telemetryOutput.Throttle * 100;
                input.Clutch = (1 - telemetryOutput.Clutch) * 100;
            }

            input.Steering = telemetryOutput.SteeringWheelAngle * 10 + 50;
            input.ABS = telemetryOutput.BrakeABSactive ? input.Brake : 0;

            if (BrakePercentage.IsVisible)
            {
                BrakePercentage.Content = $"B: {Math.Round(input.Brake, 0)} %";
            }

            if (ThrottlePercentage.IsVisible)
            {
                ThrottlePercentage.Content = $"T: {Math.Round(input.Throttle, 0)} %";
            }

            if (ClutchPercentage.IsVisible)
            {
                ClutchPercentage.Content = $"C: {Math.Round(input.ABS, 0)} %";
            }
        }

        private void AddInputsToStreamers(Input input)
        {
            throttleStreamer.Add(input.Throttle);
            brakeStreamer.Add(input.Brake);
            if (input.ABS != 0) 

            absStreamer.Add(input.ABS);
            else
            {
                absStreamer.Add(double.NaN);
            }

            if (_settings.ShowClutch)
            {
                clutchStreamer.Add(input.Clutch);
            }

            if (_settings.ShowSteering)
            {
                steeringStreamer.Add(input.Steering);
            }
        }

        private void IracingWrapper_TelemetryUpdated(object? sender, TelemetryEventArgs e)
        {
            UpdateInputs(e.TelemetryOutput);
            AddInputsToStreamers(input);
            InputPlot.Refresh();
        }

        private void Window_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void PlotSetup()
        {
            InputPlot.Menu.Clear();

            InputPlot.Plot.FigureBackground.Color = TransformColor(_settings.BackgroundColor);
            InputPlot.Plot.Axes.Frameless();

            InputPlot.Plot.Axes.SetLimitsY(-5, 105);
            InputPlot.Plot.Axes.SetLimitsX(0, 500);

            InputPlot.UserInputProcessor.IsEnabled = false;
            InputPlot.Interaction.IsEnabled = false;
        }

        private void HookStreamer(ref DataStreamer ds, SolidColorBrush color, bool isVisible)
        {
            ds = InputPlot.Plot.Add.DataStreamer(500);

            ds.Color = TransformColor(color);

            ds.LineWidth = _settings.LineWidth;

            ds.ViewScrollLeft();

            ds.ManageAxisLimits = false;

            ds.IsVisible = isVisible;
        }

        private void SetStreamerColorAndWidth(ref DataStreamer dataStreamer, SolidColorBrush color)
        {
            dataStreamer.Color = TransformColor(color);

            dataStreamer.LineWidth = _settings.LineWidth;
        }

        private ScottPlot.Color TransformColor(SolidColorBrush color)
        {
            return new ScottPlot.Color(color.Color.R, color.Color.G, color.Color.B, color.Color.A);
        }

        private void SetColorPercentageLabels()
        {
            ThrottlePercentage.Visibility = _settings.ShowPercentageThrottle ? Visibility.Visible : Visibility.Hidden;
            ThrottlePercentage.Foreground = _settings.ThrottleColor;

            BrakePercentage.Visibility = _settings.ShowPercentageBrake ? Visibility.Visible : Visibility.Hidden;
            BrakePercentage.Foreground = _settings.BrakeColor;

            ClutchPercentage.Visibility = _settings.ShowPercentageClutch ? Visibility.Visible : Visibility.Hidden;
            ClutchPercentage.Foreground = _settings.ClutchColor;

            SteeringPercentage.Visibility = _settings.ShowPercentageSteering ? Visibility.Visible : Visibility.Hidden;
            SteeringPercentage.Foreground = _settings.SteeringColor;
        }
    }
}
