using FluentAssertions;
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
            person.Nickname.Should().Be(null);
        }

        [Fact]
        public void GetLatestVersionOfPerson_AfterPersonWasDeleted_Throws()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var act = () => unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().Be("Tried retrieving person that did not exist at the given time");
        }

        [Fact]
        public void GetEarlierVersionOfPerson_1()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            person.FirstName.Should().Be("Darth");
            person.Surname.Should().Be("Vader");
        }

        [Fact]
        public void GetEarlierVersionOfPerson_2()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                new DateTime(2013, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            person.FirstName.Should().Be("Anakin");
            person.Surname.Should().Be("Skywalker");
        }

        [Fact]
        public void GetEarlierVersionOfPerson_BeforePersonWasCreated_Throws()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var act = () => unitOfWork.People.Get(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().Be("Tried retrieving person that did not exist at the given time");
        }
    }
}
