using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Application;
using DMI.StatDB.IO;
using DMI.StatDB.Persistence;

namespace DMI.StatDB.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        private Dictionary<int, Station> _stationCache;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _stationCache = new Dictionary<int, Station>();
        }

        public override void Initialize(
            ILogger logger)
        {
            base.Initialize(logger);

            UnitOfWorkFactory.Initialize(logger);
        }

        public override async Task<bool> CheckConnection()
        {
            return await UnitOfWorkFactory.CheckRepositoryConnection();
        }

        public override int CountAllStations()
        {
            //_logger.WriteLineAndStartStopWatch("Counting people matching search criteria..");

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.Stations.CountAll();

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override int CountStations(
            Expression<Func<Station, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Counting people matching search criteria..");

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.Stations.Count(predicate);

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override int CountStations(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.Stations.Count(predicates);

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override IList<Station> GetAllStations()
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.Stations.GetAll().ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        public override IList<Station> FindStations(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationsFromRepository = unitOfWork.Stations.Find(predicates).ToList();

                stationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        public override IList<Station> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationsFromRepository = unitOfWork.Stations.FindStationsWithPositions(predicate).ToList();

                stationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        public override IList<Station> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationsFromRepository = unitOfWork.Stations.FindStationsWithPositions(predicates).ToList();

                stationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        private Station IncludeInCache(
            Station stationFromRepository)
        {
            if (_stationCache.ContainsKey(stationFromRepository.StatID))
            {
                return _stationCache[stationFromRepository.StatID];
            }

            var station = stationFromRepository.Clone();
            _stationCache[station.StatID] = station;

            return station;
        }
    }
}
