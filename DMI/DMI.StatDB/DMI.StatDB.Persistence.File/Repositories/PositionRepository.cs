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
    public class PositionRepository : IPositionRepository
    {
        private List<Position> _positions;

        public IStationRepository StationRepository { get; set; }

        public PositionRepository()
        {
            _positions = new List<Position>();
        }

        public Position Get(decimal id)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Position, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Position>> GetAll()
        {
            return await Task.Run(() => _positions);
        }

        public Task<IEnumerable<Position>> Find(
            Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Position>> Find(IList<Expression<Func<Position, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Position SingleOrDefault(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(
            Position position)
        {
            await Task.Run(() =>
            {
                // Todo: Make sure you don't add a position with the same combination of station id and start date
                var station = StationRepository.Get(position.StatID);

                position.Station = station;

                if (station.Positions == null)
                {
                    station.Positions = new List<Position>();
                }

                station.Positions.Add(position);

                _positions.Add(position);

                UpdateRepositoryFile();
            });
        }

        public Task AddRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public Task Update(Position entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(Position entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<Position> positions)
        {
            _positions.Clear(); // In case we call load after having done so earlier. Might wanna clean this up..
            _positions.AddRange(positions);

            _positions.ForEach(p =>
            {
                p.Station = StationRepository.Get(p.StatID);

                if (p.Station.Positions == null)
                {
                    p.Station.Positions = new List<Position>();
                }

                p.Station.Positions.Add(p);
            });

            UpdateRepositoryFile();
        }

        private async Task UpdateRepositoryFile()
        {
            var dataIOHandler = new DataIOHandler();

            dataIOHandler.ExportDataToJson(
                (await StationRepository.GetAll()).ToList(),
                _positions,
                @"StatDBFileRepository.json");
        }
    }
}
