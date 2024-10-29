using StructureMap;
using Xunit;
using FluentAssertions;
using PR.Domain.Entities;
using PR.Persistence.Versioned;

namespace PR.Persistence.UnitTest
{
    public class PersonRepositoryFacadeTest
    {
        private const bool _versionedDB = true;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonRepositoryFacadeTest()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Initialize(_versionedDB);
            _unitOfWorkFactory.Reseed();

            if (!_versionedDB) return;

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(_unitOfWorkFactory);
            (_unitOfWorkFactory as UnitOfWorkFactoryFacade)!.DatabaseTime = null;
        }

        [Fact]
        public async void FindPersonById()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
            var ids = new List<Guid>
            {
                new("12345678-0000-0000-0000-000000000005")
            };

            // Act
            var people = await unitOfWork.People.Find(p => ids.Contains(p.ID));

            // Assert
            people.Count().Should().Be(1);
            people.Single().FirstName.Should().Be("Chewbacca");
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
            people.Count().Should().Be(5);
        }

        //[Fact]
        //public void GetPerson_AfterPersonWasDeleted_Throws()
        //{
        //    // Arrange
        //    using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

        //    // Act
        //    var act = () => unitOfWork.People.Get(
        //        new Guid("12345678-0000-0000-0000-000000000005"));

        //    // Assert
        //    var exception = Assert.Throws<InvalidOperationException>(act);
        //    exception.Message.Should().Be("Person does not exist");
        //}

        [Fact]
        public async void GetAllPeople()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = await unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(4);
            people.Count(p => p.FirstName == "Rey").Should().Be(1);
            people.Count(p => p.FirstName == "Finn").Should().Be(1);
            people.Count(p => p.FirstName == "Poe").Should().Be(1);
            people.Count(p => p.FirstName == "Chewbacca").Should().Be(1);
        }
    }
}
