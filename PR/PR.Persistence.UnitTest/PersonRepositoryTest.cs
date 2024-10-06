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
        public void Test1_Create_Person_Minimal()
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
        public void Test2_Get_All_People()
        {
            // Arrange
            using var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork();

            // Act
            var people = unitOfWork.People.GetAll();

            // Assert
            people.Count().Should().Be(4);
        }
    }
}