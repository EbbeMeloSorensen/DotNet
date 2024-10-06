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

        [Fact]
        public void Get_Latest_Version_Of_Person_Object()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();
            
            // Act
            var person = unitOfWork.People.GetObject(new Guid("11223344-5566-7788-99AA-BBCCDDEEFF03"));

            // Assert
            person.FirstName.Should().Be("Leia");
            person.Surname.Should().Be("Organa");
            person.Nickname.Should().Be(null);
        }

        [Fact]
        public void Get_Latest_Version_Of_Person_Object_Including_Associations()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.GetObjectIncludingPersonAssociations(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF03"));

            // Assert
            person.FirstName.Should().Be("Leia");
            person.Surname.Should().Be("Organa");

            person.ObjectPeople.Count().Should().Be(1);
            person.ObjectPeople.Single().Description.Should().Be("is a parent of");
            person.SubjectPeople.Count().Should().Be(0);
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

        [Fact]
        public void Get_Earlier_Version_Of_Person_Object_Including_Associations()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var person = unitOfWork.People.GetObjectIncludingPersonAssociations(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF03"),
                new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            person.FirstName.Should().Be("Leia");
            person.Surname.Should().Be("Organa");

            person.ObjectPeople.Count().Should().Be(1);
            person.ObjectPeople.Single().Description.Should().Be("is a parent of");
            person.SubjectPeople.Count().Should().Be(1);
            person.ObjectPeople.Single().Description.Should().Be("is a parent of");
        }

        [Fact]
        public void Get_Earlier_Version_Of_Person_Object_Before_Person_Was_Created_Throws()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var act = () => unitOfWork.People.GetObject(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"),
                new DateTime(2007, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().Be("Tried retrieving person that did not exist at the given time");
        }

        [Fact]
        public void Get_Latest_Version_Of_Person_Object_After_Person_Was_Deleted_Throws()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var act = () => unitOfWork.People.GetObject(
                new Guid("11223344-5566-7788-99AA-BBCCDDEEFF00"));

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(act);
            exception.Message.Should().Be("Tried retrieving person that did not exist at the given time");
        }

        [Fact]
        public void Get_Latest_Version_Of_Entire_People_Collection()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAllObjects();

            // Assert
            people.Count().Should().Be(2);
            people.Count(p => p.FirstName == "Leia").Should().Be(1);
            people.Count(p => p.FirstName == "Kylo").Should().Be(1);
        }

        [Fact]
        public void Get_Earlier_Version_Of_Entire_People_Collection_1()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAllObjects(
                new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            people.Count().Should().Be(4);
            people.Count(p => p.FirstName == "Han").Should().Be(1);
            people.Count(p => p.FirstName == "Luke").Should().Be(1);
            people.Count(p => p.FirstName == "Leia").Should().Be(1);
            people.Count(p => p.FirstName == "Ben").Should().Be(1);
        }

        [Fact]
        public void Get_Earlier_Version_Of_Entire_People_Collection_2()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAllObjects(
                new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            // Assert
            people.Count().Should().Be(0);
        }
    }
}