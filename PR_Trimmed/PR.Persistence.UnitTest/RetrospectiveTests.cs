using FluentAssertions;
using PR.Persistence.Versioned;
using StructureMap;
using Xunit;
using PR.Persistence.Versioned;

namespace PR.Persistence.UnitTest
{
    public class RetrospectiveTests
    {
        private readonly UnitOfWorkFactoryFacade _unitOfWorkFactory;

        public RetrospectiveTests()
        {
            var container = Container.For<InstanceScanner>();

            var unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            unitOfWorkFactory.OverrideConnectionString("Data source=people_bitemporal.db");
            unitOfWorkFactory.Initialize(true);
            unitOfWorkFactory.Reseed();

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
            (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = null;
        }

        [Fact]
        public async Task GetEarlierVersionOfPerson()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = await unitOfWork.People.Get(
                new Guid("00000005-0000-0000-0000-000000000000"));

            // Assert
            person.FirstName.Should().Be("Chewing Gum");
        }

        [Fact]
        public async Task GetEarlierStateOfPerson()
        {
            // Arrange
            _unitOfWorkFactory.HistoricalTime = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = await unitOfWork.People.Get(
                new Guid("00000004-0000-0000-0000-000000000000"));

            // Assert
            person.FirstName.Should().Be("Anakin Skywalker");
        }

        [Fact]
        public async Task GetEarlierVersionOfPerson_BeforePersonWasCreated_Throws()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("00000004-0000-0000-0000-000000000000");

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () =>
            {
                var person = await unitOfWork1.People.Get(id);
                unitOfWork1.Complete();
            });

            Assert.NotNull(exception);
            exception.Message.Should().Be("Person doesn't exist");
        }

        [Fact]
        public async Task GetEarlierStateOfPerson_BeforePersonExisted_Throws()
        {
            // Arrange
            _unitOfWorkFactory.HistoricalTime = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("00000005-0000-0000-0000-000000000000");

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () =>
            {
                var person = await unitOfWork1.People.Get(id);
                unitOfWork1.Complete();
            });

            Assert.NotNull(exception);
            exception.Message.Should().Be("Person doesn't exist");
        }

        [Fact]
        public async Task GetEarlierVersionOfEntirePersonCollection()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(4);
            people.Count(p => p.FirstName == "Max Rebo").Should().Be(1);
            people.Count(p => p.FirstName == "Poe Dameron").Should().Be(1);
            people.Count(p => p.FirstName == "Chewing Gum").Should().Be(1);
            people.Count(p => p.FirstName == "Rey").Should().Be(1);
        }

        [Fact]
        public async Task GetHistoricStateOfEntirePersonCollection()
        {
            // Arrange
            _unitOfWorkFactory.HistoricalTime = new DateTime(2005, 1, 1, 1, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Chewbacca").Should().Be(1);
            people.Count(p => p.FirstName == "Darth Vader").Should().Be(1);
        }

        [Fact]
        public async Task FindCurrentPeopleExclusively()
        {
            // Arrange
            _unitOfWorkFactory.IncludeHistoricalObjects = false;
            _unitOfWorkFactory.IncludeCurrentObjects = true;
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.Find(_ => _.FirstName.Contains("e"));

            // Assert
            people.Count().Should().Be(4);
            people.Count(p => p.FirstName == "Max Rebo").Should().Be(1);
            people.Count(p => p.FirstName == "Poe Dameron").Should().Be(1);
            people.Count(p => p.FirstName == "Chewbacca").Should().Be(1);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
        }

        [Fact]
        public async Task FindHistoricalPeopleExclusively()
        {
            // Arrange
            _unitOfWorkFactory.IncludeHistoricalObjects = true;
            _unitOfWorkFactory.IncludeCurrentObjects = false;
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.Find(_ => _.FirstName.Contains("e"));

            // Assert
            people.Count().Should().Be(1);
            people.Count(p => p.FirstName == "Darth Vader").Should().Be(1);
        }

        [Fact]
        public async Task FindCurrentAndHistoricalPeople()
        {
            // Arrange
            _unitOfWorkFactory.IncludeHistoricalObjects = true;
            _unitOfWorkFactory.IncludeCurrentObjects = true;
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.Find(_ => _.FirstName.Contains("e"));

            // Assert
            people.Count().Should().Be(5);
            people.Count(p => p.FirstName == "Max Rebo").Should().Be(1);
            people.Count(p => p.FirstName == "Poe Dameron").Should().Be(1);
            people.Count(p => p.FirstName == "Chewbacca").Should().Be(1);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
            people.Count(p => p.FirstName == "Darth Vader").Should().Be(1);
        }

        [Fact]
        public async Task FindHistoricalPeopleForAHistoricState()
        {
            // Arrange
            _unitOfWorkFactory.IncludeHistoricalObjects = true;
            _unitOfWorkFactory.IncludeCurrentObjects = false;
            _unitOfWorkFactory.HistoricalTime = new DateTime(2005, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.Find(_ => _.FirstName.Contains("e"));

            // Assert
            people.Count().Should().Be(0);
        }

        [Fact]
        public async Task GetAllStatesOfAPerson()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.GetAllVariants(new Guid("00000004-0000-0000-0000-000000000000"));

            // Assert
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Anakin Skywalker").Should().Be(1);
            people.Count(p => p.FirstName == "Darth Vader").Should().Be(1);
        }

        [Fact]
        public async Task RetroactivelyCorrectAnEarlierStateOfAPerson()
        {
            // Arrange
            _unitOfWorkFactory.HistoricalTime = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var person = await unitOfWork.People.Get(
                new Guid("00000004-0000-0000-0000-000000000000"));

            person.FirstName = "Ani";

            // Act
            await unitOfWork.People.Correct(person);
            unitOfWork.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();

            var personVariants =
                await unitOfWork2.People.GetAllVariants(new Guid("00000004-0000-0000-0000-000000000000"));

            personVariants.Count().Should().Be(2);
            personVariants.Count(p => p.FirstName == "Ani").Should().Be(1);
            personVariants.Count(p => p.FirstName == "Darth Vader").Should().Be(1);
        }

        [Fact]
        public async Task RetroactivelyDeleteAnEarlierStateOfAPerson()
        {
            // Arrange
            _unitOfWorkFactory.HistoricalTime = new DateTime(2002, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var person = await unitOfWork.People.Get(
                new Guid("00000004-0000-0000-0000-000000000000"));

            // Act
            await unitOfWork.People.Erase(person);
            unitOfWork.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();

            var personVariants =
                await unitOfWork2.People.GetAllVariants(new Guid("00000004-0000-0000-0000-000000000000"));

            personVariants.Count().Should().Be(1);
            personVariants.Count(p => p.FirstName == "Darth Vader").Should().Be(1);
        }

        // Like when registering that John Doe lived a different place in a given time period
        //[Fact]
        //public async Task SqueezeInANewStateOfAPerson()
        //{
        //    // Arrange
        //    // Act
        //    // Assert
        //    throw new NotImplementedException();
        //}
    }
}
