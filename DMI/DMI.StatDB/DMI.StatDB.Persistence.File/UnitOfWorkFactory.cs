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
                StatID = 573520,
                Country = "Danmark",
                IcaoId = "Kylling",
                Source = "wmo"
            });
        }
    }
}
