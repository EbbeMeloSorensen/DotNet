using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using PR.Domain;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.Versioned.Repositories
{
    public class PersonRepositoryFacade : IPersonRepository
    {
        private static DateTime _maxDate;

        static PersonRepositoryFacade()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private UnitOfWorkFacade _unitOfWorkFacade;

        private IUnitOfWork UnitOfWork => _unitOfWorkFacade.UnitOfWork;
        private DateTime? DatabaseTime => _unitOfWorkFacade.DatabaseTime;
        private DateTime? HistoricalTime => _unitOfWorkFacade.HistoricalTime;
        private bool IncludeHistoricalObjects => _unitOfWorkFacade.IncludeHistoricalObjects;

        private DateTime CurrentTime => _unitOfWorkFacade.TransactionTime;

        public PersonRepositoryFacade(
            UnitOfWorkFacade unitOfWorkFacade)
        {
            _unitOfWorkFacade = unitOfWorkFacade;
        }

        public int CountAll()
        {
            throw new NotImplementedException();
        }

        public int Count(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            AddVersionPredicates(predicates, DatabaseTime);

            return UnitOfWork.People.Count(predicates);
        }

        public int Count(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, DatabaseTime);

            return UnitOfWork.People.Count(predicates);
        }

        public async Task Add(
            Person person)
        {
            person.ID = Guid.NewGuid();
            person.Created = DateTime.UtcNow;
            person.Superseded = _maxDate;

            await UnitOfWork.People.Add(person);
        }

        public async Task AddRange(
            IEnumerable<Person> person)
        {
            throw new NotImplementedException();
        }

        public async Task<Person> Get(
            Guid id)
        {
            return await Task.Run(async () =>
            {
                var predicates = new List<Expression<Func<Person, bool>>>
                {
                    p => p.ID == id
                };

                AddVersionPredicates(predicates, DatabaseTime);

                var people = await UnitOfWork.People.Find(predicates);
                var person = people.SingleOrDefault();

                if (person == null)
                {
                    throw new InvalidOperationException("Person does not exist");
                }

                return person;
            });
        }

        public async Task<IEnumerable<Person>> GetAll()
        {
            var predicates = new List<Expression<Func<Person, bool>>>();

            AddVersionPredicates(predicates, DatabaseTime);
            AddHistoryPredicates(predicates, HistoricalTime);

            // Her hiver du ALLE gældende rækker op, uanset hvad deres virkningstidsinterval er.
            // Mon ikke det var smartere kun at hive dem op, der faktisk skærer time Of interest?
            // Ellers filtrerer du jo i memory, hvor det helst skal foregå i databasen.

            // I første omgang får vi ALLE historiske objekter med, og eneste måde at fjerne de ældste er jo netop
            // at gruppere dem efter id og så tage den seneste

            var people = (await UnitOfWork.People.Find(predicates)).ToList();

            if (!people.Any())
            {
                return people;
            }

            return people
                .GroupBy(p => p.ID)
                .Select(g => g
                    .OrderBy(p => p.Start)
                    .LastOrDefault())
                .Where(p => p != null)
                .ToList();
        }

        public async Task<IEnumerable<Person>> Find(
            Expression<Func<Person, bool>> predicate)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                predicate
            };

            return await Find(predicates);
        }

        public async Task<IEnumerable<Person>> Find(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            AddVersionPredicates(predicates, DatabaseTime);
            AddHistoryPredicates(predicates, HistoricalTime);

            return await UnitOfWork.People.Find(predicates);
        }

        public Person SingleOrDefault(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            Person person)
        {
            var objectFromRepository = await Get(person.ID);
            objectFromRepository.Superseded = CurrentTime;

            person.Created = CurrentTime;
            person.Superseded = _maxDate;
            await UnitOfWork.People.Add(person);
        }

        public async Task UpdateRange(
            IEnumerable<Person> people)
        {
            // I guess this doesn't quite work yet - check it out
            var ids = people.Select(p => p.ID).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => ids.Contains(p.ID)
            };

            var objectsFromRepository = (await Find(predicates)).ToList();

            objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);

            var newObjects = people.Select(p =>
            {
                var newObject = p.Clone();
                newObject.Created = CurrentTime;
                newObject.Superseded = _maxDate;

                return newObject;
            });

            await UnitOfWork.People.AddRange(newObjects);
        }

        public async Task Remove(
            Person person)
        {
            var objectFromRepository = await Get(person.ID);
            objectFromRepository.Superseded = CurrentTime;
        }

        public async Task RemoveRange(
            IEnumerable<Person> people)
        {
            await Task.Run(async () => 
            {
                var ids = people.Select(p => p.ID).ToList();

                var predicates = new List<Expression<Func<Person, bool>>>
                {
                    p => ids.Contains(p.ID)
                };

                var objectsFromRepository = (await Find(predicates)).ToList();

                objectsFromRepository.ForEach(p => p.Superseded = CurrentTime);
            });
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Load(
            IEnumerable<Person> people)
        {
            throw new NotImplementedException();
        }

        private void AddHistoryPredicates(
            ICollection<Expression<Func<Person, bool>>> predicates,
            DateTime? historicalTime)
        {
            historicalTime ??= DateTime.UtcNow;

            if (IncludeHistoricalObjects)
            {
                // Bemærk, at vi med denne får alle de virkningstidsintervaller med, der var historiske på pågældende tidspunkt
                predicates.Add(p => p.Start <= historicalTime);
            }
            else
            {
                predicates.Add(p => p.Start <= historicalTime && p.End > historicalTime);
            }
        }

        private void AddVersionPredicates(
            ICollection<Expression<Func<Person, bool>>> predicates,
            DateTime? databaseTime)
        {
            if (databaseTime.HasValue)
            {
                predicates.Add(pa =>
                    pa.Created <= DatabaseTime &&
                    pa.Superseded > DatabaseTime);
            }
            else
            {
                predicates.Add(pa =>
                    pa.Superseded.Year == 9999);
            }
        }
    }
}
