using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DD.Domain;
using DD.Persistence.Repositories;
using Npgsql;

namespace DD.Persistence.Npgsql.Repositories
{
    public class CreatureTypeRepository : ICreatureTypeRepository
    {
        private static string _connectionString;

        private static int _nextId;

        static CreatureTypeRepository()
        {
            _connectionString = ConnectionStringProvider.GetConnectionString();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var query = "SELECT MAX(\"id\") FROM public.\"creature_types\"";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        if (reader.IsDBNull(0))
                        {
                            _nextId = 1;
                        }
                        else
                        {
                            _nextId = reader.GetInt32(0) + 1;
                        }
                    }

                    reader.Close();
                }
            }
        }

        public CreatureType Get(decimal id)
        {
            CreatureType creatureType = null;

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var query = $"SELECT \"id\", \"name\" FROM public.\"creature_types\" WHERE \"id\" = {id}";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        creatureType = new CreatureType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };

                        reader.Close();
                    }
                }
            }

            return creatureType;
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<CreatureType, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<CreatureType, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreatureType> GetAll()
        {
            var creatureTypes = new List<CreatureType>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var query = $"SELECT \"id\", \"name\" FROM public.\"creature_types\"";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        creatureTypes.Add(new CreatureType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }

                    reader.Close();
                }
            }

            return creatureTypes;
        }

        public IEnumerable<CreatureType> Find(Expression<Func<CreatureType, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CreatureType> Find(IList<Expression<Func<CreatureType, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public CreatureType SingleOrDefault(Expression<Func<CreatureType, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(CreatureType entity)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                var commandText = $"INSERT INTO creature_types (id, name, max_hit_points, armor_class, thaco, movement) VALUES ({_nextId}, '{entity.Name}', 10, 6, 14, 5)";

                using (var cmd = new NpgsqlCommand(commandText, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                _nextId++;
            }
        }

        public void AddRange(IEnumerable<CreatureType> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(CreatureType entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<CreatureType> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(CreatureType entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<CreatureType> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<CreatureType> entities)
        {
            throw new NotImplementedException();
        }
    }
}
