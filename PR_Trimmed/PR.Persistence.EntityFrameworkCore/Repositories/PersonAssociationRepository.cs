using Microsoft.EntityFrameworkCore;
using Craft.Persistence.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.Repositories
{
    public class PersonAssociationRepository : Repository<PersonAssociation>, IPersonAssociationRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public PersonAssociationRepository(
            DbContext context) : base(context)
        {
        }

        public PersonAssociation Get(
            Guid id)
        {
            return PrDbContext.PersonAssociations.Find(id);
        }

        public override void Clear()
        {
            Context.RemoveRange(PrDbContext.PersonAssociations);
            Context.SaveChanges();
        }

        public override void Update(
            PersonAssociation personAssociation)
        {
            var objFromRepository = Get(personAssociation.Id);

            objFromRepository.SubjectPersonId = personAssociation.SubjectPersonId;
            objFromRepository.ObjectPersonId = personAssociation.ObjectPersonId;
            objFromRepository.Description = personAssociation.Description;
        }

        public override void UpdateRange(IEnumerable<PersonAssociation> entities)
        {
            throw new NotImplementedException();
        }
    }
}
