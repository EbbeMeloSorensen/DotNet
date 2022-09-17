using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.Application
{
    public interface IUIDataProvider
    {
        IUnitOfWorkFactory UnitOfWorkFactory { get; }

        void Initialize(ILogger logger);

        Task<bool> CheckConnection();

        void CreatePerson(
            Person person);

        Person GetPerson(Guid id);

        IList<Person> GetAllPeople();

        IList<Person> FindPeople(
            Expression<Func<Person, bool>> predicate);

        IList<Person> FindPeople(
            IList<Expression<Func<Person, bool>>> predicates);

        void ExportData(
            string fileName,
            IList<Expression<Func<Person, bool>>> predicates);
    }
}
