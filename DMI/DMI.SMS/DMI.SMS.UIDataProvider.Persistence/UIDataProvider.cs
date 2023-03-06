using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using DMI.SMS.Application;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.SMS.IO;
using DMI.SMS.Persistence;

namespace DMI.SMS.UIDataProvider.Persistence
{
    public class UIDataProvider : UIDataProviderBase
    {
        private Dictionary<int, StationInformation> _stationInformationCache;

        public IUnitOfWorkFactory UnitOfWorkFactory { get; }

        public UIDataProvider(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler) : base(dataIOHandler)
        {
            UnitOfWorkFactory = unitOfWorkFactory;

            _stationInformationCache = new Dictionary<int, StationInformation>();
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

        public override void CreateStationInformation(
            StationInformation stationInformation,
            bool introduceNewObject)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                if (introduceNewObject)
                {
                    stationInformation.ObjectId = unitOfWork.StationInformations.GenerateUniqueObjectId();
                    stationInformation.GlobalId = unitOfWork.StationInformations.GenerateUniqueGlobalId();
                }

                // Her har de ikke fået gdb_from_date og gdb_to_date, men i databasen har rækken det .. hvor faen laves det så henne?
                // Ok det er sgu en lille smule morsomt, for det ser vitterligt ud som om at det er selve databasen, der altså klarer det
                // måske under anvendelse af en stored procedure eller hvad faen ved jeg...
                // Det viser altså også, at det er en smule farligt at gå bag om ryggen på de sædvanlige interfaces, når man laver noget i databasen
                // I hvert fald så bliver du vel nødt til at finde ud af, HVORDAN databasen tildeler værdier til de felter.. 
                // Det er altså også en lille smule sjovt at se, at det gdb_archive-oid, som databasen genererer, det er ikke bare den maksimalt forekommende + 1
                // det er noget andet den gør.
                // Hmmm man kan tilsyneladende godt sætte nogle værdier for gdb_from_date og gdb_to_date - dvs det er kun hvis man IKKE sender dem med at den finder på noget selv
                unitOfWork.StationInformations.Add(stationInformation);
                unitOfWork.Complete();
            }

            var cacheObj = stationInformation.Clone();
            _stationInformationCache[stationInformation.GdbArchiveOid] = cacheObj;

            OnStationInformationCreated(cacheObj);
        }

        public override StationInformation GetStationInformation(
            int id)
        {
            if (_stationInformationCache.ContainsKey(id))
            {
                return _stationInformationCache[id];
            }

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformation = unitOfWork.StationInformations.Get(id).Clone();
                _stationInformationCache[id] = stationInformation;
                return stationInformation;
            }
        }

        public override IList<StationInformation> GetAllStationInformations()
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<StationInformation>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.StationInformations.GetAll().ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        public override void DeleteStationInformations(
            IList<StationInformation> stationInformations)
        {
            var gdbArchiveOIDs = stationInformations.Select(s => s.GdbArchiveOid);

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsForDeletion = unitOfWork.StationInformations.Find(s => gdbArchiveOIDs.Contains(s.GdbArchiveOid));

                unitOfWork.StationInformations.RemoveRange(stationInformationsForDeletion);
                unitOfWork.Complete();

                foreach (var stationInformationForDeletion in stationInformationsForDeletion)
                {
                    RemoveFromCache(stationInformationForDeletion);
                }
            }
        }

        public override void DeleteStationInformation(
            StationInformation stationInformation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationForDeletion = unitOfWork.StationInformations.Get(stationInformation.GdbArchiveOid);

                unitOfWork.StationInformations.Remove(stationInformationForDeletion);
                unitOfWork.Complete();

                RemoveFromCache(stationInformationForDeletion);
            }
        }

        public override void DeleteAllStationInformations()
        {
            throw new NotImplementedException();
        }

        public override void UpdateStationInformation(
            StationInformation stationInformation)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.StationInformations.Update(stationInformation);
                unitOfWork.Complete();
            }

            // Update the the cache too
            var cacheObj = GetStationInformation(stationInformation.GdbArchiveOid);
            cacheObj.CopyAttributes(stationInformation);
        }

        public override void UpdateStationInformations(
            IList<StationInformation> stationInformations)
        {
            //_logger.WriteLineAndStartStopWatch($"Updating people..");

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.StationInformations.UpdateRange(stationInformations);
                unitOfWork.Complete();
            }

            // Update the the cache too
            foreach (var station in stationInformations)
            {
                var cacheObj = GetStationInformation(station.GdbArchiveOid);
                cacheObj.CopyAttributes(station);
            }

            OnStationInformationsUpdated(stationInformations);

            //_logger.StopStopWatchAndWriteLine("Completed updating people");
        }

        public override int CountAllStationInformations()
        {
            //_logger.WriteLineAndStartStopWatch("Counting people matching search criteria..");

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.StationInformations.CountAll();

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override int CountStationInformations(
            Expression<Func<StationInformation, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Counting people matching search criteria..");

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.StationInformations.Count(predicate);

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override int CountStationInformations(
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var count = unitOfWork.StationInformations.Count(predicates);

                //_logger.StopStopWatchAndWriteLine("Completed counting people");

                return count;
            }
        }

        public override IList<StationInformation> FindStationInformations(
            Expression<Func<StationInformation, bool>> predicate)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<StationInformation>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.StationInformations.Find(predicate).ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        public override IList<StationInformation> FindStationInformations(
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            //_logger.WriteLineAndStartStopWatch("Retrieving people matching search criteria..");

            var stationInformations = new List<StationInformation>();

            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                var stationInformationsFromRepository = unitOfWork.StationInformations.Find(predicates).ToList();

                stationInformationsFromRepository.ForEach(s =>
                {
                    var cacheStationInformation = IncludeInCache(s);
                    stationInformations.Add(cacheStationInformation);
                });
            }

            //_logger.StopStopWatchAndWriteLine("Completed retrieving people");

            return stationInformations;
        }

        protected override void LoadStationInformations(
            IList<StationInformation> stationInformations)
        {
            using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
            {
                // We reset the values of the identity field because we're not allowed to explicitly set it.
                // Notice that this implies that the rows are assigned new primary keys which may be practically unacceptable.
                unitOfWork.StationInformations.Load(stationInformations.Select(_ => 
                {
                    _.GdbArchiveOid = 0;
                    return _;
                }));
            }

            // We don't update the cache, because it might be a lot of data
            // On the contrary, we clear the cache, so we're not looking at obsolete data
            _stationInformationCache.Clear();
        }

        private StationInformation IncludeInCache(
            StationInformation stationInformationFromRepository)
        {
            if (_stationInformationCache.ContainsKey(stationInformationFromRepository.GdbArchiveOid))
            {
                return _stationInformationCache[stationInformationFromRepository.GdbArchiveOid];
            }

            var stationInformation = stationInformationFromRepository.Clone();
            _stationInformationCache[stationInformation.GdbArchiveOid] = stationInformation;

            return stationInformation;
        }

        private void RemoveFromCache(StationInformation stationInformation)
        {
            if (!_stationInformationCache.ContainsKey(stationInformation.GdbArchiveOid)) return;

            _stationInformationCache.Remove(stationInformation.GdbArchiveOid);
        }
    }
}
