using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities.PR;
using PR.Persistence.Repositories.PR;

namespace PR.Persistence.Versioned.Repositories
{
    public class PersonCommentRepositoryFacade : IPersonCommentRepository
    {
        public ILogger Logger { get; }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<PersonComment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count(
            IList<Expression<Func<PersonComment, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonComment>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonComment>> Find(
            Expression<Func<PersonComment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonComment>> Find(
            IList<Expression<Func<PersonComment, bool>>> predicates)
        {
            throw new NotImplementedException();
        }

        public PersonComment SingleOrDefault(
            Expression<Func<PersonComment, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task Add(
            PersonComment entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRange(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public Task Update(
            PersonComment entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public Task Remove(
            PersonComment entity)
        {
            throw new NotImplementedException();
        }

        public Task RemoveRange(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public Task Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public Task<PersonComment> Get(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PersonComment>> GetAllVariants(
            Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
