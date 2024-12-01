using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;
using Npgsql;

namespace DMI.StatDB.Persistence.Npgsql.Repositories
{
    internal class PositionRepository : IPositionRepository
    {
        public Position Get(decimal id)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            return CountPositions(null);
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
            return await Task.Run(() => GetPositions(null));
        }

        public Task<IEnumerable<Position>> Find(Expression<Func<Position, bool>> predicate)
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

        public Task Add(Position entity)
        {
            throw new NotImplementedException();
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

        public void Load(IEnumerable<Position> entities)
        {
            throw new NotImplementedException();
        }

        private int CountPositions(
            string whereClause)
        {
            var result = -1;

            var query = $"SELECT COUNT(\"statid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.position";

            if (whereClause != null)
            {
                query += $" WHERE {whereClause}";
            }

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        result = reader.GetInt32(0);
                    }

                    reader.Close();
                }
            }

            if (result == -1)
            {
                throw new InvalidOperationException();
            }

            return result;
        }

        private IEnumerable<Position> GetPositions(
            string whereClause)
        {
            var positions = new List<Position>();

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var query = $"SELECT " +
                            "\"statid\", " +
                            "\"entity\", " +
                            "\"start_time\", " +
                            "\"end_time\", " +
                            "\"lat\", " +
                            "\"long\", " +
                            "\"height\" " +
                            $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.position";

                if (whereClause != null)
                {
                    query += $" WHERE {whereClause}";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        positions.Add(new Position
                        {
                            StatID = reader.GetInt32(0),
                            Entity = reader.GetString(1),
                            StartTime = reader.GetDateTime(2),
                            EndTime = reader.IsDBNull(3) ? new DateTime?() : reader.GetDateTime(3),
                            Lat = reader.IsDBNull(4) ? new double?() : reader.GetDouble(4),
                            Long = reader.IsDBNull(5) ? new double?() : reader.GetDouble(5),
                            Height = reader.IsDBNull(6) ? new double?() : reader.GetDouble(6)
                        });
                    }

                    reader.Close();
                }
            }

            return positions;
        }
    }
}
