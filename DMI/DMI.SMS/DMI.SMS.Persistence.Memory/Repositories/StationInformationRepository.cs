using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.Memory.Repositories
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<StationInformation> Find(IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            var temp = _stationInformations;

            foreach (var predicate in predicates)
            {
                temp = temp.Where(predicate.Compile()).ToList();
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
        }

        public void AddRange(IEnumerable<StationInformation> stationInformations)
        {
            throw new NotImplementedException();
        }

        public void Update(StationInformation entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(StationInformation entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<StationInformation> entities)
        {
            _stationInformations.AddRange(entities);
            _nextId = _stationInformations.Count == 0 ? 1 : _stationInformations.Max(si => si.GdbArchiveOid) + 1;
        }

        public StationInformation GetStationInformationWithContactPersons(int id)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueObjectId()
        {
            throw new NotImplementedException();
        }

        public string GenerateUniqueGlobalId()
        {
            throw new NotImplementedException();
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
