using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.IO;
using DMI.StatDB.Persistence.File.Repositories;

namespace DMI.StatDB.Persistence.File
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private StationRepository _stationRepository;
        private PositionRepository _positionRepository;

        public UnitOfWorkFactory()
        {
            _stationRepository = new StationRepository();
            _positionRepository = new PositionRepository();

            _stationRepository.PositionRepository = _positionRepository;
            _positionRepository.StationRepository = _stationRepository;

            //PopulateWithDummyData();
        }

        public void Initialize(ILogger logger)
        {
            var file = new FileInfo(@"StatDBFileRepository.json");

            if (!file.Exists) return;

            var dataIOHandler = new DataIOHandler();
            IList<Station> stations;
            IList<Position> positions;
            dataIOHandler.ImportDataFromJson(file.FullName, out stations, out positions);

            _stationRepository.Load(stations);
            _positionRepository.Load(positions);

            logger?.WriteLine(LogMessageCategory.Information,
                $"Loaded {stations.Count} station records and {positions.Count} position records from file into memory");
        }

        public async Task<bool> CheckRepositoryConnection()
        {
            return await Task.Run(() =>
            {
                return true;
            });
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork(_stationRepository, _positionRepository);
        }

        private void PopulateWithDummyData()
        {
            _stationRepository.Add(new Station
            {
                StatID = 573520,
                Country = "Danmark",
                IcaoId = "Bamse",
                Source = "ing"
            });

            _stationRepository.Add(new Station
            {
                StatID = 573620,
                Country = "Danmark",
                IcaoId = "Kylling",
                Source = "wmo"
            });

            _positionRepository.Add(new Position
            {
                StartTime = new DateTime(1972, 3, 17),
                EndTime = new DateTime(1975, 7, 24),
                Height = 2,
                Lat = 10,
                Long = 11,
                StatID = 573520
            });

            _positionRepository.Add(new Position
            {
                StartTime = new DateTime(1975, 7, 24),
                EndTime = new DateTime?(),
                Height = 3,
                Lat = 11,
                Long = 12,
                StatID = 573520
            });

            _positionRepository.Add(new Position
            {
                StartTime = new DateTime(1972, 3, 17),
                EndTime = new DateTime(1980, 6, 13),
                Height = 4,
                Lat = 12,
                Long = 13,
                StatID = 573520
            });
        }
    }
}
