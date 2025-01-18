using System;
using System.Threading.Tasks;
using Craft.Logging;
using Craft.Persistence;
using PR.Domain.Entities.PR;

namespace PR.Persistence.Repositories.PR
{
    public interface IPersonCommentRepository : IRepository<PersonComment>
    {
        ILogger Logger { get; }

        Task<PersonComment> Get(
            Guid id);
    }
}
