using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.SMS.IO;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.File.Repositories
{
    public class StationInformationRepository : IStationInformationRepository
    {
        private static int _nextId = 1;
        private List<StationInformation> _stationInformations;

        public StationInformationRepository()
        {
            _stationInformations = new List<StationInformation>();
        }

        public StationInformation Get(int id)
        {
            return _stationInformations.Single(p => p.GdbArchiveOid == id);
        }

        public int CountAll()
        {
            return _stationInformations.Count();
        }

        public int Count(Expression<Func<StationInformation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            var temp = _stationInformations;

            foreach (var predicate in predicates)
            {
                temp = temp.Where(predicate.Compile()).ToList();
            }

            return temp.Count();
        }

        public IEnumerable<StationInformation> GetAll()
        {
            return _stationInformations;
        }

        public IEnumerable<StationInformation> Find(Expression<Func<StationInformation, bool>> predicate)
        {
            return _stationInformations.Where(predicate.Compile());
        }

        public IEnumerable<StationInformation> Find(IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            IEnumerable<StationInformation> temp = _stationInformations;

            foreach (var predicate in predicates)
            {
                temp = temp.Where(predicate.Compile());
            }

            return temp;
        }

        public StationInformation SingleOrDefault(Expression<Func<StationInformation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(StationInformation stationInformation)
        {
            stationInformation.GdbArchiveOid = _nextId++;

            _stationInformations.Add(stationInformation);

            UpdateRepositoryFile();
        }

        public void AddRange(IEnumerable<StationInformation> stationInformations)
        {
            throw new NotImplementedException();
        }

        public void Update(StationInformation stationInformation)
        {
            var stationInformationFromRepository = Get(stationInformation.GdbArchiveOid);
            stationInformationFromRepository.CopyAttributes(stationInformation);

            UpdateRepositoryFile();
        }

        public void UpdateRange(IEnumerable<StationInformation> stationInformations)
        {
            var ids = stationInformations.Select(p => p.GdbArchiveOid);
            var stationInformationsFromRepository = Find(s => ids.Contains(s.GdbArchiveOid));

            stationInformationsFromRepository.ToList().ForEach(sRepo =>
            {
                var updatedStationInformation = stationInformations.Single(sUpd => sUpd.GdbArchiveOid == sRepo.GdbArchiveOid);

                sRepo.CopyAttributes(updatedStationInformation);
            });

            UpdateRepositoryFile();
        }

        public void Remove(StationInformation entity)
        {
            _stationInformations = _stationInformations.Where(s => s.GdbArchiveOid != entity.GdbArchiveOid).ToList();

            UpdateRepositoryFile();
        }

        public void RemoveRange(IEnumerable<StationInformation> entities)
        {
            var gdbArchiveOIds = entities.Select(s => s.GdbArchiveOid).ToList();

            _stationInformations = _stationInformations.Where(s => !gdbArchiveOIds.Contains(s.GdbArchiveOid)).ToList();

            UpdateRepositoryFile();
        }

        public void Load(IEnumerable<StationInformation> entities)
        {
            _stationInformations.Clear(); // In case we call load after having done so earlier. Might wanna clean this up..
            _stationInformations.AddRange(entities);
            _nextId = _stationInformations.Count == 0 ? 1 : _stationInformations.Max(si => si.GdbArchiveOid) + 1;

            UpdateRepositoryFile();
        }

        public StationInformation GetStationInformationWithContactPersons(int id)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueObjectId()
        {
            return _stationInformations.Max(s => s.ObjectId) + 1;
        }

        public string GenerateUniqueGlobalId()
        {
            return $"{Guid.NewGuid()}";
        }

        private void UpdateRepositoryFile()
        {
            var dataIOHandler = new DataIOHandler();

            dataIOHandler.ExportDataToJson(
                _stationInformations,
                "SMSFileRepository.json");
        }

        public void RemoveLogically(
            StationInformation stationInformation,
            DateTime transactionTime)
        {
            throw new NotImplementedException();
        }

        public void Supersede(
            StationInformation stationInformation,
            DateTime transactionTime,
            string user)
        {
            throw new NotImplementedException();
        }
    }
}
