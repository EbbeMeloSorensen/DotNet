using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public IEnumerable<Position> GetAll()
        {
            return _positions;
        }

        public IEnumerable<Position> Find(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Position> Find(IList<Expression<Func<Position, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Position SingleOrDefault(Expression<Func<Position, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(
            Position position)
        {
            // Todo: Make sure you don't add a position with the same combination of station id and start date
            var station = StationRepository.GetStation(position.StatID);

            position.Station = station;

            if (station.Positions == null)
            {
                station.Positions = new List<Position>();
            }

            station.Positions.Add(position);

            _positions.Add(position);

            UpdateRepositoryFile();
        }

        public void AddRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Position entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Position entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Position> entities)
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
                p.Station = StationRepository.GetStation(p.StatID);

                if (p.Station.Positions == null)
                {
                    p.Station.Positions = new List<Position>();
                }

                p.Station.Positions.Add(p);
            });

            UpdateRepositoryFile();
        }

        private void UpdateRepositoryFile()
        {
            var dataIOHandler = new DataIOHandler();

            dataIOHandler.ExportDataToJson(
                StationRepository.GetAll().ToList(),
                _positions,
                @"StatDBFileRepository.json");
        }
    }
}
