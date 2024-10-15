using FluentAssertions;
using StructureMap;
using WIGOS.Persistence.UnitTest;
using Xunit;
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
            _unitOfWorkFactory.Reseed();

            if (_versionedDB)
            {
                _unitOfWorkFactory = new UnitOfWorkFactoryFacade(_unitOfWorkFactory);
                (_unitOfWorkFactory as UnitOfWorkFactoryFacade).DatabaseTime = null;
            }
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
            people.Count().Should().Be(_versionedDB ? 3 : 8);

            if (!_versionedDB) return;

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
            //_unitOfWorkFactory.DatabaseTime = null;
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Leia").Should().Be(1);
            people.Count(p => p.FirstName == "Kylo").Should().Be(1);
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
            people.Single().FirstName.Should().Be("Leia");
        }
    }
}
