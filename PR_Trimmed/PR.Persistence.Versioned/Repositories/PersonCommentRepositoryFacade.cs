using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain;
using PR.Domain.Entities.C2IEDM.ObjectItems;
using PR.Domain.Entities.PR;
using PR.Persistence.Repositories.PR;

namespace PR.Persistence.Versioned.Repositories
{
    public class PersonCommentRepositoryFacade : IPersonCommentRepository
    {
        private static DateTime _maxDate;

        static PersonCommentRepositoryFacade()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private UnitOfWorkFacade _unitOfWorkFacade;
        private bool _returnClonesInsteadOfRepositoryObjects = true;

        private IUnitOfWork UnitOfWork => _unitOfWorkFacade.UnitOfWork;
        private DateTime? DatabaseTime => _unitOfWorkFacade.DatabaseTime;
        private DateTime? HistoricalTime => _unitOfWorkFacade.HistoricalTime;

        public ILogger Logger { get; }

        public PersonCommentRepositoryFacade(
            ILogger logger,
            UnitOfWorkFacade unitOfWorkFacade)
        {
            Logger = logger;
            _unitOfWorkFacade = unitOfWorkFacade;
        }

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

        public async Task Add(
            PersonComment personComment)
        {
            var person = (await UnitOfWork.People.Find(_ =>
                    _.ID == personComment.PersonID &&
                    _.Superseded.Year == 9999))
                .OrderBy(_ => _.Start)
                .Last();

            var now = DateTime.UtcNow;
            personComment.ID = Guid.NewGuid();
            personComment.PersonArchiveID = person.ArchiveID;
            personComment.Created = now;
            personComment.Superseded = _maxDate;

            await UnitOfWork.PersonComments.Add(personComment);
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

        public async Task<PersonComment> Get(
            Guid id)
        {
            var predicates = new List<Expression<Func<PersonComment, bool>>>
            {
                p => p.ID == id
            };

            predicates.AddVersionPredicates(DatabaseTime);

            var personComment = (await UnitOfWork.PersonComments.Find(predicates)).SingleOrDefault();

            if (personComment == null)
            {
                throw new InvalidOperationException("Person comment doesn't exist");
            }

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return personComment.Clone();
            }

            return personComment;
        }
    }
}
