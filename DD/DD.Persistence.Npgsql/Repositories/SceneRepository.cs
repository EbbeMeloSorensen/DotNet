using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using DD.Domain;
using DD.Persistence.Repositories;

namespace DD.Persistence.Npgsql.Repositories
{
    public class SceneRepository : ISceneRepository
    {
        public Scene Get(decimal id)
        {
            throw new NotImplementedException();
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<Scene, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(IList<Expression<Func<Scene, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scene> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scene> Find(Expression<Func<Scene, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Scene> Find(IList<Expression<Func<Scene, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Scene SingleOrDefault(Expression<Func<Scene, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(Scene entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<Scene> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(Scene entity)
        {
            throw new NotImplementedException();
        }

        public void UpdateRange(IEnumerable<Scene> entities)
        {
            throw new NotImplementedException();
        }

        public void Remove(Scene entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Scene> entities)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(IEnumerable<Scene> entities)
        {
            throw new NotImplementedException();
        }
    }
}