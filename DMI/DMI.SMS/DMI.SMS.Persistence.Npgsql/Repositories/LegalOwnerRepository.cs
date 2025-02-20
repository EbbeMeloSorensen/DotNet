﻿using System.Text;
using System.Linq.Expressions;
using System.Transactions;
using Npgsql;
using Craft.Utils;
using DMI.SMS.Domain.Entities;
using DMI.SMS.Persistence.Repositories;

namespace DMI.SMS.Persistence.Npgsql.Repositories
{
    public class LegalOwnerRepository : ILegalOwnerRepository
    {
        private const string _tableName = "legalowner";

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<LegalOwner, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<LegalOwner, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public LegalOwner Get(
            decimal id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LegalOwner>> GetAll()
        {
            return await Task.Run(() => GetLegalOwners(null));
        }

        public async Task<IEnumerable<LegalOwner>> Find(
            Expression<Func<LegalOwner, bool>> predicate)
        {
            return await Task.Run(() => GetLegalOwners($" WHERE {predicate.ToMSSqlString()}"));
        }

        public async Task<IEnumerable<LegalOwner>> Find(
            IList<Expression<Func<LegalOwner, bool>>> predicates)
        {
            return await Task.Run(() =>
            {
                var whereClauseBuilder = new StringBuilder();

                if (predicates.Count <= 0)
                {
                    return GetLegalOwners(whereClauseBuilder.ToString());
                }

                whereClauseBuilder.Append(" WHERE ");

                whereClauseBuilder.Append(predicates
                    .Select(p => $"({p.ToMSSqlString()})")
                    .Aggregate((c, n) => $"{c} AND {n}"));

                return GetLegalOwners(whereClauseBuilder.ToString());
            });
        }

        public LegalOwner SingleOrDefault(
            Expression<Func<LegalOwner, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        // Bemærk, at man kan indtaste lige hvad man vil i ParentGuid-feltet. Der er ikke nogen constraint, der afviser det.
        public async Task Add(
            LegalOwner entity)
        {
            await Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString());
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                // Todo: Man skal også kunne indsætte alle mulige andre
                using var cmd = new NpgsqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = $"INSERT INTO {ConnectionStringProvider.GetPostgreSqlSchema()}.\"{_tableName}\"(" +
                                  "\"name\", " +
                                  "\"phonenumber\", " +
                                  "\"email\", " +
                                  "\"date\", " +
                                  "\"description\", " +
                                  "\"objectid\", " +
                                  "\"globalid\", " +
                                  "\"parentguid\", " +
                                  "\"gdb_from_date\", " +
                                  "\"gdb_to_date\", " +
                                  "\"created_user\", " +
                                  "\"created_date\", " +
                                  "\"last_edited_user\", " +
                                  "\"last_edited_date\") " +
                                  $"VALUES(@name, @phonenumber, @email, @date, @description, @objectid, @globalid, @parentguid, @gdbfromdate, @gdbtodate, @createduser, @createddate, @lastediteduser, @lastediteddate)";

                cmd.Parameters.AddWithValue("name", !string.IsNullOrEmpty(entity.Name) ? (object)entity.Name : DBNull.Value);
                cmd.Parameters.AddWithValue("phonenumber", !string.IsNullOrEmpty(entity.PhoneNumber) ? (object)entity.PhoneNumber : DBNull.Value);
                cmd.Parameters.AddWithValue("email", !string.IsNullOrEmpty(entity.Email) ? (object)entity.Email : DBNull.Value);
                cmd.Parameters.AddWithValue("date", entity.Date.HasValue ? (object)entity.Date.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("description", !string.IsNullOrEmpty(entity.Description) ? (object)entity.Description : DBNull.Value);
                cmd.Parameters.AddWithValue("objectid", entity.ObjectId);
                cmd.Parameters.AddWithValue("globalid", entity.GlobalId);
                cmd.Parameters.AddWithValue("parentguid", entity.ParentGuid);
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
            IEnumerable<LegalOwner> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            LegalOwner entity)
        {
            await Task.Run(() =>
            {
                using var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString());
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                var nameString = !string.IsNullOrEmpty(entity.Name)
                    ? $"\'{entity.Name}\'"
                    : "null";

                var phoneNumberString = !string.IsNullOrEmpty(entity.PhoneNumber)
                    ? $"\'{entity.PhoneNumber}\'"
                    : "null";

                var emailString = !string.IsNullOrEmpty(entity.Email)
                    ? $"\'{entity.Email}\'"
                    : "null";

                var dateString = entity.Date.HasValue ? $"{entity.Date.Value.AsDateString()}" : "null";

                var descriptionString = !string.IsNullOrEmpty(entity.Description)
                    ? $"\'{entity.Description}\'"
                    : "null";

                var query =
                    $"UPDATE {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName} SET " +
                    $"name={nameString}, " +
                    $"phonenumber={phoneNumberString}, " +
                    $"email={emailString}, " +
                    $"date=\'{dateString}\', " +
                    $"description={descriptionString}, " +
                    $"gdb_to_date='{entity.GdbToDate.AsDateTimeString(true, true)}'" +
                    $" WHERE \"gdb_archive_oid\" = {entity.GdbArchiveOid}";

                using var cmd = new NpgsqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            });
        }

        public Task UpdateRange(
            IEnumerable<LegalOwner> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(
            LegalOwner entity)
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

        public void RemoveLogically(
            LegalOwner legalOwner,
            DateTime transactionTime)
        {
            using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
            {
                conn.Open();
                conn.EnlistTransaction(Transaction.Current);

                var query =
                    $"UPDATE {ConnectionStringProvider.GetPostgreSqlSchema()}.{_tableName} SET " +
                    $"gdb_to_date='{transactionTime.AsDateTimeString(true, true)}'" +
                    $" WHERE \"gdb_archive_oid\" = {legalOwner.GdbArchiveOid}";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Supersede(
            LegalOwner legalOwner,
            DateTime transactionTime,
            string user)
        {
            RemoveLogically(legalOwner, transactionTime);

            var newLegalOwnerRecord = new LegalOwner();
            newLegalOwnerRecord.CopyAttributes(legalOwner);

            newLegalOwnerRecord.GdbFromDate = transactionTime;
            newLegalOwnerRecord.GdbToDate = new DateTime(9999, 12, 31, 23, 59, 59);
            newLegalOwnerRecord.LastEditedUser = user;
            newLegalOwnerRecord.LastEditedDate = transactionTime;

            Add(newLegalOwnerRecord);
        }

        public async Task RemoveRange(
            IEnumerable<LegalOwner> entities)
        {
            await Task.Run(() =>
            {
                entities.ToList().ForEach(cp => Remove(cp));
            });
        }

        public void Load(
            IEnumerable<LegalOwner> entities)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<LegalOwner> GetLegalOwners(
            string whereClause)
        {
            var legalOwners = new List<LegalOwner>();

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
                    "\"name\", " +
                    "\"phonenumber\", " +
                    "\"email\", " +
                    "\"date\", " +
                    "\"description\", " +
                    "\"parentguid\" " +
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
                        legalOwners.Add(new LegalOwner
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
                            Name = reader.IsDBNull(9) ? null : reader.GetString(9),
                            PhoneNumber = reader.IsDBNull(10) ? null : reader.GetString(10),
                            Email = reader.IsDBNull(11) ? null : reader.GetString(11),
                            Date = reader.IsDBNull(12) ? new DateTime?() : reader.GetDateTime(12),
                            Description = reader.IsDBNull(13) ? null : reader.GetString(13),
                            ParentGuid = reader.IsDBNull(14) ? null : reader.GetString(14),
                        });
                    }

                    reader.Close();
                }
            }

            return legalOwners;
        }
    }
}
