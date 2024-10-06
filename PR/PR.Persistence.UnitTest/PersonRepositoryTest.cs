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
            people.Count().Should().Be(5);
        }

        [Fact]
        public void Get_All_Person_Rows()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(4);
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

        [Fact]
        public void Get_Latest_Version_Of_Person_Object()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
            
            // Act
            var person = unitOfWork.People.GetObject(new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

            // Assert
            person.FirstName.Should().Be("Darth");
            person.Surname.Should().Be("Vader");
            person.Nickname.Should().Be(null);
        }

        [Fact]
        public void Get_Earlier_Version_Of_Person_Object()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.GetObject(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            person.FirstName.Should().Be("Anakin");
            person.Nickname.Should().Be("Ani");
        }

    }
}