using SharpOverlay.Models;
using SharpOverlay.Services;
using iRacingSdkWrapper;
using ScottPlot.Plottables;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for InputGraph.xaml
    /// </summary>
    public partial class InputGraph : Window
    {
        private DataStreamer throttleStreamer;
        private DataStreamer brakeStreamer;
        private DataStreamer clutchStreamer;
        private Input input = new Input();

        private bool absActive;
        private ScottPlot.Color currentBgColor;

        private SdkWrapper iracingWrapper = iRacingDataService.Wrapper;
        public InputGraph()
        {
            InitializeComponent();
            Services.JotService.tracker.Track(this);
            iracingWrapper.TelemetryUpdated += iracingWrapper_TelemetryUpdated;
            App.appSettings.InputGraphSettings.PropertyChanged += graph_HandleSettingUpdated;
            HookStreamer(ref throttleStreamer, App.appSettings.InputGraphSettings.ThrottleColor, true);
            HookStreamer(ref brakeStreamer, App.appSettings.InputGraphSettings.BrakeColor, true);
            HookStreamer(ref clutchStreamer, App.appSettings.InputGraphSettings.ClutchColor, App.appSettings.InputGraphSettings.ShowClutch);
            PlotSetup();
        }

        private void graph_HandleSettingUpdated(object sender, EventArgs e)
        {
            SetStreamerColorAndWidth(ref throttleStreamer, App.appSettings.InputGraphSettings.ThrottleColor);
            SetStreamerColorAndWidth(ref brakeStreamer, App.appSettings.InputGraphSettings.BrakeColor);
            SetStreamerColorAndWidth(ref clutchStreamer, App.appSettings.InputGraphSettings.ClutchColor);

            //InputPlot.Plot.DataBackground.Color = TransformColor(App.appSettings.InputGraphSettings.BackgroundColor);
            InputPlot.Plot.FigureBackground.Color = TransformColor(App.appSettings.InputGraphSettings.BackgroundColor);
            InputPlot.Refresh();

            if (App.appSettings.InputGraphSettings.ShowClutch)
            {
                clutchStreamer.IsVisible = true;
            }
            else if (!App.appSettings.InputGraphSettings.ShowClutch)
            {
                clutchStreamer.IsVisible = false;
            }
        }
        private void UpdateInputs(TelemetryInfo telemetryInfo)
        {

            if (App.appSettings.InputGraphSettings.UseRawValues)
            {
                input.Brake = iracingWrapper.GetTelemetryValue<float>("BrakeRaw").Value * 100;
                input.Throttle = iracingWrapper.GetTelemetryValue<float>("ThrottleRaw").Value * 100;
                input.Clutch = (1 - iracingWrapper.GetTelemetryValue<float>("ClutchRaw").Value) * 100;

            }
            else
            {
                input.Brake = telemetryInfo.Brake.Value * 100;
                input.Throttle = telemetryInfo.Throttle.Value * 100;
                input.Clutch = (1 - telemetryInfo.Clutch.Value) * 100;
            }
        }

        private void AddInputsToStreamers(Input input)
        {
            throttleStreamer.Add(input.Throttle);
            brakeStreamer.Add(input.Brake);
            if (App.appSettings.InputGraphSettings.ShowClutch)
            {
                clutchStreamer.Add(input.Clutch);
            }
        }

        private void iracingWrapper_TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            UpdateInputs(e.TelemetryInfo);
            AddInputsToStreamers(input);
            InputPlot.Refresh();

            absActive = iracingWrapper.GetTelemetryValue<bool>("BrakeABSactive").Value;
            if (App.appSettings.InputGraphSettings.ShowABS)
            {
                ABSFlash();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void PlotSetup()
        {
            InputPlot.Menu.Clear();

            InputPlot.Plot.FigureBackground.Color = TransformColor(App.appSettings.InputGraphSettings.BackgroundColor);
            InputPlot.Plot.Axes.Frameless();

            InputPlot.Plot.Axes.SetLimitsY(-5, 105);
            InputPlot.Plot.Axes.SetLimitsX(0, 250);

            InputPlot.UserInputProcessor.IsEnabled = false;
            InputPlot.Interaction.IsEnabled = false;
        }

        private void HookStreamer(ref DataStreamer ds, SolidColorBrush color, bool isVisible)
        {
            ds = InputPlot.Plot.Add.DataStreamer(250);

            ds.Color = TransformColor(color);

            ds.LineWidth = App.appSettings.InputGraphSettings.LineWidth;

            ds.ViewScrollLeft();

            ds.ManageAxisLimits = false;

            ds.IsVisible = isVisible;

        }

        private void SetStreamerColorAndWidth(ref DataStreamer dataStreamer, SolidColorBrush color)
        {
            dataStreamer.Color = TransformColor(color);

            dataStreamer.LineWidth = App.appSettings.InputGraphSettings.LineWidth;
        }
        private ScottPlot.Color TransformColor(SolidColorBrush color)
        {
            return new ScottPlot.Color(color.Color.R, color.Color.G, color.Color.B, color.Color.A);
        }

        private void ABSFlash()
        {
            if (absActive && currentBgColor == TransformColor(App.appSettings.InputGraphSettings.BackgroundColor))
            {
                InputPlot.Plot.FigureBackground.Color = TransformColor(App.appSettings.InputGraphSettings.ABSColor);
                currentBgColor = InputPlot.Plot.DataBackground.Color;
            }
            else if(currentBgColor != TransformColor(App.appSettings.InputGraphSettings.BackgroundColor))
            {
                InputPlot.Plot.FigureBackground.Color = TransformColor(App.appSettings.InputGraphSettings.BackgroundColor);
                currentBgColor = TransformColor(App.appSettings.InputGraphSettings.BackgroundColor);
            }
        }
    }
}

