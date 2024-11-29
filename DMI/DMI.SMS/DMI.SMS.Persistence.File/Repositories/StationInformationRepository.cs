using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public StationInformation GetByGlobalId(
            string globalId)
        {
            throw new NotImplementedException();
        }

        public StationInformation Get(
            int id)
        {
            return _stationInformations.Single(p => p.GdbArchiveOid == id);
        }

        public int CountAll()
        {
            return _stationInformations.Count();
        }

        public int Count(
            Expression<Func<StationInformation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            var temp = _stationInformations;

            foreach (var predicate in predicates)
            {
                temp = temp.Where(predicate.Compile()).ToList();
            }

            return temp.Count();
        }

        public async Task<IEnumerable<StationInformation>> GetAll()
        {
            return await Task.Run(() => _stationInformations);
        }

        public async Task<IEnumerable<StationInformation>> Find(
            Expression<Func<StationInformation, bool>> predicate)
        {
            return await Task.Run(() => _stationInformations.Where(predicate.Compile()));
        }

        public async Task<IEnumerable<StationInformation>> Find(
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            return await Task.Run(() =>
            {
                IEnumerable<StationInformation> temp = _stationInformations;

                foreach (var predicate in predicates)
                {
                    temp = temp.Where(predicate.Compile());
                }

                return temp;
            });
        }

        public StationInformation SingleOrDefault(
            Expression<Func<StationInformation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(
            StationInformation stationInformation)
        {
            await Task.Run(() =>
            {
                stationInformation.GdbArchiveOid = _nextId++;

                _stationInformations.Add(stationInformation);

                UpdateRepositoryFile();
            });
        }

        public Task AddRange(
            IEnumerable<StationInformation> stationInformations)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            StationInformation stationInformation)
        {
            await Task.Run(() =>
            {
                var stationInformationFromRepository = Get(stationInformation.GdbArchiveOid);
                stationInformationFromRepository.CopyAttributes(stationInformation);

                UpdateRepositoryFile();
            });
        }

        public async Task UpdateRange(
            IEnumerable<StationInformation> stationInformations)
        {
            var ids = stationInformations.Select(p => p.GdbArchiveOid);
            var stationInformationsFromRepository = await Find(s => ids.Contains(s.GdbArchiveOid));

            stationInformationsFromRepository.ToList().ForEach(sRepo =>
            {
                var updatedStationInformation = stationInformations.Single(sUpd => sUpd.GdbArchiveOid == sRepo.GdbArchiveOid);

                sRepo.CopyAttributes(updatedStationInformation);
            });

            UpdateRepositoryFile();
        }

        public async Task Remove(
            StationInformation entity)
        {
            await Task.Run(() =>
            {
                _stationInformations = _stationInformations.Where(s => s.GdbArchiveOid != entity.GdbArchiveOid).ToList();

                UpdateRepositoryFile();
            });
        }

        public async Task RemoveRange(
            IEnumerable<StationInformation> entities)
        {
            await Task.Run(() =>
            {
                var gdbArchiveOIds = entities.Select(s => s.GdbArchiveOid).ToList();

                _stationInformations = _stationInformations.Where(s => !gdbArchiveOIds.Contains(s.GdbArchiveOid)).ToList();

                UpdateRepositoryFile();
            });
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<StationInformation> entities)
        {
            _stationInformations.Clear(); // In case we call load after having done so earlier. Might wanna clean this up..
            _stationInformations.AddRange(entities);
            _nextId = _stationInformations.Count == 0 ? 1 : _stationInformations.Max(si => si.GdbArchiveOid) + 1;

            UpdateRepositoryFile();
        }

        public StationInformation GetWithContactPersons(int id)
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

            dataIOHandler.ExportData(
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
