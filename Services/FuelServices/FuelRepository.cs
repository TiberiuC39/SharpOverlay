using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace SharpOverlay.Services.FuelServices
{
    public class FuelRepository
    {
        private const string _fileName = "fuelHistory.json";
        private readonly FuelContext _repository;

        public FuelRepository()
        {
            var list = InitializeRepository();

            _repository = list;
        }

        public void AddOrUpdate(AddFuelHistoryDTO newData)
        {
            if (_repository.ByTrack.TryGetValue(newData.TrackId, out RaceHistory trackRaceHistory))
            {
                AddOrUpdateEntry(newData, trackRaceHistory);
            }
            else
            {
                CreateNewEntry(newData);
            }
        }

        private void CreateNewEntry(AddFuelHistoryDTO newData)
        {
            var newModel = new FuelModel()
            {
                Consumption = newData.Consumption,
                LapCount = newData.LapCount,
                LapTime = newData.LapTime.TotalSeconds
            };

            var newRace = new RaceHistory()
            {
                ByCarId = new Dictionary<int, FuelModel>()
                    {
                        { newData.CarId, newModel }
                    }
            };

            _repository.ByTrack.TryAdd(newData.TrackId, newRace);
        }

        private static void AddOrUpdateEntry(AddFuelHistoryDTO newData, RaceHistory trackRaceHistory)
        {
            if (trackRaceHistory.ByCarId.TryGetValue(newData.CarId, out FuelModel entry))
            {
                entry.LapCount = newData.LapCount;
                entry.LapTime = newData.LapTime.TotalSeconds;
                entry.Consumption = newData.Consumption;
            }
            else
            {
                var newModel = new FuelModel()
                {
                    Consumption = newData.Consumption,
                    LapCount = newData.LapCount,
                    LapTime = newData.LapTime.TotalSeconds
                };

                trackRaceHistory.ByCarId.TryAdd(newData.CarId, newModel);
            }
        }

        public FuelModel? Get(int trackId, int carId)
        {
            FuelModel? item = null;

            if (_repository.ByTrack.TryGetValue(trackId, out RaceHistory trackHistory))
            {
                if (trackHistory.ByCarId.TryGetValue(carId, out item));
            }

            return item;
        }

        public void Remove(int trackId)
        {
            _repository.ByTrack.Remove(trackId);
        }

        public void Save()
        {
            string filePath = "../../../" + _fileName;

            if (_repository.ByTrack.Count > 0)
            {
                var options = new JsonSerializerOptions()
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(_repository, options);

                File.WriteAllText(filePath, json);
            }
        }

        public void Update(FuelContext history)
        {
            throw new System.NotImplementedException();
        }

        private FuelContext InitializeRepository()
        {
            string filePath = "../../../" + _fileName;
            FuelContext? context = null;

            try
            {
                string json = File.ReadAllText(filePath);

                if (!string.IsNullOrEmpty(json))
                    context = JsonSerializer.Deserialize<FuelContext>(json);
            }
            catch (FileNotFoundException)
            {
                using (var stream = File.Create(filePath));
            }

            return context ?? new FuelContext();
        }
    }
}
