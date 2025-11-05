using Core.Events;
using Core.Services.Spotter;
using Presentation.Events;
using Presentation.Models;
using Presentation.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Core.Models.Enums;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Presentation.Overlays
{
    /// <summary>
    /// Interaction logic for BarSpotter.xaml
    /// </summary>
    public partial class BarSpotter : Window
    {
        private readonly WindowStateService _windowStateService;
        private readonly BarSpotterService _service;

        private readonly BarSpotterSettings _settings = App.appSettings.BarSpotterSettings;

        public BarSpotter()
        {
            InitializeComponent();
            this.DataContext = App.appSettings.BarSpotterSettings;
            Services.JotService.tracker.Track(this);

            _service = new BarSpotterService();
            _service.OnBarUpdated += OnBarUpdate;
            _service.SimReader.OnConnected += OnConnect;

            _windowStateService = new WindowStateService(_service.SimReader, _settings);
            _windowStateService.WindowStateChanged += OnWindowStateChange;
            _windowStateService.Initialize();

            barSpotterWindow.SizeChanged += Window_SetBarEqualToWindow;
        }

        protected override void OnClosed(EventArgs e)
        {
            _service.OnBarUpdated -= OnBarUpdate;
            _service.Dispose();

            _windowStateService.WindowStateChanged -= OnWindowStateChange;
            _windowStateService.Dispose();

            barSpotterWindow.SizeChanged -= Window_SetBarEqualToWindow;

            base.OnClosed(e);
        }

        private void OnConnect(object? sender, EventArgs e)
        {
            CarClear();
        }

        private void OnWindowStateChange(object? sender, WindowStateEventArgs e)
        {
            if (e.IsInTestMode)
            {
                HandleTestMode();
            }
            else if (e.IsOpen)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        private void Window_MouseDown(object? sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void HandleTestMode()
        {
            RenderBothBars();
            Show();
            leftCanvas.Visibility = Visibility.Visible;
            rightCanvas.Visibility = Visibility.Visible;
        }

        private void Window_SetBarEqualToWindow(object? sender, EventArgs e)
        {
            if (_settings.IsInTestMode)
            {
                leftFill.Height = _settings.BarLength;
                rightFill.Height = _settings.BarLength;
            }
        }

        private void OnBarUpdate(object? sender, BarSpotterEventArgs e)
        {
            if (!_settings.IsInTestMode)
            {
                var spotter = e.CarPos;

                if (spotter == Spotter.Clear)
                {
                    CarClear();
                }
                else if (spotter == Spotter.CarLeft)
                {
                    var pixelOffset = PixelOffset(e.OffsetPct);

                    RenderLeftBar(pixelOffset);

                    rightCanvas.Visibility = Visibility.Hidden;
                    leftCanvas.Visibility = Visibility.Visible;
                }
                else if (spotter == Spotter.CarRight)
                {
                    var pixelOffset = PixelOffset(e.OffsetPct);

                    RenderRightBar(pixelOffset);
                    rightCanvas.Visibility = Visibility.Visible;
                    leftCanvas.Visibility = Visibility.Hidden;
                }
                else if (spotter == Spotter.CarLeftRight)
                {
                    RenderBothBars();
                }
                else if (spotter == Spotter.TwoCarsLeft)
                {
                    RenderLeftBar(0, _settings.ThreeWideBarColor);
                    leftCanvas.Visibility = Visibility.Visible;
                    rightCanvas.Visibility = Visibility.Hidden;
                }
                else if (spotter == Spotter.TwoCarsRight)
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

        private double PixelOffset(double offsetPct)
        {
            var pixelOffset = grid.ActualHeight * -offsetPct;
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
