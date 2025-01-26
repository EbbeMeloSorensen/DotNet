using StructureMap;
using Xunit;
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
        public async Task FindPersonById_PersonWasDeleted()
        {
            await Common.FindPersonById_PersonWasDeleted(_unitOfWorkFactory);
        }

        [Fact]
        public async Task FindPeopleById()
        {
            await Common.FindPeopleById(_unitOfWorkFactory);
        }

        [Fact]
        public async Task FindPeopleIncludingComments()
        {
            await Common.FindPeopleIncludingComments(_unitOfWorkFactory);
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
        public async Task DeletePerson_WithoutAnyChildObjects()
        {
            await Common.DeletePerson_WithoutAnyChildObjects(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePerson_WithChildObjects_Throws()
        {
            await Common.DeletePerson_WithChildObjects_Throws(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePeople_WithoutAnyChildObjects()
        {
            await Common.DeletePeople_WithoutAnyChildObjects(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePeople_WithChildObjects_Throws()
        {
            await Common.DeletePeople_WithChildObjects_Throws(_unitOfWorkFactory);
        }

        [Fact]
        public async Task CreatePersonComment()
        {
            await Common.CreatePersonComment(_unitOfWorkFactory);
        }

        [Fact]
        public async Task UpdatePersonComment()
        {
            await Common.UpdatePersonComment(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePersonComment()
        {
            await Common.DeletePersonComment(_unitOfWorkFactory);
        }

        [Fact]
        public async Task DeletePersonComments()
        {
            await Common.DeletePersonComments(_unitOfWorkFactory);
        }
    }
}
