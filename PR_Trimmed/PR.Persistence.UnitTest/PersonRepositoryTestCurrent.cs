using StructureMap;
using Xunit;
using FluentAssertions;
using PR.Domain.Entities.PR;

namespace PR.Persistence.UnitTest
{
    [Collection("Test Collection 2")]
    public class PersonRepositoryTestCurrent
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonRepositoryTestCurrent()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.OverrideConnectionString("Data source=people_current.db");
            _unitOfWorkFactory.Initialize(false);
            _unitOfWorkFactory.Reseed();
        }

        [Fact]
        public async Task CreatePerson()
        {
            await Common.CreatePerson(_unitOfWorkFactory);
        }

        [Fact]
        public async Task GetAllPeople()
        {
            await Common.GetAllPeople(_unitOfWorkFactory);
        }

        [Fact]
        public async Task GetPersonById()
        {
            await Common.GetPersonById(_unitOfWorkFactory);
        }

        [Fact]
        public async Task GetPersonIncludingCommentsById()
        {
            await Common.GetPersonIncludingCommentsById(_unitOfWorkFactory);
        }

        [Fact]
        public async Task FindPersonById()
        {
            await Common.FindPersonById(_unitOfWorkFactory);
        }

        [Fact]
        public async Task FindPeopleById()
        {
            await Common.FindPeopleById(_unitOfWorkFactory);
        }

        [Fact]
        public async Task UpdatePerson()
        {
            await Common.UpdatePerson(_unitOfWorkFactory);
        }

        [Fact]
        public async Task UpdatePeople()
        {
            await Common.UpdatePeople(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePerson()
        {
            await Common.DeletePerson(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePeople()
        {
            await Common.DeletePeople(_unitOfWorkFactory);
        }
    }
}
