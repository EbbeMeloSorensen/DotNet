using StructureMap;
using Xunit;
using FluentAssertions;
using WIGOS.Persistence.UnitTest;

namespace PR.Persistence.UnitTest
{
    public class UnitTest1
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public UnitTest1()
        {
            var container = Container.For<InstanceScanner>();

            _unitOfWorkFactory = container.GetInstance<IUnitOfWorkFactory>();
            
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