using System.IO;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.File.Repositories;

namespace DMI.StatDB.Persistence.File
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private StationRepository _stationRepository;

        public UnitOfWorkFactory()
        {
            _stationRepository = new StationRepository();

            PopulateWithDummyData();
        }

        public void Initialize(ILogger logger)
        {
            var file = new FileInfo("StatDBFileRepository.json");

            if (!file.Exists) return;

            // Todo: Implement

            //var dataIOHandler = new DataIOHandler();
            //IList<Station> stations;
            //dataIOHandler.ImportDataFromJson(file.Name, out stations);

            //_stationRepository.Load(stations);

            //logger?.WriteLine(LogMessageCategory.Information,
            //    $"Loaded {stations.Count} station records from file into memory");
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
            return new UnitOfWork(_stationRepository);
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
