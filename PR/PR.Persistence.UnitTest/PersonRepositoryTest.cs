using StructureMap;
using Xunit;
using FluentAssertions;
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
            _unitOfWorkFactory.Reset();
        }

        [Fact]
        public void Test1_Get_All_People()
        {
            // Act
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var people = unitOfWork.People.GetAll();
                people.Count().Should().Be(66);
            }
        }

        [Fact]
        public void Test2_Create_Person()
        {
            // Act
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
            }
        }
    }
}