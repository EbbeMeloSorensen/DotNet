﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Npgsql;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class ObservingFacilityRepository : IObservingFacilityRepository
    {
        public Task Add(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
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

        public async Task<IEnumerable<ObservingFacility>> Find(
            Expression<Func<ObservingFacility, bool>> predicate)
        {
            return await Task.Run(() =>
            {
                return GetObservingFacilities($" WHERE {predicate.ToMSSqlString()}");
            });
        }

        public Task<IEnumerable<ObservingFacility>> Find(
            IList<Expression<Func<ObservingFacility, bool>>> predicates)
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
                        var a = 0; // (Det er ikke sikkert, at den er der, f.eks i situationen hvor man antager, at den er der, fordi man har fundet en i sms,,)
                        //throw new InvalidOperationException();
                    }

                    reader.Close();
                }
            }

            return result;
        }

        public async Task<IEnumerable<ObservingFacility>> GetAll()
        {
            return await Task.Run(() =>
            {
                return GetObservingFacilities(null);
            });
        }

        public ObservingFacility GetIncludingTimeSeries(int id)
        {
            var observingFacility = Get(id);

            if (observingFacility == null)
            {
                return null;
            }

            observingFacility.TimeSeries = new List<TimeSeries>();

            var firstYear = 1953;
            var lastYear = DateTime.Now.Year;
            var years = Enumerable.Range(firstYear, lastYear - firstYear + 1);

            foreach (var year in years)
            {
                using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
                {
                    conn.Open();

                    //var basisTable = "temp_wind_radiation";
                    var basisTable = "precip_hum_pressure";
                    //var parameter = "temp_dry";
                    var parameter = "precip_past10min";

                    var query = $"SELECT DISTINCT(\"statid\") " +
                        $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"{basisTable}_{year}\" " +
                        $"WHERE statid = {id} " +
                        $"AND {parameter} is not null";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read() || true)
                        {
                            observingFacility.TimeSeries.Add(
                                TimeSeriesRepository.GenerateTimeSeries(id, parameter)
                            );

                            break; // Sikr lige at du ikke forårsager et leak på denne måde
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

        public Task Remove(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(
            IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        public ObservingFacility SingleOrDefault(
            Expression<Func<ObservingFacility, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Update(ObservingFacility entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(
            IEnumerable<ObservingFacility> entities)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ObservingFacility> GetObservingFacilities(
            string whereClause)
        {
            var observingFacilities = new List<ObservingFacility>();

            var connectionString = ConnectionStringProvider.GetConnectionString();

            using (var conn = new NpgsqlConnection(connectionString))
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
