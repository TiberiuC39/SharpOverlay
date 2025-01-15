using iRacingSdkWrapper;
using SharpOverlay.Events;
using SharpOverlay.Models;
using SharpOverlay.Services.Base;
using SharpOverlay.Services.Spotter;
using SharpOverlay.Utilities;
using System;
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
        private readonly SimReader _simReader = new(DefaultTickRates.BarSpotter);
        private readonly WindowStateService _windowStateService;
        private readonly BarSpotterService _service;

        private readonly BarSpotterSettings _settings = App.appSettings.BarSpotterSettings;

        private Enums.CarLeftRight carLeftRight;

        public BarSpotter()
        {
            InitializeComponent();
            this.DataContext = App.appSettings.BarSpotterSettings;
            Services.JotService.tracker.Track(this);

            _service = new BarSpotterService(_simReader);
            _windowStateService = new WindowStateService(_simReader, _settings);

            _simReader.OnConnected += OnConnected;
            _simReader.OnDisconnected += OnDisconnected;
            _simReader.OnTelemetryUpdated += OnTelemetryUpdated;

            _windowStateService.WindowStateChanged += OnWindowStateChanged;

            barSpotterWindow.SizeChanged += window_SetBarEqualToWindow;
        }

        private void OnWindowStateChanged(object? sender, WindowStateEventArgs e)
        {
            if (e.IsInTestMode && e.IsEnabled)
            {
                HandleTestMode();
            }
            else if (e.IsOpen && e.IsEnabled)
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
            CarClear();
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

        private void HandleTestMode()
        {
            if (_settings.IsInTestMode && _settings.IsEnabled)
            {
                RenderBothBars();
                Show();
                leftCanvas.Visibility = Visibility.Visible;
                rightCanvas.Visibility = Visibility.Visible;
            }
            else
            {
                CarClear();
            }
        }

        private void window_SetBarEqualToWindow(object? sender, EventArgs e)
        {
            if (_settings.IsInTestMode)
            {
                leftFill.Height = _settings.BarLength;
                rightFill.Height = _settings.BarLength;
            }
        }

        private void OnTelemetryUpdated(object? sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (!_settings.IsInTestMode)
            {
                carLeftRight = (Enums.CarLeftRight)e.TelemetryInfo.CarLeftRight.Value;

                if (carLeftRight == Enums.CarLeftRight.irsdk_LRClear)
                {
                    CarClear();
                    return;
                }

                if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeft)
                {
                    double pixelOffset = PixelOffset();

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

        private void CarClear()
        {
            RenderLeftBar(grid.ActualHeight);
            RenderRightBar(grid.ActualHeight);

            leftCanvas.Visibility = Visibility.Hidden;
            rightCanvas.Visibility = Visibility.Hidden;
        }

        private double PixelOffset()
        {
            double offset = _service.CenterOffset();

            var pixelOffset = grid.ActualHeight * -offset;
            return pixelOffset;
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

            leftCanvas.Visibility = Visibility.Visible;
            rightCanvas.Visibility = Visibility.Visible;
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
    }
}
