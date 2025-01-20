using StructureMap;
using Xunit;
using FluentAssertions;
using PR.Domain.Entities.PR;
using PR.Persistence.Versioned;

namespace PR.Persistence.UnitTest
{
    [Collection("Test Collection 1")]
    public class PersonRepositoryTestBitemporal
    {
        private readonly TestCollectionFixture _fixture;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonRepositoryTestBitemporal(
            TestCollectionFixture fixture)
        {
            _fixture = fixture;

            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.OverrideConnectionString("Data source=people_bitemporal.db");
            _unitOfWorkFactory.Initialize(true);
            _unitOfWorkFactory.Reseed();

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(_unitOfWorkFactory);
            (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = null;
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
