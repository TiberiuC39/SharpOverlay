
using iRacingSdkWrapper;
using SharpOverlay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace SharpOverlay.Services
{
    public class iRacingTelemetryService
    {
        private SdkWrapper _sdk;
        private TelemetryOutput _telemetryOutput;
        private SessionInfo? _sessionInfo;
        private Dictionary<int, Racer> _drivers = new();
        private Racer? _me;
        private Car? _car;

        public List<Racer> Drivers => _drivers.Values.ToList();

        public iRacingTelemetryService()
        {
            _sdk = iRacingDataService.Wrapper;
            GetTelemetryOutput();

            _sdk.TelemetryUpdated += TelemetryUpdatedEvent;
            _sdk.SessionInfoUpdated += SessionUpdatedEvent;
        }

        private void GetTelemetryOutput()
        {
            _telemetryOutput = new TelemetryOutput(_sdk.Sdk);
        }

        public void HookUpToTelemetryEvent(EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> action)
        {
            _sdk.TelemetryUpdated += action;
        }

        private void TelemetryUpdatedEvent(object sender, SdkWrapper.TelemetryUpdatedEventArgs telemetryEvent)
        {
            GetTelemetryOutput();
        }

        private async void SessionUpdatedEvent(object sender, SdkWrapper.SessionInfoUpdatedEventArgs sessionEvent)
        {
            _sessionInfo = sessionEvent.SessionInfo;
            await ParseDrivers();
        }        

        private Task ParseDrivers()
        {
            Dictionary<int, Racer> drivers = new();

            var driverInfoNode = _sessionInfo.YamlRoot["DriverInfo"];

            foreach (YamlMappingNode node in (YamlSequenceNode)driverInfoNode["Drivers"])
            {
                var driver = ParseDriver(node);

                drivers.Add(driver.UserID, driver);
            }

            _drivers = drivers;

            _me = _drivers[GetDriverUserId()];

            return Task.CompletedTask;
        }

        private Racer ParseDriver(YamlMappingNode driverInfo)
            => new Racer()
            {
                UserID = int.Parse(driverInfo["UserID"].ToString()),
                UserName = driverInfo["UserName"].ToString(),
                AbbrevName = driverInfo["AbbrevName"].ToString(),
                Initials = driverInfo["Initials"].ToString(),
                TeamName = driverInfo["TeamName"].ToString(),
                TeamID = int.Parse(driverInfo["TeamID"].ToString()),
                CarIdx = int.Parse(driverInfo["CarIdx"].ToString()),                
                IRating = int.Parse(driverInfo["IRating"].ToString()),
                LicLevel = int.Parse(driverInfo["LicLevel"].ToString()),
                LicSubLevel = int.Parse(driverInfo["LicSubLevel"].ToString()),
                LicString = driverInfo["LicString"].ToString(),
                LicColor = driverInfo["LicColor"].ToString(),
                IsSpectator = int.Parse(driverInfo["IsSpectator"].ToString()),
                CarDesignStr = driverInfo["CarDesignStr"].ToString(),
                HelmetDesignStr = driverInfo["HelmetDesignStr"].ToString(),
                SuitDesignStr = driverInfo["SuitDesignStr"].ToString(),
                BodyType = int.Parse(driverInfo["BodyType"].ToString()),
                FaceType = int.Parse(driverInfo["FaceType"].ToString()),
                HelmetType = int.Parse(driverInfo["HelmetType"].ToString()),
                CarNumberDesignStr = driverInfo["CarNumberDesignStr"].ToString(),
                CarSponsor_1 = int.Parse(driverInfo["CarSponsor_1"].ToString()),
                CarSponsor_2 = int.Parse(driverInfo["CarSponsor_2"].ToString()),
                ClubName = driverInfo["ClubName"].ToString(),
                ClubID = int.Parse(driverInfo["ClubID"].ToString()),
                DivisionName = driverInfo["DivisionName"].ToString(),
                DivisionID = int.Parse(driverInfo["DivisionID"].ToString()),
                CurDriverIncidentCount = int.Parse(driverInfo["CurDriverIncidentCount"].ToString()),
                TeamIncidentCount = int.Parse(driverInfo["TeamIncidentCount"].ToString())
            };

        private int GetDriverUserId()
        {
            string userID = _sessionInfo.YamlRoot["DriverInfo"]["DriverUserID"].ToString();

            return int.Parse(userID);
        }

        public FuelViewModel? GetFuelStatistics()
        {
            if (_telemetryOutput is not null && _sessionInfo is not null)
            {
                if (_car is null)
                {
                    _car = new Car(_telemetryOutput, _sessionInfo["DriverInfo"], _me.CarIdx);
                }
                _car.UpdateTelemetry(_telemetryOutput);

                return FuelViewModel.Create(_car);                
            }

            return null;
        }
    }
}
