using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.PostgreSQL.Repositories
{
    public class PersonAssociationRepository : Repository<PersonAssociation>, IPersonAssociationRepository
    {
        public PRDbContext PrDbContext
        {
            get { return Context as PRDbContext; }
        }

        public PersonAssociationRepository(
            DbContext context) : base(context)
        {
        }

        public PersonAssociation Get(
            Guid id)
        {
            return PrDbContext.PersonAssociations.Find(id);
        }

        public override void Update(
            PersonAssociation personAssociation)
        {
            var objFromRepository = Get(personAssociation.Id);

            objFromRepository.SubjectPersonId = personAssociation.SubjectPersonId;
            objFromRepository.ObjectPersonId = personAssociation.ObjectPersonId;
            objFromRepository.Description = personAssociation.Description;
        }

        public override void UpdateRange(
            IEnumerable<PersonAssociation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
