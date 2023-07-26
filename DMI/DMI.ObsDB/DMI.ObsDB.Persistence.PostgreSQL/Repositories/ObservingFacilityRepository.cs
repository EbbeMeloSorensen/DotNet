using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Npgsql;
using DMI.ObsDB.Domain.Entities;
using DMI.ObsDB.Persistence.Repositories;
using System.Configuration;
using System.Diagnostics.Metrics;

namespace DMI.ObsDB.Persistence.PostgreSQL.Repositories
{
    public class ObservingFacilityRepository : IObservingFacilityRepository
    {
        private const string _tableName = "stations_or_something";

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
                    "\"gdb_archive_oid\", " +
                    "\"globalid\", " +
                    "\"objectid\", " +
                    "\"created_user\", " +
                    "\"created_date\", " +
                    "\"last_edited_user\", " +
                    "\"last_edited_date\", " +
                    "\"gdb_from_date\", " +
                    "\"gdb_to_date\", " +
                    "\"stationname\", " +
                    "\"stationid_dmi\", " +
                    "\"stationtype\", " +
                    "\"accessaddress\", " +
                    "\"country\", " +
                    "\"status\", " +
                    "\"datefrom\", " +
                    "\"dateto\", " +
                    "\"stationowner\", " +
                    "\"comment\", " +
                    "\"stationid_icao\", " +
                    "\"referencetomaintenanceagreement\", " +
                    "\"facilityid\", " +
                    "\"si_utm\", " +
                    "\"si_northing\", " +
                    "\"si_easting\", " +
                    "\"si_geo_lat\", " +
                    "\"si_geo_long\", " +
                    "\"serviceinterval\", " +
                    "\"lastservicedate\", " +
                    "\"nextservicedate\", " +
                    "\"addworkforcedate\", " +
                    "\"lastvisitdate\", " +
                    "\"altstationid\", " +
                    "\"wmostationid\", " +
                    "\"regionid\", " +
                    "\"wigosid\", " +
                    "\"wmocountrycode\", " +
                    "\"hha\", " +
                    "\"hhp\", " +
                    "\"wmorbsn\", " +
                    "\"wmorbcn\", " +
                    "\"wmorbsnradio\", " +
                    "\"wgs_lat\", " +
                    "\"wgs_long\", " +
                    "\"shape\"::TEXT " +
                    $"FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName}";

                if (!string.IsNullOrEmpty(whereClause))
                {
                    query += $"{whereClause}";
                }

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        observingFacilities.Add(new ObservingFacility
                        {
                            //GdbArchiveOid = reader.GetInt32(0),
                            //GlobalId = reader.IsDBNull(1) ? null : reader.GetString(1),
                            //ObjectId = reader.GetInt32(2),
                            //CreatedUser = reader.IsDBNull(3) ? null : reader.GetString(3),
                            //CreatedDate = reader.IsDBNull(4) ? new DateTime?() : reader.GetDateTime(4),
                            //LastEditedUser = reader.IsDBNull(5) ? null : reader.GetString(5),
                            //LastEditedDate = reader.IsDBNull(6) ? new DateTime?() : reader.GetDateTime(6),
                            //GdbFromDate = reader.GetDateTime(7),
                            //GdbToDate = reader.GetDateTime(8),
                            //StationName = reader.IsDBNull(9) ? null : reader.GetString(9),
                            //StationIDDMI = reader.IsDBNull(10) ? new int?() : reader.GetInt32(10),
                            //Stationtype = reader.IsDBNull(11) ? new StationType?() : (StationType)reader.GetInt32(11),
                            //AccessAddress = reader.IsDBNull(12) ? null : reader.GetString(12),
                            //Country = reader.IsDBNull(13) ? new Country?() : (Country)reader.GetInt32(13),
                            //Status = reader.IsDBNull(14) ? new Status?() : (Status)reader.GetInt32(14),
                            //DateFrom = reader.IsDBNull(15) ? new DateTime?() : reader.GetDateTime(15),
                            //DateTo = reader.IsDBNull(16) ? new DateTime?() : reader.GetDateTime(16),
                            //StationOwner = reader.IsDBNull(17) ? new StationOwner?() : (StationOwner)reader.GetInt32(17),
                            //Comment = reader.IsDBNull(18) ? null : reader.GetString(18),
                            //Stationid_icao = reader.IsDBNull(19) ? null : reader.GetString(19),
                            //Referencetomaintenanceagreement = reader.IsDBNull(20) ? null : reader.GetString(20),
                            //Facilityid = reader.IsDBNull(21) ? null : reader.GetString(21),
                            //Si_utm = reader.IsDBNull(22) ? new int?() : reader.GetInt32(22),
                            //Si_northing = reader.IsDBNull(23) ? new double?() : reader.GetDouble(23),
                            //Si_easting = reader.IsDBNull(24) ? new double?() : reader.GetDouble(24),
                            //Si_geo_lat = reader.IsDBNull(25) ? new double?() : reader.GetDouble(25),
                            //Si_geo_long = reader.IsDBNull(26) ? new double?() : reader.GetDouble(26),
                            //Serviceinterval = reader.IsDBNull(27) ? new int?() : reader.GetInt32(27),
                            //Lastservicedate = reader.IsDBNull(28) ? new DateTime?() : reader.GetDateTime(28),
                            //Nextservicedate = reader.IsDBNull(29) ? new DateTime?() : reader.GetDateTime(29),
                            //Addworkforcedate = reader.IsDBNull(30) ? new DateTime?() : reader.GetDateTime(30),
                            //Lastvisitdate = reader.IsDBNull(31) ? new DateTime?() : reader.GetDateTime(31),
                            //Altstationid = reader.IsDBNull(32) ? null : reader.GetString(32),
                            //Wmostationid = reader.IsDBNull(33) ? null : reader.GetString(33),
                            //Regionid = reader.IsDBNull(34) ? null : reader.GetString(34),
                            //Wigosid = reader.IsDBNull(35) ? null : reader.GetString(35),
                            //Wmocountrycode = reader.IsDBNull(36) ? null : reader.GetString(36),
                            //Hha = reader.IsDBNull(37) ? new double?() : reader.GetDouble(37),
                            //Hhp = reader.IsDBNull(38) ? new double?() : reader.GetDouble(38),
                            //Wmorbsn = reader.IsDBNull(39) ? new int?() : reader.GetInt32(39),
                            //Wmorbcn = reader.IsDBNull(40) ? new int?() : reader.GetInt32(40),
                            //Wmorbsnradio = reader.IsDBNull(41) ? new int?() : reader.GetInt32(41),
                            //Wgs_lat = reader.IsDBNull(42) ? new double?() : reader.GetDouble(42),
                            //Wgs_long = reader.IsDBNull(43) ? new double?() : reader.GetDouble(43),
                            //Shape = reader.IsDBNull(44) ? null : reader.GetString(44)
                        });
                    }

                    reader.Close();
                }
            }

            return observingFacilities;
        }
    }
}
