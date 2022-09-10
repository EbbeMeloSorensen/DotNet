using System;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;

namespace PR.Application
{
    public interface IUIDataProvider
    {
        void Initialize(ILogger logger);

        Task<bool> CheckConnection();

        void CreatePerson(
            Person person);

        Person GetPerson(Guid id);
    }
}
