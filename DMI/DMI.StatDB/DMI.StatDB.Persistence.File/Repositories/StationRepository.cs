using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.IO;
using DMI.StatDB.Persistence.Repositories;

namespace DMI.StatDB.Persistence.File.Repositories
{
    public class StationRepository : IStationRepository
    {
        private static int _nextId = 1;
        private List<Station> _stations;

        // Der er behov for at man kan slette
        // elementer herfra, når man sletter stationer. Hvis man gør det fra Entity Framework repositoryet,
        // så sker det tilsyneladende via kaskade deletion
        public IPositionRepository PositionRepository { get; set; }

        public StationRepository()
        {
            _stations = new List<Station>();
        }

        public Station Get(int id)
        {
            return _stations.Single(p => p.StatID == id);
        }

        public int CountAll()
        {
            return _stations.Count();
        }

        public int Count(Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Station, bool>>> predicates)
        {
            var temp = _stations;

            foreach (var predicate in predicates)
            {
                temp = temp.Where(predicate.Compile()).ToList();
            }

            return temp.Count();
        }

        public async Task<IEnumerable<Station>> GetAll()
        {
            return await Task.Run(() => _stations);
        }

        public async Task<IEnumerable<Station>> Find(
            Expression<Func<Station, bool>> predicate)
        {
            return await Task.Run(() => _stations.Where(predicate.Compile()));
        }

        public async Task<IEnumerable<Station>> Find(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            return await Task.Run(() =>
            {
                IEnumerable<Station> temp = _stations;

                foreach (var predicate in predicates)
                {
                    temp = temp.Where(predicate.Compile());
                }

                return temp;
            });
        }

        public Station SingleOrDefault(Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(Station station)
        {
            if (station.StatID == 0)
            {
                // Generate an id for the record
                station.StatID = _nextId++;
            }
            else
            {
                // The record has an id already, make sure it doesn't already exist
                if (_stations.Select(s => s.StatID).Contains(station.StatID))
                {
                    throw new InvalidOperationException("ID already exists");
                }

                // Determine the next id
                _nextId = Math.Max(_nextId, station.StatID + 1);
            }

            _stations.Add(station);

            await UpdateRepositoryFile();
    }

        public Task AddRange(IEnumerable<Station> stations)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Station station)
        {
            var stationFromRepository = Get(station.StatID);
            stationFromRepository.CopyAttributes(station);

            await UpdateRepositoryFile();
        }

        public async Task UpdateRange(
            IEnumerable<Station> stations)
        {
            var ids = stations.Select(p => p.StatID);
            var stationsFromRepository = await Find(s => ids.Contains(s.StatID));

            stationsFromRepository.ToList().ForEach(sRepo =>
            {
                var updatedStation = stations.Single(sUpd => sUpd.StatID == sRepo.StatID);

                sRepo.CopyAttributes(updatedStation);
            });

            await UpdateRepositoryFile();
        }

        public async Task Remove(Station entity)
        {
            _stations = _stations.Where(s => s.StatID != entity.StatID).ToList();

            await UpdateRepositoryFile();
        }

        public async Task RemoveRange(IEnumerable<Station> entities)
        {
            var gdbArchiveOIds = entities.Select(s => s.StatID).ToList();

            _stations = _stations.Where(s => !gdbArchiveOIds.Contains(s.StatID)).ToList();

            await UpdateRepositoryFile();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public async Task Load(IEnumerable<Station> entities)
        {
            _stations.Clear(); // In case we call load after having done so earlier. Might wanna clean this up..
            _stations.AddRange(entities);
            _nextId = _stations.Count == 0 ? 1 : _stations.Max(si => si.StatID) + 1;

            await UpdateRepositoryFile();
        }

        public Station GetWithPositions(int statid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> GetAllStationsWithPositions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> FindStationsWithPositions(Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            IEnumerable<Station> temp = _stations;

            foreach (var predicate in predicates)
            {
                temp = temp.Where(predicate.Compile());
            }

            return temp;
        }

        private async Task UpdateRepositoryFile()
        {
            var dataIOHandler = new DataIOHandler();

            dataIOHandler.ExportDataToJson(
                _stations,
                (await PositionRepository.GetAll()).ToList(),
                @"StatDBFileRepository.json");
        }
    }
}
