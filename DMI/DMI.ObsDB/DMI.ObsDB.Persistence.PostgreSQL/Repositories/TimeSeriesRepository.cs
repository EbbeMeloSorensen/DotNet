using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Npgsql;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class TimeSeriesRepository : ITimeSeriesRepository
    {
        private static int _nextId;
        private static Dictionary<int, int> _idMap;
        private static Dictionary<int, Tuple<int, string>> _stationIdAndParamIdMap;

        static TimeSeriesRepository()
        {
            _nextId = 1;
            _idMap = new Dictionary<int, int>();
            _stationIdAndParamIdMap = new Dictionary<int, Tuple<int, string>>();
        }

        public void Add(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<TimeSeries, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeSeries> Find(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeSeries> Find(IList<Expression<Func<TimeSeries, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public TimeSeries Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TimeSeries> GetAll()
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(int id)
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(int id, DateTime startTime)
        {
            throw new NotImplementedException();
        }

        public TimeSeries GetIncludingObservations(
            int id, 
            DateTime startTime, 
            DateTime endTime)
        {
            if (!_stationIdAndParamIdMap.TryGetValue(id, out var statIdAndParamId))
            {
                throw new InvalidOperationException();
            }

            var statId = statIdAndParamId.Item1;
            var paramId = statIdAndParamId.Item2;
            var result = GenerateTimeSeries(statId, paramId);

            var firstYear = startTime.Year;
            var lastYear = endTime.Year;

            if (endTime.Month == 1 &&
                endTime.Day == 1 &&
                endTime.Hour == 0 &&
                endTime.Minute == 0 &&
                endTime.Second == 0)
            {
                lastYear -= 1;
            }

            lastYear = Math.Min(lastYear, DateTime.Now.Year);

            var years = Enumerable.Range(firstYear, lastYear - firstYear + 1);

            var observations = new List<Observation>();

            foreach(var year in years)
            {
                using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
                {
                    conn.Open();

                    var query = $"SELECT \"timeobs\", \"{paramId}\" " +
                        $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"temp_wind_radiation_{year}\" " +
                        $"WHERE statid = {statId} " +
                        "AND best = true";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var time = reader.GetDateTime(0);
                            time = time.ToUniversalTime();

                            if (time >= startTime &&
                                time < endTime)
                            {
                                if (reader.IsDBNull(1))
                                {
                                    continue;
                                }

                                observations.Add(new Observation
                                {
                                    TimeSeriesId = result.Id,
                                    Time = time,
                                    Value = reader.GetDouble(1)
                                });
                            }
                        }
                            
                        reader.Close();
                    }
                }
            }

            result.Observations = observations.OrderBy(_ => _.Time).ToList();

            return result;
        }

        public void Load(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public TimeSeries SingleOrDefault(Expression<Func<TimeSeries, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(TimeSeries entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<TimeSeries> entities)
        {
            throw new NotImplementedException();
        }

        public static TimeSeries GenerateTimeSeries(int statId, string paramId)
        {
            var hashCode = new Tuple<int, string>(statId, paramId).GetHashCode();

            if (!_idMap.TryGetValue(hashCode, out int id))
            {
                id = _nextId++;
                _idMap.Add(hashCode, id);
                _stationIdAndParamIdMap.Add(id, new Tuple<int, string>(statId, paramId));
            }

            return new TimeSeries { Id = id, ParamId = paramId };
        }
    }
}
