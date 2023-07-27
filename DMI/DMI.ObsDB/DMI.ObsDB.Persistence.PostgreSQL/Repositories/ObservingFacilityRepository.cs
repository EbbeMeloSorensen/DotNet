using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Npgsql;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class ObservingFacilityRepository : IObservingFacilityRepository
    {
        private const string _tableName = "station";

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
            throw new NotImplementedException();
        }

        public IEnumerable<ObservingFacility> Find(IList<Expression<Func<ObservingFacility, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public ObservingFacility Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ObservingFacility> GetAll()
        {
            return GetObservingFacilities(null);
        }

        public ObservingFacility GetIncludingTimeSeries(int id)
        {
            throw new NotImplementedException();
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

                var query = $"SELECT " +
                    "\"statid\" " +
                    $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"{_tableName}\"";

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
