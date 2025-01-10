using Craft.Logging;
using Craft.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PR.Domain.Entities;
using PR.Persistence.Repositories;

namespace PR.Persistence.EntityFrameworkCore.Repositories
{
    public class PersonCommentRepository : Repository<PersonComment>, IPersonCommentRepository
    {
        private PRDbContextBase PrDbContext => Context as PRDbContextBase;

        public PersonCommentRepository(
            DbContext context) : base(context)
        {
        }

        public ILogger Logger { get; }

        public async Task<PersonComment> Get(
            Guid id)
        {
            return await Task.Run(() =>
            {
                var person = PrDbContext.PersonComments.SingleOrDefault(p => p.ID == id);

                if (person == null)
                {
                    throw new InvalidOperationException("Person Comment does not exist");
                }

                return person;
            });
        }

        public Task<IEnumerable<PersonComment>> GetAllVariants(
            Guid id)
        {
            throw new NotImplementedException();
        }

        public override async Task Update(
            PersonComment personComment)
        {
            await Task.Run(() => { });
        }

        public override async Task UpdateRange(
            IEnumerable<PersonComment> people)
        {
            await Task.Run(() => { });
        }

        public override async Task Clear()
        {
            Context.RemoveRange(PrDbContext.People);
            await Context.SaveChangesAsync();
        }
    }
}
