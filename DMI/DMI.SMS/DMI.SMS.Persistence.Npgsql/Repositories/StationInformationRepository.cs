using System.Text;
using System.Linq.Expressions;
using System.Transactions;
using Npgsql;
using Craft.Utils;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Domain.EntityClassExtensions;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.Npgsql.Repositories
{
    public class StationInformationRepository : IStationInformationRepository
    {
        private const string _tableName = "stationinformation";

        private Dictionary<StationType, int> _stationTypeCodeMap = new Dictionary<StationType, int>
        {
            { StationType.Synop, 0 },
            { StationType.Strømstation, 1 },
            { StationType.SVK_gprs, 2 },
            { StationType.Vandstandsstation, 3 },
            { StationType.GIWS, 4 },
            { StationType.Pluvio, 5 },
            { StationType.SHIP_AWS, 6 },
            { StationType.Temp_ship, 7 },
            { StationType.Lynpejlestation, 8 },
            { StationType.Radar, 9 },
            { StationType.Radiosonde, 10 },
            { StationType.Historisk_stationstype, 11 },
            { StationType.Manuel_nedbør, 12 },
            { StationType.Bølgestation, 13 },
            { StationType.Snestation, 14 }
        };

        private Dictionary<StationOwner, int> _stationOwnerCodeMap = new Dictionary<StationOwner, int>
        {
            { StationOwner.DMI, 0 },
            { StationOwner.SVK, 1 },
            { StationOwner.Havne_Kommuner_mv, 2 },
            { StationOwner.GC_net_Greenland_Climate_data, 3 },
            { StationOwner.Danske_lufthavne, 4 },
            { StationOwner.MITT_GRL_lufthavne, 5 },
            { StationOwner.Vejdirektoratet, 6 },
            { StationOwner.Synop_Aarhus_Uni, 7 },
            { StationOwner.Asiaq, 8 },
            { StationOwner.Kystdirektoratet, 9 },
            { StationOwner.PROMICE_GEUS_PROMICE_net_i_Grønland, 10 },
            { StationOwner.Forsvaret, 11 }
        };

        private Dictionary<Country, int> _countryCodeMap = new Dictionary<Country, int>
        {
            { Country.Denmark, 0 },
            { Country.Greenland, 1 },
            { Country.FaroeIslands, 2 }
        };

        private Dictionary<Status, int> _statusCodeMap = new Dictionary<Status, int>
        {
            { Status.Inactive, 0 },
            { Status.Active, 1 }
        };

        public int CountAll()
        {
            var result = -1;

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var query = $"SELECT COUNT(\"gdb_archive_oid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName}";

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

        public int Count(
            Expression<Func<StationInformation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            var result = -1;

            var whereClauseBuilder = new StringBuilder();

            if (predicates.Count > 0)
            {
                whereClauseBuilder.Append(" WHERE ");

                whereClauseBuilder.Append(predicates
                    .Select(p => $"({p.ToMSSqlString()})")
                    .Aggregate((c, n) => $"{c} AND {n}"));
            }

            var query = $"SELECT COUNT(\"gdb_archive_oid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName}{whereClauseBuilder.ToString()}";

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

        public StationInformation Get(
            int id)
        {
            var stationInformation = GetStationInformations($" WHERE gdb_archive_oid = {id}").SingleOrDefault();

            return stationInformation;
        }

        public StationInformation GetByGlobalId(
            string globalId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<StationInformation>> GetAll()
        {
            return await Task.Run(() => GetStationInformations(null));
        }

        public async Task<IEnumerable<StationInformation>> Find(
            Expression<Func<StationInformation, bool>> predicate)
        {
            return await Task.Run(() => GetStationInformations($" WHERE {predicate.ToMSSqlString()}"));
        }

        public async Task<IEnumerable<StationInformation>> Find(
            IList<Expression<Func<StationInformation, bool>>> predicates)
        {
            return await Task.Run(() =>
            {
                var whereClauseBuilder = new StringBuilder();

                if (predicates.Count <= 0)
                {
                    return GetStationInformations(whereClauseBuilder.ToString());
                }

                whereClauseBuilder.Append(" WHERE ");

                whereClauseBuilder.Append(predicates
                    .Select(p => $"({p.ToMSSqlString()})")
                    .Aggregate((c, n) => $"{c} AND {n}"));

                return GetStationInformations(whereClauseBuilder.ToString());
            });
        }

        public StationInformation SingleOrDefault(
            Expression<Func<StationInformation, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Add(
            StationInformation entity)
        {
            await Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString());
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                // Todo: Man skal også kunne indsætte alle mulige andre
                using var cmd = new NpgsqlCommand();
                // We do it this way because we get an error about having to type cast if we use the AddWithValue instruction
                var shapeString = string.IsNullOrEmpty(entity.Shape) ? "null" : $"'{entity.Shape}'";

                cmd.Connection = conn;
                cmd.CommandText = $"INSERT INTO {ConnectionStringProvider.GetPostgreSqlSchema()}.\"{_tableName}\"(" +
                                  "\"shape\", " +
                                  "\"wgs_long\", " +
                                  "\"wgs_lat\", " +
                                  "\"wmorbsnradio\", " +
                                  "\"wmorbcn\", " +
                                  "\"wmorbsn\", " +
                                  "\"hhp\", " +
                                  "\"hha\", " +
                                  "\"serviceinterval\", " +
                                  "\"si_geo_long\", " +
                                  "\"si_geo_lat\", " +
                                  "\"si_easting\", " +
                                  "\"si_northing\", " +
                                  "\"si_utm\", " +
                                  "\"wmocountrycode\", " +
                                  "\"wigosid\", " +
                                  "\"regionid\", " +
                                  "\"wmostationid\", " +
                                  "\"altstationid\", " +
                                  "\"facilityid\", " +
                                  "\"referencetomaintenanceagreement\", " +
                                  "\"stationid_icao\", " +
                                  "\"comment\", " +
                                  "\"lastvisitdate\", " +
                                  "\"addworkforcedate\", " +
                                  "\"nextservicedate\", " +
                                  "\"lastservicedate\", " +
                                  "\"dateto\", " +
                                  "\"datefrom\", " +
                                  "\"stationowner\", " +
                                  "\"status\", " +
                                  "\"country\", " +
                                  "\"stationtype\", " +
                                  "\"accessaddress\", " +
                                  "\"stationid_dmi\", " +
                                  "\"objectid\", " +
                                  "\"globalid\", " +
                                  "\"stationname\", " +
                                  "\"gdb_from_date\", " +
                                  "\"gdb_to_date\", " +
                                  "\"created_user\", " +
                                  "\"created_date\", " +
                                  "\"last_edited_user\", " +
                                  "\"last_edited_date\") " +
                                  $"VALUES({shapeString}, @wgslong, @wgslat, @wmorbsnradio, @wmorbcn, @wmorbsn, @hhp, @hha, @serviceinterval, @sigeolong, @sigeolat, @sieasting, @sinorthing, @siutm, @wmocountrycode, @wigosid, @regionid, @wmostationid, @altstationid, @facilityid, @referencetomaintenanceagreement, @stationidicao, @comment, @lastvisitdate, @addworkforcedate, @nextservicedate, @lastservicedate, @dateto, @datefrom, @stationowner, @status, @country, @stationtype, @accessaddress, @stationiddmi, @objectid, @globalid, @stationname, @gdbfromdate, @gdbtodate, @createduser, @createddate, @lastediteduser, @lastediteddate)";

                cmd.Parameters.AddWithValue("wgslong", entity.Wgs_long.HasValue ? (object)entity.Wgs_long.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("wgslat", entity.Wgs_lat.HasValue ? (object)entity.Wgs_lat.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("wmorbsnradio", entity.Wmorbsnradio.HasValue ? (object)entity.Wmorbsnradio.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("wmorbcn", entity.Wmorbcn.HasValue ? (object)entity.Wmorbcn.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("wmorbsn", entity.Wmorbsn.HasValue ? (object)entity.Wmorbsn.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("hhp", entity.Hhp.HasValue ? (object)entity.Hhp.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("hha", entity.Hha.HasValue ? (object)entity.Hha.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("serviceinterval", entity.Serviceinterval.HasValue ? (object)entity.Serviceinterval.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("sigeolong", entity.Si_geo_long.HasValue ? (object)entity.Si_geo_long.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("sigeolat", entity.Si_geo_lat.HasValue ? (object)entity.Si_geo_lat.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("sieasting", entity.Si_easting.HasValue ? (object)entity.Si_easting.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("sinorthing", entity.Si_northing.HasValue ? (object)entity.Si_northing.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("siutm", entity.Si_utm.HasValue ? (object)entity.Si_utm.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("wmocountrycode", !string.IsNullOrEmpty(entity.Wmocountrycode) ? (object)entity.Wmocountrycode : DBNull.Value);
                cmd.Parameters.AddWithValue("wigosid", !string.IsNullOrEmpty(entity.Wigosid) ? (object)entity.Wigosid : DBNull.Value);
                cmd.Parameters.AddWithValue("regionid", !string.IsNullOrEmpty(entity.Regionid) ? (object)entity.Regionid : DBNull.Value);
                cmd.Parameters.AddWithValue("wmostationid", !string.IsNullOrEmpty(entity.Wmostationid) ? (object)entity.Wmostationid : DBNull.Value);
                cmd.Parameters.AddWithValue("altstationid", !string.IsNullOrEmpty(entity.Altstationid) ? (object)entity.Altstationid : DBNull.Value);
                cmd.Parameters.AddWithValue("facilityid", !string.IsNullOrEmpty(entity.Facilityid) ? (object)entity.Facilityid : DBNull.Value);
                cmd.Parameters.AddWithValue("referencetomaintenanceagreement", !string.IsNullOrEmpty(entity.Stationid_icao) ? (object)entity.Stationid_icao : DBNull.Value);
                cmd.Parameters.AddWithValue("stationidicao", !string.IsNullOrEmpty(entity.Stationid_icao) ? (object)entity.Stationid_icao : DBNull.Value);
                cmd.Parameters.AddWithValue("comment", !string.IsNullOrEmpty(entity.Comment) ? (object)entity.Comment : DBNull.Value);
                cmd.Parameters.AddWithValue("lastvisitdate", entity.Lastvisitdate.HasValue ? (object)entity.Lastvisitdate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("addworkforcedate", entity.Addworkforcedate.HasValue ? (object)entity.Addworkforcedate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("nextservicedate", entity.Nextservicedate.HasValue ? (object)entity.Nextservicedate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("lastservicedate", entity.Lastservicedate.HasValue ? (object)entity.Lastservicedate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("dateto", entity.DateTo.HasValue ? (object)entity.DateTo.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("datefrom", entity.DateFrom.HasValue ? (object)entity.DateFrom.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("stationowner", entity.StationOwner.HasValue ? (object)_stationOwnerCodeMap[entity.StationOwner.Value] : DBNull.Value);
                cmd.Parameters.AddWithValue("status", entity.Status.HasValue ? (object)_statusCodeMap[entity.Status.Value] : DBNull.Value);
                cmd.Parameters.AddWithValue("country", entity.Country.HasValue ? (object)_countryCodeMap[entity.Country.Value] : DBNull.Value);
                cmd.Parameters.AddWithValue("stationtype", entity.Stationtype.HasValue ? (object)_stationTypeCodeMap[entity.Stationtype.Value] : DBNull.Value);
                cmd.Parameters.AddWithValue("accessaddress", !string.IsNullOrEmpty(entity.AccessAddress) ? (object)entity.AccessAddress : DBNull.Value);
                cmd.Parameters.AddWithValue("stationiddmi", entity.StationIDDMI.HasValue ? (object)entity.StationIDDMI.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("objectid", entity.ObjectId);
                cmd.Parameters.AddWithValue("globalid", entity.GlobalId);
                cmd.Parameters.AddWithValue("stationname", !string.IsNullOrEmpty(entity.StationName) ? (object)entity.StationName : DBNull.Value);
                cmd.Parameters.AddWithValue("gdbfromdate", entity.GdbFromDate);
                cmd.Parameters.AddWithValue("gdbtodate", entity.GdbToDate);
                cmd.Parameters.AddWithValue("createduser", !string.IsNullOrEmpty(entity.CreatedUser) ? (object)entity.CreatedUser : DBNull.Value);
                cmd.Parameters.AddWithValue("createddate", entity.CreatedDate.HasValue ? (object)entity.CreatedDate.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("lastediteduser", !string.IsNullOrEmpty(entity.LastEditedUser) ? (object)entity.LastEditedUser : DBNull.Value);
                cmd.Parameters.AddWithValue("lastediteddate", entity.LastEditedDate.HasValue ? (object)entity.LastEditedDate.Value : DBNull.Value);
                cmd.ExecuteNonQuery();

                // We need to assign the id to the object and pass it back,
                // because the id will be needed for deleting it, etc
                cmd.CommandText = $"SELECT currval(pg_get_serial_sequence('sde.{_tableName}', 'gdb_archive_oid'))";
                var assignedGdbArchiveOid = cmd.ExecuteScalar();
                entity.GdbArchiveOid = Convert.ToInt32(assignedGdbArchiveOid);
            });
        }

        public Task AddRange(
            IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            StationInformation entity)
        {
            await Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString());
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                var stationNameString = !string.IsNullOrEmpty(entity.StationName)
                    ? $"\'{entity.StationName}\'"
                    : "null";

                var stationTypeString = entity.Stationtype.HasValue
                    ? $"'{_stationTypeCodeMap[entity.Stationtype.Value]}'"
                    : "null";

                var countryString = entity.Country.HasValue
                    ? $"{_countryCodeMap[entity.Country.Value]}"
                    : "null";

                var stationOwnerString = entity.StationOwner.HasValue
                    ? $"{_stationOwnerCodeMap[entity.StationOwner.Value]}"
                    : "null";

                var query =
                    $"UPDATE {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName} SET " +
                    $"stationname={stationNameString}, " +
                    $"stationtype={stationTypeString}, " +
                    $"country={countryString}, " +
                    $"stationowner={stationOwnerString}, " +
                    $"gdb_to_date='{entity.GdbToDate.AsDateTimeString(true, true)}'" +
                    $" WHERE \"gdb_archive_oid\" = {entity.GdbArchiveOid}";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            });
        }

        public Task UpdateRange(
            IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(
            StationInformation entity)
        {
            await Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString());
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                var query = $"DELETE FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.\"{_tableName}\" WHERE \"gdb_archive_oid\" = {entity.GdbArchiveOid}";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            });
        }

        public Task RemoveRange(
            IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<StationInformation> entities)
        {
            throw new NotImplementedException();
        }

        public StationInformation GetWithContactPersons(
            int id)
        {
            throw new NotImplementedException();
        }

        public int GenerateUniqueObjectId()
        {
            var result = 1;

            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();

                var count = 0;
                var query = $"SELECT COUNT(\"objectid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName}";
                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }

                    reader.Close();
                }

                if (count > 0)
                {
                    query = $"SELECT MAX(\"objectid\") FROM {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName}";

                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetInt32(0) + 1;
                        }

                        reader.Close();
                    }
                }
            }

            return result;
        }

        public string GenerateUniqueGlobalId()
        {
            return "{" + $"{Guid.NewGuid()}" + "}";
        }

        private IEnumerable<StationInformation> GetStationInformations(
            string whereClause)
        {
            var stationInformations = new List<StationInformation>();

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
                        stationInformations.Add(new StationInformation
                        {
                            GdbArchiveOid = reader.GetInt32(0),
                            GlobalId = reader.IsDBNull(1) ? null : reader.GetString(1),
                            ObjectId = reader.GetInt32(2),
                            CreatedUser = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CreatedDate = reader.IsDBNull(4) ? new DateTime?() : reader.GetDateTime(4),
                            LastEditedUser = reader.IsDBNull(5) ? null : reader.GetString(5),
                            LastEditedDate = reader.IsDBNull(6) ? new DateTime?() : reader.GetDateTime(6),
                            GdbFromDate = reader.GetDateTime(7),
                            GdbToDate = reader.GetDateTime(8),
                            StationName = reader.IsDBNull(9) ? null : reader.GetString(9),
                            StationIDDMI = reader.IsDBNull(10) ? new int?() : reader.GetInt32(10),
                            Stationtype = reader.IsDBNull(11) ? new StationType?() : (StationType)reader.GetInt32(11),
                            AccessAddress = reader.IsDBNull(12) ? null : reader.GetString(12),
                            Country = reader.IsDBNull(13) ? new Country?() : (Country)reader.GetInt32(13),
                            Status = reader.IsDBNull(14) ? new Status?() : (Status)reader.GetInt32(14),
                            DateFrom = reader.IsDBNull(15) ? new DateTime?() : reader.GetDateTime(15),
                            DateTo = reader.IsDBNull(16) ? new DateTime?() : reader.GetDateTime(16),
                            StationOwner = reader.IsDBNull(17) ? new StationOwner?() : (StationOwner)reader.GetInt32(17),
                            Comment = reader.IsDBNull(18) ? null : reader.GetString(18),
                            Stationid_icao = reader.IsDBNull(19) ? null : reader.GetString(19),
                            Referencetomaintenanceagreement = reader.IsDBNull(20) ? null : reader.GetString(20),
                            Facilityid = reader.IsDBNull(21) ? null : reader.GetString(21),
                            Si_utm = reader.IsDBNull(22) ? new int?() : reader.GetInt32(22),
                            Si_northing = reader.IsDBNull(23) ? new double?() : reader.GetDouble(23),
                            Si_easting = reader.IsDBNull(24) ? new double?() : reader.GetDouble(24),
                            Si_geo_lat = reader.IsDBNull(25) ? new double?() : reader.GetDouble(25),
                            Si_geo_long = reader.IsDBNull(26) ? new double?() : reader.GetDouble(26),
                            Serviceinterval = reader.IsDBNull(27) ? new int?() : reader.GetInt32(27),
                            Lastservicedate = reader.IsDBNull(28) ? new DateTime?() : reader.GetDateTime(28),
                            Nextservicedate = reader.IsDBNull(29) ? new DateTime?() : reader.GetDateTime(29),
                            Addworkforcedate = reader.IsDBNull(30) ? new DateTime?() : reader.GetDateTime(30),
                            Lastvisitdate = reader.IsDBNull(31) ? new DateTime?() : reader.GetDateTime(31),
                            Altstationid = reader.IsDBNull(32) ? null : reader.GetString(32),
                            Wmostationid = reader.IsDBNull(33) ? null : reader.GetString(33),
                            Regionid = reader.IsDBNull(34) ? null : reader.GetString(34),
                            Wigosid = reader.IsDBNull(35) ? null : reader.GetString(35),
                            Wmocountrycode = reader.IsDBNull(36) ? null : reader.GetString(36),
                            Hha = reader.IsDBNull(37) ? new double?() : reader.GetDouble(37),
                            Hhp = reader.IsDBNull(38) ? new double?() : reader.GetDouble(38),
                            Wmorbsn = reader.IsDBNull(39) ? new int?() : reader.GetInt32(39),
                            Wmorbcn = reader.IsDBNull(40) ? new int?() : reader.GetInt32(40),
                            Wmorbsnradio = reader.IsDBNull(41) ? new int?() : reader.GetInt32(41),
                            Wgs_lat = reader.IsDBNull(42) ? new double?() : reader.GetDouble(42),
                            Wgs_long = reader.IsDBNull(43) ? new double?() : reader.GetDouble(43),
                            Shape = reader.IsDBNull(44) ? null : reader.GetString(44)
                        });
                    }

                    reader.Close();
                }
            }

            return stationInformations;
        }

        public void RemoveLogically(
            StationInformation stationInformation,
            DateTime transactionTime)
        {
            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                var query =
                    $"UPDATE {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName} SET " +
                    $"gdb_to_date='{transactionTime.AsDateTimeString(true, true)}'" +
                    $" WHERE \"gdb_archive_oid\" = {stationInformation.GdbArchiveOid}";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Supersede(
            StationInformation stationInformation,
            DateTime transactionTime,
            string user)
        {
            RemoveLogically(stationInformation, transactionTime);

            var newStationInformationRecord = new StationInformation();
            newStationInformationRecord.CopyAttributes(stationInformation);

            newStationInformationRecord.GdbFromDate = transactionTime;
            newStationInformationRecord.GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59);
            newStationInformationRecord.LastEditedUser = user;
            newStationInformationRecord.LastEditedDate = transactionTime;

            Add(newStationInformationRecord);
        }
    }
}
