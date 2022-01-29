using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;
using DMI.StatDB.Domain.Entities;
using DMI.StatDB.Persistence.Repositories;
using Npgsql;

namespace DMI.StatDB.Persistence.Npgsql.Repositories
{
    public class StationRepository : IStationRepository
    {
        private static int _nextId;

        public StationRepository()
        {
            if (_nextId > 0)
            {
                return;
            }

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var count = 0;
                var query = $"SELECT COUNT(\"statid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.station";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }

                    reader.Close();
                }

                if (count == 0)
                {
                    _nextId = 1;
                }
                else
                {
                    query = $"SELECT MAX(\"statid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.station";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _nextId = reader.GetInt32(0) + 1;
                        }

                        reader.Close();
                    }
                }
            }
        }

        public int CountAll()
        {
            return CountStations(null);
        }

        public int Count(
            Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            var whereClause = predicates
                .Select(p => $"({p.ToMSSqlString()})")
                .Aggregate((c, n) => $"{c} AND {n}");

            return CountStations(whereClause);
        }

        public Station Get(
            decimal id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> GetAll()
        {
            return GetStations(null);
        }

        public IEnumerable<Station> Find(
            Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> Find(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            var whereClause = predicates
                .Select(p => $"({p.ToMSSqlString()})")
                .Aggregate((c, n) => $"{c} AND {n}");

            return GetStations(whereClause);
        }

        public Station SingleOrDefault(
            Expression<Func<Station, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(
            Station entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(
            IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(
            Station entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(
            IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(
            Station entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(
            IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<Station> entities)
        {
            throw new NotImplementedException();
        }

        public Station GetStationWithPositions(int statid)
        {
            return GetStationsWithPositions($"statid = {statid}").Single();
        }

        public IEnumerable<Station> GetAllStationsWithPositions()
        {
            return GetStationsWithPositions(null);
        }

        public IEnumerable<Station> FindStationsWithPositions(
            Expression<Func<Station, bool>> predicate)
        {
            var whereClause = predicate.ToMSSqlString();

            return GetStationsWithPositions(whereClause);
        }

        public IEnumerable<Station> FindStationsWithPositions(
            IList<Expression<Func<Station, bool>>> predicates)
        {
            var whereClause = predicates
                .Select(p => $"({p.ToMSSqlString()})")
                .Aggregate((c, n) => $"{c} AND {n}");

            return GetStationsWithPositions(whereClause);
        }

        private int CountStations(
            string whereClause)
        {
            var result = -1;

            var query = $"SELECT COUNT(\"statid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.station";

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

        private IEnumerable<Station> GetStations(
            string whereClause)
        {
            var stations = new List<Station>();

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var query = $"SELECT " +
                    "\"statid\", " +
                    "\"icao_id\", " +
                    "\"country\", " +
                    "\"source\" " +
                    $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.station";

                if (whereClause != null)
                {
                    query += $" WHERE {whereClause}";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stations.Add(new Station
                        {
                            StatID = reader.GetInt32(0),
                            IcaoId = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Country = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Source = reader.IsDBNull(3) ? null : reader.GetString(3)
                        });
                    }

                    reader.Close();
                }
            }

            return stations;
        }

        private IEnumerable<Station> GetStationsWithPositions(
            string whereClause)
        {
            var stations = new List<Station>();

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var query = $"SELECT " +
                    "station.statid, " +
                    "station.icao_id, " +
                    "station.country, " +
                    "station.source, " +
                    "position.start_time, " +
                    "position.end_time, " +
                    "position.lat, " +
                    "position.long, " +
                    "position.height " +
                    $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.station " +
                    "INNER JOIN position ON station.statid=position.statid";

                if (whereClause != null)
                {
                    query += $" WHERE {whereClause}";
                }

                query += " ORDER BY statid, position.start_time";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var statId = reader.GetInt32(0);

                        DateTime? startTime = DateTime.MinValue;
                        DateTime? endTime = DateTime.MaxValue;

                        try
                        {
                            startTime = reader.GetDateTime(4);
                        }
                        catch (InvalidCastException)
                        {
                            // Just swallow it.. 
                            // we assume we're here because the starttime is -infinity

                            // Todo: Find a better way
                        }

                        try
                        {
                            endTime = reader.IsDBNull(5) ? new DateTime?() : reader.GetDateTime(5);
                        }
                        catch (InvalidCastException)
                        {
                            // Just swallow it.. 
                            // we assume we're here because the starttime is infinity

                            // Todo: Find a better way
                        }

                        var lat = reader.IsDBNull(6) ? new double?() : reader.GetDouble(6);
                        var @long = reader.IsDBNull(7) ? new double?() : reader.GetDouble(7);
                        var height = reader.IsDBNull(8) ? new double?() : reader.GetDouble(8);

                        var position = new Position
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            Lat = lat,
                            Long = @long,
                            Height = height
                        };

                        if (!stations.Any() || stations.Last().StatID != statId)
                        {
                            stations.Add(new Station
                            {
                                StatID = statId,
                                IcaoId = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Country = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Source = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Positions = new HashSet<Position>{ position }
                            });
                        }
                        else
                        {
                            stations.Last().Positions.Add(position);
                        }
                    }

                    reader.Close();
                }
            }

            return stations;
        }
    }
}
