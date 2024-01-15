using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Npgsql;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class ObservingFacilityRepository : IObservingFacilityRepository
    {
        public void Add(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<ObservingFacility, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<ObservingFacility, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObservingFacility> Find(Expression<Func<ObservingFacility, bool>> predicate)
        {
            return GetObservingFacilities($" WHERE {predicate.ToMSSqlString()}");
        }

        public IEnumerable<ObservingFacility> Find(IList<Expression<Func<ObservingFacility, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public ObservingFacility Get(int id)
        {
            ObservingFacility result = null;

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var query = $"SELECT " +
                    "\"statid\" " +
                    $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"stations\" " +
                    $"WHERE statid = {id}";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var statid = reader.GetInt32(0);

                        result = new ObservingFacility
                        {
                            Id = statid,
                            StatId = statid,
                        };
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    reader.Close();
                }
            }

            return result;
        }

        public IEnumerable<ObservingFacility> GetAll()
        {
            return GetObservingFacilities(null);
        }

        public ObservingFacility GetIncludingTimeSeries(int id)
        {
            var observingFacility = Get(id);
            observingFacility.TimeSeries = new List<TimeSeries>();

            var firstYear = 1953;
            var lastYear = DateTime.Now.Year;
            var years = Enumerable.Range(firstYear, lastYear - firstYear + 1);

            var parameters = new HashSet<string>();

            foreach (var year in years)
            {
                using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
                {
                    conn.Open();

                    var query = $"SELECT DISTINCT(\"statid\") " +
                        $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"temp_wind_radiation_{year}\" " +
                        $"WHERE statid = {id} " +
                        "AND temp_dry is not null";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            observingFacility.TimeSeries.Add(
                                TimeSeriesRepository.GenerateTimeSeries(id, "temp_dry")
                            );

                            break; // SIkr lige at du ikke forårsager et leak på denne måde
                        }
                            
                        reader.Close();
                    }
                }
            }

            return observingFacility;
        }

        public void Load(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public ObservingFacility SingleOrDefault(Expression<Func<ObservingFacility, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Update(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ObservingFacility> GetObservingFacilities(
            string whereClause)
        {
            var observingFacilities = new List<ObservingFacility>();

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var query = $"SELECT DISTINCT " +
                    "\"statid\" " +
                    $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"stations\"";

                if (!string.IsNullOrEmpty(whereClause))
                {
                    query += $"{whereClause}";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var statid = reader.GetInt32(0);

                        observingFacilities.Add(new ObservingFacility
                        {
                            Id = statid,
                            StatId = statid,
                        });
                    }

                    reader.Close();
                }
            }

            return observingFacilities;
        }
    }
}
