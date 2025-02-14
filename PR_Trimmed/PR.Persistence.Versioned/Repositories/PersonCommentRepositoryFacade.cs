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
        private DateTime CurrentTime => _unitOfWorkFacade.TransactionTime;

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

        public async Task<IEnumerable<PersonComment>> GetAll()
        {
            var predicates = new List<Expression<Func<PersonComment, bool>>>();

            predicates.AddVersionPredicates(DatabaseTime);

            var personCommentRows = (await UnitOfWork.PersonComments.Find(predicates)).ToList();

            Logger?.WriteLine(LogMessageCategory.Information, $"PersonCommentRepositoryFacade: Retrieved {personCommentRows.Count} rows");

            return personCommentRows;
        }

        public async Task<IEnumerable<PersonComment>> Find(
            Expression<Func<PersonComment, bool>> predicate)
        {
            var predicates = new List<Expression<Func<PersonComment, bool>>>
            {
                predicate
            };

            return await Find(predicates);
        }

        public async Task<IEnumerable<PersonComment>> Find(
            IList<Expression<Func<PersonComment, bool>>> predicates)
        {
            predicates.AddVersionPredicates(DatabaseTime);

            var personComments = await UnitOfWork.PersonComments.Find(predicates);

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return personComments.Select(_ => _.Clone()).ToList();
            }

            return personComments;
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

        public async Task Update(
            PersonComment personComment)
        {
            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(personComment.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;
            await UnitOfWork.PersonComments.Update(objectFromRepository);

            personComment.ArchiveID = Guid.NewGuid();
            personComment.Created = CurrentTime;
            personComment.Superseded = _maxDate;
            await UnitOfWork.PersonComments.Add(personComment);
        }

        public Task UpdateRange(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Remove(
            PersonComment personComment)
        {
            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(personComment.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;
        }

        public async Task RemoveRange(
            IEnumerable<PersonComment> personComments)
        {
            var ids = personComments.Select(p => p.ID).ToList();

            var predicates = new List<Expression<Func<PersonComment, bool>>>
            {
                p => ids.Contains(p.ID)
            };

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectsFromRepository = (await Find(predicates)).ToList();
            _returnClonesInsteadOfRepositoryObjects = true;

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);
        }

        public async Task Clear()
        {
            await UnitOfWork.PersonComments.Clear();
        }

        public void Load(
            IEnumerable<PersonComment> entities)
        {
            throw new NotImplementedException();
        }
    }
}
