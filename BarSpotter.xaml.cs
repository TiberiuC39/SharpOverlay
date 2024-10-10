using SharpOverlay.Models;
using SharpOverlay.Services;
using iRacingSdkWrapper;
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
        private SdkWrapper iracingWrapper = iRacingDataService.Wrapper;
        private List<Driver> drivers = new List<Driver>();
        private bool isUpdatingDrivers = true;
        private float closestCar;
        private Dictionary<Driver, float> relatives = new Dictionary<Driver, float>();
        private float trackLen;
        private Enums.CarLeftRight carLeftRight;

        public BarSpotter()
        {
            InitializeComponent();
            this.DataContext = App.appSettings.BarSpotterSettings;
            Services.JotService.tracker.Track(this);

            iracingWrapper.SessionUpdated += iracingWrapper_SessionInfoUpdated;
            iracingWrapper.TelemetryUpdated += iracingWrapper_TelemetryUpdated;
            iracingWrapper.Connected += iracingWrapper_Connected;
            iracingWrapper.Disconnected += iracingWrapper_Disconnected;

            barSpotterWindow.SizeChanged += window_SetBarEqualToWindow;

            if (iracingWrapper.IsConnected)
            {
                iracingWrapper.RequestSessionInfoUpdate();
            }

            App.appSettings.BarSpotterSettings.PropertyChanged += settings_TestMode;
        }
        private void HandleTestMode(bool test)
        {
            if (test)
            {
                leftFill.Height = App.appSettings.BarSpotterSettings.BarLength;
                rightFill.Height = App.appSettings.BarSpotterSettings.BarLength;
                leftFill.Visibility = Visibility.Visible;
                rightFill.Visibility = Visibility.Visible;
            }
            else
            {
                leftFill.Height = 0;
                rightFill.Height = 0;

            }
        }

        private void window_SetBarEqualToWindow(object sender, EventArgs e)
        {
            if (App.appSettings.BarSpotterSettings.TestMode)
            {
                leftFill.Height = App.appSettings.BarSpotterSettings.BarLength;
                rightFill.Height = App.appSettings.BarSpotterSettings.BarLength;
            }
        }

        private void settings_TestMode(object sender, EventArgs e)
        {
            HandleTestMode(App.appSettings.BarSpotterSettings.TestMode);

        }
        private void InitializeBar(Rectangle rect)
        {
            rect.Width = App.appSettings.BarSpotterSettings.BarWidth;
            rect.Fill = App.appSettings.BarSpotterSettings.BarColor;
        }
        private void iracingWrapper_SessionInfoUpdated(object sender, SdkWrapper.SessionUpdatedEventArgs e)
        {
            trackLen = (float) (e.SessionInfo.WeekendInfo.TrackLength * 1000f);

            isUpdatingDrivers = true;
            this.ParseDrivers(e.SessionInfo);
            Driver? me = drivers.Find(d => d.Id == iracingWrapper.DriverId);
            isUpdatingDrivers = false;
        }

        private void iracingWrapper_TelemetryUpdated(object sender, SdkWrapper.TelemetryUpdatedEventArgs e)
        {
            if (isUpdatingDrivers) return;

            this.UpdateDriversTelemetry(e.TelemetryInfo);

            var closestDriver = relatives.OrderBy(x => x.Value).FirstOrDefault();
        }

        private void ParseDrivers(SessionInfo sessionInfo)
        {
            int id = 0;
            Driver? driver;

            var newDrivers = new List<Driver>();

            do
            {
                driver = null;
                var racer = sessionInfo.Drivers.FirstOrDefault(d => d.CarIdx == id);

                string name = racer?.UserName;

                if (name != null && name != "Pace Car")
                {
                    if (drivers.Any())
                    {
                        driver = drivers.Find(d => d.Name == name);
                    }

                    if (driver == null)
                    {
                        driver = new Driver();
                        driver.Id = id;
                        driver.Name = name;
                        driver.Number = racer.CarNumber.ToString();

                    }
                    newDrivers.Add(driver);

                    id++;
                }
            } while (driver != null);

            drivers.Clear();
            drivers.AddRange(newDrivers);
        }
        private void UpdateDriversTelemetry(TelemetryInfo info)
        {
            Driver? me = drivers.Find(d => d.Id == iracingWrapper.DriverId);

            var laps = info.CarIdxLap.Value;
            var lapDistances = info.CarIdxLapDistPct.Value;
            var trackSurfaces = info.CarIdxTrackSurface.Value;
            var minRelative = float.MaxValue;
            var difference = 100f;
            
            foreach (Driver driver in drivers)
            {
                driver.LapDistance = lapDistances[driver.Id];
                driver.TrackSurface = trackSurfaces[driver.Id];

                if (me != null && driver.TrackSurface != TrackSurfaces.NotInWorld && driver.Id != me.Id)
                {
                    var relative = driver.LapDistance - me.LapDistance;

                    if (relative > 0.5)
                    {
                        relative -= 1;
                    }
                    else if (relative < -0.5)
                    {
                        relative += 1;
                    }

                    driver.RelativeLapDistance = relative;
                    relatives[driver] = relative;

                    var closestDriver = relatives.OrderBy(e => Math.Abs(e.Value - 0)).FirstOrDefault();
                    closestCar = closestDriver.Value;

                    foreach(var r in relatives.Values)
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
                    driver.RelativeLapDistance = -1;
                }
            }
            if (!App.appSettings.BarSpotterSettings.TestMode)
            {
                leftFill.Visibility = Visibility.Hidden;
                rightFill.Visibility = Visibility.Hidden;

                carLeftRight = (Enums.CarLeftRight)iracingWrapper.GetData("CarLeftRight");

                if (carLeftRight == Enums.CarLeftRight.irsdk_LRClear)
                {
                    leftFill.Visibility = Visibility.Hidden;
                    rightFill.Visibility = Visibility.Hidden;
                }
                if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeft)
                {
                    var normalizedDistance = Math.Max(-1, Math.Min(1, closestCar / (5 / trackLen)));
                    UpdateBar(leftFill, normalizedDistance, App.appSettings.BarSpotterSettings.BarColor);

                }
                if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarRight)
                {
                    var normalizedDistance = Math.Max(-1, Math.Min(1, closestCar / (5 / trackLen)));
                    UpdateBar(rightFill, normalizedDistance, App.appSettings.BarSpotterSettings.BarColor);

                }

                if (carLeftRight == Enums.CarLeftRight.irsdk_LRCarLeftRight)
                {
                    UpdateBar(rightFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                    UpdateBar(leftFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                }

                if (carLeftRight == Enums.CarLeftRight.irsdk_LR2CarsLeft)
                {
                    UpdateBar(leftFill, 10f, App.appSettings.BarSpotterSettings.ThreeWideBarColor);
                }

                if (carLeftRight == Enums.CarLeftRight.irsdk_LR2CarsRight)
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
            else {
                rect.Height = grid.ActualHeight * (1 - Math.Abs(barSize));

            }
            Canvas.SetTop(rect, topPos);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void iracingWrapper_Disconnected(object sender, EventArgs e)
        {
            leftFill.Height = 0;
            rightFill.Height = 0;
        }

        private void iracingWrapper_Connected(object sender, EventArgs e)
        {
            drivers.Clear();
            relatives.Clear();
        }
    }
}
