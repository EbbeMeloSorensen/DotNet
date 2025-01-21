using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain;
using PR.Domain.Entities.PR;
using PR.Persistence.Repositories.PR;

namespace PR.Persistence.Versioned.Repositories
{
    // Generelt når vi returnerer objekter herfra skal vi returnere kloner, da det ellers fucker, når man opdaterer
    public class PersonRepositoryFacade : IPersonRepository
    {
        private bool _returnClonesInsteadOfRepositoryObjects = true;
        private static DateTime _maxDate;

        static PersonRepositoryFacade()
        {
            _maxDate = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }

        private UnitOfWorkFacade _unitOfWorkFacade;

        private IUnitOfWork UnitOfWork => _unitOfWorkFacade.UnitOfWork;
        private DateTime? DatabaseTime => _unitOfWorkFacade.DatabaseTime;
        private DateTime? HistoricalTime => _unitOfWorkFacade.HistoricalTime;
        private bool IncludeCurrentObjects => _unitOfWorkFacade.IncludeCurrentObjects;
        private bool IncludeHistoricalObjects => _unitOfWorkFacade.IncludeHistoricalObjects;

        private DateTime CurrentTime => _unitOfWorkFacade.TransactionTime;

        public PersonRepositoryFacade(
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
            var now = DateTime.UtcNow;
            person.ID = Guid.NewGuid();
            person.Created = now;
            person.Superseded = _maxDate;

            // Todo: Check for initialization på en bedre måde
            if (person.Start.Year == 1)
            {
                person.Start = now;
            }

            if (person.End.Year == 1)
            {
                person.End = _maxDate;
            }

            await UnitOfWork.People.Add(person);
        }

        public async Task AddRange(
            IEnumerable<Person> person)
        {
            throw new NotImplementedException();
        }

        public ILogger Logger { get; }

        public async Task<Person> Get(
            Guid id)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.ID == id
            };

            AddVersionPredicates(predicates, DatabaseTime);
            AddHistoryPredicates(predicates, HistoricalTime);

            var peopleRows = (await UnitOfWork.People.Find(predicates)).ToList();

            if (!peopleRows.Any())
            {
                throw new InvalidOperationException("Person doesn't exist");
            }

            var person = peopleRows
                .OrderBy(_ => _.Start)
                .LastOrDefault();

            if (person == null)
            {
                throw new InvalidOperationException("Person doesn't exist");
            }

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return person.Clone();
            }

            return person;
        }

        public async Task<IEnumerable<Person>> GetAllVariants(
            Guid id)
        {
            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => p.ID == id
            };

            AddVersionPredicates(predicates, DatabaseTime);

            return (await UnitOfWork.People.Find(predicates)).OrderBy(_ => _.Start);
        }

        public async Task<IEnumerable<DateTime>> GetAllValidTimeIntervalExtrema()
        {
            var predicates = new List<Expression<Func<Person, bool>>>();

            AddVersionPredicates(predicates, DatabaseTime);

            List<DateTime> timeStamps = null;

            if (true)
            {
                timeStamps = (await UnitOfWork.People.Find(predicates))
                    .GroupBy(_ => _.ID)
                    .SelectMany(g =>
                    {
                        var timeStampsForFeature = g
                            .SelectMany(_ => new[] { _.Start, _.End })
                            .OrderBy(_ => _);

                        if (timeStampsForFeature.Last().Year == 9999)
                        {
                            return new[] { timeStampsForFeature.First() };
                        }

                        return new[] { timeStampsForFeature.First(), timeStampsForFeature.Last() };
                    })
                    .Distinct()
                    .OrderBy(_ => _)
                    .ToList();
            }
            else
            {
                timeStamps = (await UnitOfWork.People.Find(predicates))
                    .SelectMany(_ => new[] { _.Start, _.End })
                    .Where(_ => _.Year < 9999)
                    .Distinct()
                    .OrderBy(_ => _)
                    .ToList();
            }

            return timeStamps;
        }

        public async Task<IEnumerable<DateTime>> GetAllDatabaseWriteTimes()
        {
            var timeStamps = (await UnitOfWork.People.GetAll())
                .SelectMany(_ => new[]{_.Created, _.Superseded})
                .Distinct()
                .ToList();

            if (timeStamps.Any() && timeStamps.Last().Year == 9999)
            {
                return timeStamps.SkipLast(1);
            }

            return timeStamps;
        }

        public async Task<Person> GetIncludingComments(
            Guid id)
        {
            var person = await Get(id);

            var personCommentPredicates = new List<Expression<Func<PersonComment, bool>>>
            {
                _ => _.PersonID == person.ID
            };

            AddVersionPredicates(personCommentPredicates, DatabaseTime);

            var personCommentRows = (await UnitOfWork.PersonComments.Find(personCommentPredicates)).ToList();

            person.Comments = personCommentRows;

            return person;
        }

        public async Task<IEnumerable<Person>> FindIncludingComments(
            Expression<Func<Person, bool>> predicate)
        {
            var people = await Find(predicate);
            var personIDs = people.Select(_ => _.ID);

            var personCommentPredicates = new List<Expression<Func<PersonComment, bool>>>
            {
                _ => personIDs.Contains(_.PersonID) 
            };

            AddVersionPredicates(personCommentPredicates, DatabaseTime);

            var personCommentRows = (await UnitOfWork.PersonComments.Find(personCommentPredicates)).ToList();

            var personCommentGroups = personCommentRows.GroupBy(_ => _.PersonID);

            people.ToList().ForEach(p =>
            {
                var personCommentGroup = personCommentGroups.SingleOrDefault(_ => _.Key == p.ID);

                if (personCommentGroup != null)
                {
                    p.Comments = personCommentGroup.ToList();
                }
            });

            return people;
        }

        public Task<IEnumerable<Person>> FindIncludingComments(
            IList<Expression<Func<Person, bool>>> predicates)
        {
            throw new NotImplementedException();
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

            var peopleRows = (await UnitOfWork.People.Find(predicates)).ToList();

            Logger?.WriteLine(LogMessageCategory.Information, $"PersonRepositoryFacade: Retrieved {peopleRows.Count} rows");

            if (!peopleRows.Any())
            {
                return new List<Person>();
            }

            // Med denne forsvinder Rey og Anakin, dvs tidligere varianter.
            // Det er fint, men hvis nu vi har sat filteret til at vi kun skal have levende eller kun døde,
            // så skal vi altså filtrere det endnu en gang
            var people = peopleRows
                .GroupBy(p => p.ID)
                .Select(g => g
                    .OrderBy(p => p.Start)
                    .LastOrDefault())
                .Where(p => p != null)
                .ToList();

            var historicalTime = HistoricalTime.HasValue
                ? HistoricalTime.Value
                : new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            if (!IncludeCurrentObjects)
            {
                people = people.Where(_ => _.End < historicalTime).ToList();
            }

            Logger?.WriteLine(LogMessageCategory.Information, $"PersonRepositoryFacade: Retrieved {people.Count} objects");

            return people;
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

            var people = await UnitOfWork.People.Find(predicates);

            if (_returnClonesInsteadOfRepositoryObjects)
            {
                return people.Select(_ => _.Clone()).ToList();
            }

            return people;
        }

        public Person SingleOrDefault(
            Expression<Func<Person, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task Update(
            Person person)
        {
            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(person.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.End = CurrentTime;
            await UnitOfWork.People.Update(objectFromRepository);

            person.ArchiveID = Guid.NewGuid();
            person.Created = CurrentTime;
            person.Superseded = _maxDate;
            person.Start = CurrentTime;
            person.End = _maxDate;
            await UnitOfWork.People.Add(person);
        }

        public async Task UpdateRange(
            IEnumerable<Person> people)
        {
            var ids = people.Select(p => p.ID).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
            {
                p => ids.Contains(p.ID)
            };

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectsFromRepository = (await Find(predicates)).ToList();
            _returnClonesInsteadOfRepositoryObjects = true;

            objectsFromRepository.ForEach(pRepo =>
            {
                pRepo.End = CurrentTime;
            });

            await UnitOfWork.People.UpdateRange(objectsFromRepository);

            people.ToList().ForEach(_ =>
            {
                _.ArchiveID = Guid.NewGuid();
                _.Created = CurrentTime;
                _.Superseded = _maxDate;
                _.Start = CurrentTime;
                _.End = _maxDate;
            });

            await UnitOfWork.People.AddRange(people);
        }

        public async Task Remove(
            Person person)
        {
            var personFromRepo = await GetIncludingComments(person.ID);

            if (personFromRepo.Comments.Any())
            {
                throw new InvalidOperationException("Cant delete person with child rows (comments)");
            }

            _returnClonesInsteadOfRepositoryObjects = false;
            var objectFromRepository = await Get(person.ID);
            _returnClonesInsteadOfRepositoryObjects = true;
            objectFromRepository.Superseded = CurrentTime;
        }

        public async Task RemoveRange(
            IEnumerable<Person> people)
        {
            var ids = people.Select(p => p.ID).ToList();

            var predicates = new List<Expression<Func<Person, bool>>>
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
            await UnitOfWork.People.Clear();
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
                predicates.Add(p => p.Start <= historicalTime);
            }
            else if (IncludeCurrentObjects)
            {
                // ONLY current objects
                predicates.Add(p => p.Start <= historicalTime && p.End > historicalTime);
            }
            else
            {
                throw new InvalidOperationException("Either Include current or include historical should be true");
            }
        }

        private void AddVersionPredicates<T>(
            ICollection<Expression<Func<T, bool>>> predicates,
            DateTime? databaseTime) where T : IVersionedObject
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
