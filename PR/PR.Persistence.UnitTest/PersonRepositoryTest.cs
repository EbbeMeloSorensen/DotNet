using StructureMap;
using Xunit;
using FluentAssertions;
using PR.Domain.Entities;
using WIGOS.Persistence.UnitTest;

namespace PR.Persistence.UnitTest
{
    public class PersonRepositoryTest
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public PersonRepositoryTest()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            _unitOfWorkFactory.Reseed();
        }

        [Fact]
        public void Create_Person_Minimal()
        {
            // Arrange
            var person = new Person
            {
                FirstName = "Bamse"
            };

            // Act
            using var unitOfWork1 = _unitOfWorkFactory.GenerateUnitOfWork();
            unitOfWork1.People.Add(person);
            unitOfWork1.Complete();

            // Assert
            using var unitOfWork2 = _unitOfWorkFactory.GenerateUnitOfWork();
            var people = unitOfWork2.People.GetAll();
            people.Count().Should().Be(8);
        }

        // Herfra checker vi funktionalitet, der generelt IKKE skal være tilgængelige for brugeren

        [Fact]
        public void Get_All_Person_Rows()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(7);
        }

        [Fact]
        public void Get_Non_Superseded_Person_Row()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var id = unitOfWork.People.GetAll()
                .Single(_ => _.FirstName == "Darth")
                .Id;

            // Act
            var person = unitOfWork.People.Get(id);

            // Assert
            person.FirstName.Should().Be("Darth");
            person.Surname.Should().Be("Vader");
            person.Nickname.Should().Be(null);
        }

        [Fact]
        public void Get_Superseded_Person_Row()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            var id = unitOfWork.People.GetAll()
                .Single(_ => _.FirstName == "Anakin")
                .Id;

            // Act
            var person = unitOfWork.People.Get(id);

            // Assert
            person.FirstName.Should().Be("Anakin");
            person.Surname.Should().Be("Skywalker");
            person.Nickname.Should().Be("Ani");
        }

        // Herfra checker vi ting, der skal være tilgængelige for brugeren
    }
}