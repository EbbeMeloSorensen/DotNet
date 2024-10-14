using FluentAssertions;
using PR.Domain.Entities;
using PR.Persistence.Versioned;
using StructureMap;
using WIGOS.Persistence.UnitTest;
using Xunit;

namespace PR.Persistence.UnitTest
{
    public class PersonRepositoryFacadeTest
    {
        private readonly UnitOfWorkFactoryFacade _unitOfWorkFactory;

        public PersonRepositoryFacadeTest()
        {
            var container = Container.For<InstanceScanner>();

            var unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            unitOfWorkFactory.Reseed();

            _unitOfWorkFactory = new UnitOfWorkFactoryFacade(unitOfWorkFactory);
            _unitOfWorkFactory.DatabaseTime = null;
        }

        [Fact]
        public void CreatePerson()
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
            people.Count().Should().Be(3);

            var expected = new List<string>
            {
                "Bamse",
                "Kylo",
                "Leia"
            };

            people.Select(_ => _.FirstName).OrderBy(_ => _).SequenceEqual(expected).Should().BeTrue();
        }

        [Fact]
        public void GetLatestVersionOfPerson()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.Get(new Guid("11223344-5566-7788-99AA-BBCCDDEEFF03"));

            // Assert
            person.FirstName.Should().Be("Leia");
            person.Surname.Should().Be("Organa");
        }

        [Fact]
        public void GetLatestVersionOfPerson_AfterPersonWasDeleted_Throws()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var act = () => unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().Be("Tried retrieving person that did not exist at the given time");
        }

        [Fact]
        public void GetEarlierVersionOfPerson_1()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

            // Assert
            person.FirstName.Should().Be("Darth");
            person.Surname.Should().Be("Vader");
        }

        [Fact]
        public void GetEarlierVersionOfPerson_2()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

            // Assert
            person.FirstName.Should().Be("Anakin");
            person.Surname.Should().Be("Skywalker");
        }

        [Fact]
        public void GetEarlierVersionOfPerson_BeforePersonWasCreated_Throws()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var act = () => unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().Be("Tried retrieving person that did not exist at the given time");
        }

        [Fact]
        public void GetLatestVersionOfEntirePeopleCollection()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = null;
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Leia").Should().Be(1);
            people.Count(p => p.FirstName == "Kylo").Should().Be(1);

        }

        [Fact]
        public void GetEarlierVersionOfEntirePersonCollection_1()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(4);
            people.Count(p => p.FirstName == "Han").Should().Be(1);
            people.Count(p => p.FirstName == "Luke").Should().Be(1);
            people.Count(p => p.FirstName == "Leia").Should().Be(1);
            people.Count(p => p.FirstName == "Ben").Should().Be(1);
        }

        [Fact]
        public void GetEarlierVersionOfEntirePersonCollection_2()
        {
            // Arrange
            _unitOfWorkFactory.DatabaseTime = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(0);
        }

        [Fact]
        public void FindPersonUsingItsId()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
            var objectIds = new List<Guid>
            {
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF03")    
            };

            // Act
            var people = unitOfWork.People.Find(p => objectIds.Contains(p.ObjectId));

            // Assert
            people.Count().Should().Be(1);
        }
    }
}
