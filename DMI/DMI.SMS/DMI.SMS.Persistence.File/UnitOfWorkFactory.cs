using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.SMS.Domain.Entities;
using DMI.SMS.IO;
using DMI.SMS.Persistence.File.Repositories;

namespace DMI.SMS.Persistence.File
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private StationInformationRepository _stationInformationRepository;

        public UnitOfWorkFactory()
        {
            _stationInformationRepository = new StationInformationRepository();

            //PopulateWithDummyData();
        }

        public void Initialize(ILogger logger)
        {
            var file = new FileInfo("SMSFileRepository.json");

            if (!file.Exists) return;

            var dataIOHandler = new DataIOHandler();
            IList<StationInformation> stationInformations;
            dataIOHandler.ImportDataFromJson(file.Name, out stationInformations);

            _stationInformationRepository.Load(stationInformations);

            logger?.WriteLine(LogMessageCategory.Information, 
                $"Loaded {stationInformations.Count} station information records from file into memory");
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
            return new UnitOfWork(_stationInformationRepository);
        }

        private void PopulateWithDummyData()
        {
            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                StationName = "Krøllebølle",
                StationIDDMI = 12345,
                ObjectId = 1,
                GdbFromDate = new DateTime(1980, 1, 1),
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59)
            });

            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                StationName = "Tullebølle",
                StationIDDMI = 23456,
                ObjectId = 2,
                GdbFromDate = new DateTime(1980, 1, 1),
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59)
            });

            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                StationName = "Tappernøje",
                StationIDDMI = 34567,
                ObjectId = 3,
                GdbFromDate = new DateTime(1980, 1, 1),
                GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59)
            });

            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                ObjectId = 4,
                StationName = null,
                GdbToDate = new DateTime(2018, 10, 1)
            });

            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                ObjectId = 5,
                StationIDDMI = null,
                GdbToDate = new DateTime(2018, 10, 1)
            });

            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                StationName = "Botanisk Have",
                StationIDDMI = 5735,
                ObjectId = 6,
                GdbFromDate = new DateTime(2010, 1, 1),
                GdbToDate = new DateTime(2019, 6, 1)
            });

            _stationInformationRepository.Add(new Domain.Entities.StationInformation
            {
                StationName = "Livgardens Kaserne",
                StationIDDMI = 5735,
                ObjectId = 6,
                GdbFromDate = new DateTime(2019, 6, 1),
                GdbToDate = new DateTime(9999, 12, 31)
            });
        }
    }
}
