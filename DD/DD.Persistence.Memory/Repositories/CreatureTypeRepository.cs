using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DD.Domain;
using DD.Persistence.Repositories;

namespace DD.Persistence.Memory.Repositories
{
    public class CreatureTypeRepository : ICreatureTypeRepository
    {
        private static int _nextId = 1;
        private List<CreatureType> _creatureTypes;

        public CreatureTypeRepository()
        {
            _creatureTypes = new List<CreatureType>();
        }

        public CreatureType Get(decimal id)
        {
            return _creatureTypes.Single(p => p.Id == id);
        }

        public int CountAll()
        {
            return _creatureTypes.Count();
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
            return _creatureTypes;
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
            entity.Id = _nextId++;

            _creatureTypes.Add(entity);
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
