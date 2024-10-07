using System;
using System.Collections.Generic;
using System.Linq;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.RepositoryFacades
{
    public class PersonRepositoryFacade
    {
        private IPersonRepository _personRepository;

        public PersonRepositoryFacade(
            IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public Person Get(
            Guid objectId,
            DateTime? databaseTime = null)
        {
            IEnumerable<Person> people;

            if (databaseTime.HasValue)
            {
                people = _personRepository.Find(p => p.ObjectId == objectId &&
                                                    p.Created <= databaseTime &&
                                                    p.Superseded > databaseTime);
            }
            else
            {
                people = _personRepository.Find(p => p.ObjectId == objectId &&
                                                     p.Superseded.Year == 9999);
            }

            var result = people.SingleOrDefault();

            if (result == null)
            {
                throw new InvalidOperationException("Tried retrieving person that did not exist at the given time");
            }

            return result;
        }
    }
}
