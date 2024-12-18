using StructureMap;
using Xunit;
using FluentAssertions;
using PR.Domain;
using PR.Domain.Entities;
using PR.Persistence.Versioned;

namespace PR.Persistence.UnitTest
{
    public class PersonRepositoryTestBitemporal
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonRepositoryTestBitemporal()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Initialize(true);
            _unitOfWorkFactory.Reseed();

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(_unitOfWorkFactory);
            (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = null;
        }

        [Fact]
        public async Task CreatePerson()
        {
            // Arrange
            var person = new Person
            {
                FirstName = "Wicket"
            };

            // Act
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            await unitOfWork1.People.Add(person);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var people = await unitOfWork2.People.GetAll();
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
            people.Count(p => p.FirstName == "Wicket").Should().Be(1);
        }

        [Fact]
        public async void GetAllPeople()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(1);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
        }

        [Fact]
        public async void GetPersonById()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("12345678-0000-0000-0000-000000000006");

            // Act
            var person = await unitOfWork.People.Get(id);

            // Assert
            person.FirstName.Should().Be("Rey Skywalker");
        }

        [Fact]
        public async void FindPersonById()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000006")
            };

            // Act
            var people = await unitOfWork.People.Find(p => ids.Contains(p.ID));

            // Assert
            people.Count().Should().Be(1);
            people.Single().FirstName.Should().Be("Rey Skywalker");
        }

        [Fact]
        public async void FindPeopleByID()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();

            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000006")
            };

            // Act
            var people = (await unitOfWork1.People.Find(p => ids.Contains(p.ID))).ToList();

            people.Count.Should().Be(1);
            people.Count(p => p.FirstName == "Rey Skywalker").Should().Be(1);
        }

        [Fact]
        public async void UpdatePerson()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("12345678-0000-0000-0000-000000000006");
            var person1 = await unitOfWork1.People.Get(id);

            person1.FirstName = "Riley";

            // Act
            await unitOfWork1.People.Update(person1);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var person2 = await unitOfWork2.People.Get(id);
            person2.FirstName.Should().Be("Riley");
        }

        [Fact]
        public async void UpdatePeople()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();

            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000006")
            };

            // Act
            var people1 = (await unitOfWork1.People.Find(p => ids.Contains(p.ID))).ToList();

            people1.ForEach(_ => _.FirstName = "Rudy");
            await unitOfWork1.People.UpdateRange(people1);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var people2 = (await unitOfWork2.People.Find(p => ids.Contains(p.ID))).ToList();
            people2.Count.Should().Be(1);
            people2.Count(p => p.FirstName == "Rudy").Should().Be(1);
        }

        [Fact]
        public async void DeletePerson()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            var id = new Guid("12345678-0000-0000-0000-000000000006");
            var person = await unitOfWork1.People.Get(id);

            // Act
            await unitOfWork1.People.Remove(person);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var people = await unitOfWork2.People.GetAll();
            people.Count().Should().Be(0);
        }

        [Fact]
        public async void DeletePeople()
        {
            // Arrange
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();

            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000005"),
                new("12345678-0000-0000-0000-000000000006")
            };

            // Act
            var people = (await unitOfWork1.People.Find(p => ids.Contains(p.ID))).ToList();

            await unitOfWork1.People.RemoveRange(people);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            people = (await unitOfWork2.People.Find(p => ids.Contains(p.ID))).ToList();
            people.Count.Should().Be(0);
        }
    }
}
