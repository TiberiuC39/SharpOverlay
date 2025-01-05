using iRacingSdkWrapper;
using SharpOverlay.Events;
using SharpOverlay.Models;
using SharpOverlay.Services.Base;
using SharpOverlay.Services.Spotter;
using SharpOverlay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace SharpOverlay
{
    /// <summary>
    /// Interaction logic for BarSpotter.xaml
    /// </summary>
    public partial class BarSpotter : Window
    {
        private readonly SimReader _simReader = new SimReader(DefaultTickRates.BarSpotter);
        private readonly WindowStateService _windowStateService = new WindowStateService();
        private readonly BarSpotterService _service;

        private readonly BarSpotterSettings _settings = App.appSettings.BarSpotterSettings;

        private List<Driver> drivers = new List<Driver>();
        private Driver? _me;
        private bool isUpdatingDrivers = true;
        private float closestCar;
        private Dictionary<int, float> relatives = new Dictionary<int, float>();
        private float trackLen;
        private Enums.CarLeftRight carLeftRight;

        public BarSpotter()
        {
            InitializeComponent();
            this.DataContext = App.appSettings.BarSpotterSettings;
            Services.JotService.tracker.Track(this);

            _service = new BarSpotterService(_simReader);

            _simReader.OnConnected += OnConnected;
            _simReader.OnDisconnected += OnDisconnected;
            _simReader.OnTelemetryUpdated += OnTelemetryUpdated;
            _simReader.OnTelemetryUpdated += _windowStateService.ExecuteOnTelemetry;

            _windowStateService.WindowStateChanged += OnWindowStateChanged;

            barSpotterWindow.SizeChanged += window_SetBarEqualToWindow;

            App.appSettings.BarSpotterSettings.PropertyChanged += settings_TestMode;
        }
        private void OnWindowStateChanged(object? sender, WindowStateEventArgs e)
        {
            if (e.IsOpen)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void OnConnected(object? sender, EventArgs e)
        {
            Canvas.SetTop(leftFill, grid.ActualHeight);
            Canvas.SetTop(rightFill, grid.ActualHeight);

            leftCanvas.Visibility = Visibility.Hidden;
            rightCanvas.Visibility = Visibility.Hidden;
        }

        private void OnDisconnected(object? sender, EventArgs e)
        {
            _service.Clear();
        }

        private void Window_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void HandleTestMode(bool test)
        {
            if (test)
            {
                RenderBothBars();
                leftCanvas.Visibility = Visibility.Visible;
                rightCanvas.Visibility = Visibility.Visible;
            }
        }

        private void window_SetBarEqualToWindow(object? sender, EventArgs e)
        {
            if (App.appSettings.BarSpotterSettings.IsInTestMode)
            {
                leftFill.Height = App.appSettings.BarSpotterSettings.BarLength;
                rightFill.Height = App.appSettings.BarSpotterSettings.BarLength;
            }
        }

        private void settings_TestMode(object? sender, EventArgs e)
        {
            HandleTestMode(App.appSettings.BarSpotterSettings.IsInTestMode);
        }

        private void OnTelemetryUpdated(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (!App.appSettings.BarSpotterSettings.IsInTestMode)
            {
                carLeftRight = (Enums.CarLeftRight)e.TelemetryInfo.CarLeftRight.Value;

                if (carLeftRight == Enums.CarLeftRight.irsdk_LRClear)
                {
                    RenderLeftBar(grid.ActualHeight);
                    RenderRightBar(grid.ActualHeight);

                    leftCanvas.Visibility = Visibility.Hidden;
                    rightCanvas.Visibility = Visibility.Hidden;
                }
                else
                {
                    if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeft)
                    {
                        double offset = _service.CenterOffset();

                        var pixelOffset = grid.ActualHeight * -offset;

                        RenderLeftBar(pixelOffset);

                        rightCanvas.Visibility = Visibility.Hidden;
                        leftCanvas.Visibility = Visibility.Visible;
                    }
                    else if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarRight)
                    {
                        double offset = _service.CenterOffset();

                        var pixelOffset = grid.ActualHeight * -offset;

                        RenderRightBar(pixelOffset);
                        rightCanvas.Visibility = Visibility.Visible;
                        leftCanvas.Visibility = Visibility.Hidden;
                    }
                    else if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeftRight)
                    {
                        RenderBothBars();
                        leftCanvas.Visibility = Visibility.Visible;
                        rightCanvas.Visibility = Visibility.Visible;
                    }
                    else if (carLeftRight == Enums.CarLeftRight.irsdk_LR2CarsLeft)
                    {
                        RenderLeftBar(0, _settings.ThreeWideBarColor);
                        leftCanvas.Visibility = Visibility.Visible;
                        rightCanvas.Visibility = Visibility.Hidden;
                    }
                    else if (carLeftRight == Enums.CarLeftRight.irsdk_LR2CarsRight)
                    {
                        RenderRightBar(0, _settings.ThreeWideBarColor);
                        rightCanvas.Visibility = Visibility.Visible;
                        leftCanvas.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        private void RenderLeftBar(double offset)
        {
            RenderBar(leftFill, offset);
        }

        private void RenderLeftBar(double offset, Brush color)
        {
            RenderBar(leftFill, offset, color);
        }
        private void RenderRightBar(double offset, Brush color)
        {
            RenderBar(rightFill, offset, color);
        }

        private void RenderRightBar(double offset)
        {
            RenderBar(rightFill, offset);
        }

        private void RenderBothBars()
        {
            leftFill.Fill = _settings.ThreeWideBarColor;
            rightFill.Fill = _settings.ThreeWideBarColor;

            Canvas.SetTop(leftFill, 0);
            Canvas.SetTop(rightFill, 0);
        }

        private void RenderBar(Rectangle rect, double offset)
        {
            RenderBar(rect, offset, _settings.BarColor);
        }

        private void RenderBar(Rectangle rect, double offset, Brush color)
        {
            rect.Fill = color;
            Canvas.SetTop(rect, offset);
        }

        private void UpdateRelatives(Driver? me, float[] lapDistances, TrackSurfaces[] trackSurfaces)
        {
            var difference = 100f;

            foreach (Driver driver in drivers)
            {
                driver.LapDistancePct = lapDistances[driver.CarIdx];
                driver.TrackSurface = trackSurfaces[driver.CarIdx];

                if (me != null && driver.TrackSurface != TrackSurfaces.NotInWorld && driver.CarIdx != me.CarIdx)
                {
                    var relative = driver.LapDistancePct - me.LapDistancePct;

                    if (relative > 0.5)
                    {
                        relative -= 1;
                    }
                    else if (relative < -0.5)
                    {
                        relative += 1;
                    }

                    driver.RelativeLapDistancePct = relative;
                    relatives[driver.CarIdx] = relative;

                    var closestDriver = relatives.OrderBy(e => Math.Abs(e.Value - 0)).FirstOrDefault();
                    closestCar = closestDriver.Value;

                    foreach (var r in relatives.Values)
                    {
                        var currentDifference = Math.Abs(0 - r);
                        if (currentDifference < difference)
                        {
                            closestCar = r;
                            difference = currentDifference;
                        }
                    }
                }
                else
                {
                    driver.RelativeLapDistancePct = -1;
                }
            }
        }

        private void RenderBar(TelemetryInfo info)
        {
            if (!App.appSettings.BarSpotterSettings.IsInTestMode)
            {
                leftFill.Visibility = Visibility.Hidden;
                rightFill.Visibility = Visibility.Hidden;

                carLeftRight = (Enums.CarLeftRight)info.CarLeftRight.Value;

                if (carLeftRight == Enums.CarLeftRight.irsdk_LRClear)
                {
                    leftFill.Visibility = Visibility.Hidden;
                    rightFill.Visibility = Visibility.Hidden;
                }
                else if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeft)
                {
                    var normalizedDistance = Math.Max(-1, Math.Min(1, closestCar / (5 / trackLen)));
                    UpdateBar(leftFill, normalizedDistance, App.appSettings.BarSpotterSettings.BarColor);
                }
                else if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarRight)
                {
                    var normalizedDistance = Math.Max(-1, Math.Min(1, closestCar / (5 / trackLen)));
                    UpdateBar(rightFill, normalizedDistance, App.appSettings.BarSpotterSettings.BarColor);
                }
                else if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeftRight)
                {
                    UpdateBar(rightFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                    UpdateBar(leftFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                }
                else if (carLeftRight == Enums.CarLeftRight.irsdk_LR2CarsLeft)
                {
                    UpdateBar(leftFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                }
                else if (carLeftRight == Enums.CarLeftRight.irsdk_LR2CarsRight)
                {
                    UpdateBar(rightFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                }
            }
        }

        private void UpdateBar(System.Windows.Shapes.Rectangle rect, float barSize, Brush color)
        {
            double topPos = 0;
            rect.Visibility = Visibility.Visible;
            rect.Fill = color;
            if (barSize > 0)
            {
                topPos = 0;
                ((ScaleTransform)((TransformGroup)rect.RenderTransform).Children[0]).ScaleY = 1;
            }
            else
            {
                topPos = grid.ActualHeight;
                ((ScaleTransform)((TransformGroup)rect.RenderTransform).Children[0]).ScaleY = -1;

            }
            if (barSize > 1)
            {
                rect.Height = grid.ActualHeight;
            }
            else
            {
                rect.Height = grid.ActualHeight * (1 - Math.Abs(barSize));

            }
            Canvas.SetTop(rect, topPos);
        }
    }
}
