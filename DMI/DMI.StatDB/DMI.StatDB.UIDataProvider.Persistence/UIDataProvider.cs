﻿using System;
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
        private Dictionary<int, Position> _positionCache;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _stationCache = new Dictionary<int, Station>();
            _positionCache = new Dictionary<int, Position>();
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

        public override async Task<IList<Station>> GetAllStations()
        {
            var stations = new List<Station>();

            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            var stationsFromRepository = (await unitOfWork.Stations.GetAll()).ToList();

            stationsFromRepository.ForEach(s =>
            {
                var cacheStation = IncludeInCache(s);
                stations.Add(cacheStation);
            });

            return stations;
        }

        public override async Task<IList<Position>> GetAllPositions()
        {
            var positions = new List<Position>();

            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            var positionsFromRepository = (await unitOfWork.Positions.GetAll()).ToList();

            positionsFromRepository.ForEach(p =>
            {
                var cachePosition = IncludeInCache(p);
                positions.Add(cachePosition);
            });

            return positions;
        }

        public override async Task<IList<Station>> FindStations(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            var stationsFromRepository = (await unitOfWork.Stations.Find(predicates)).ToList();

            stationsFromRepository.ForEach(s =>
            {
                var cacheStation = IncludeInCache(s);
                stations.Add(cacheStation);
            });

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        public override async Task<IList<Station>> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            var stationsFromRepository = (await unitOfWork.Stations.FindStationsWithPositions(predicate)).ToList();

            stationsFromRepository.ForEach(s =>
            {
                var cacheStation = IncludeInCache(s);
                stations.Add(cacheStation);
            });

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        public override async Task<IList<Station>> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stations = new List<Station>();

            using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            var stationsFromRepository = (await unitOfWork.Stations.FindStationsWithPositions(predicates)).ToList();

            stationsFromRepository.ForEach(s =>
            {
                var cacheStation = IncludeInCache(s);
                stations.Add(cacheStation);
            });

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stations;
        }

        protected override void LoadStations(IList<Station> stations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Stations.Load(stations);
            }
        }

        protected override void LoadPositions(IList<Position> positions)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Positions.Load(positions);
            }
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

        private Position IncludeInCache(
            Position positionFromRepository)
        {
            var hashCode = new Tuple<int, string, DateTime>(
                positionFromRepository.StatID,
                positionFromRepository.Entity,
                positionFromRepository.StartTime).GetHashCode();

            if (_positionCache.ContainsKey(hashCode))
            {
                return _positionCache[hashCode];
            }

            var position = positionFromRepository.Clone();
            _positionCache[hashCode] = position;

            return position;
        }
    }
}
