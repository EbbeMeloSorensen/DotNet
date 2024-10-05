using FluentAssertions;
using StructureMap;
using WIGOS.Domain.Entities.WIGOS.AbstractEnvironmentalMonitoringFacilities;
using Xunit;

namespace WIGOS.Persistence.UnitTest
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
        public void Test1()
        {
            // Arrange
            var observingFacility = new ObservingFacility(
                Guid.NewGuid(),
                DateTime.UtcNow);

            // Act
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.ObservingFacilities.Add(observingFacility);
                unitOfWork.Complete();
            }

            // Assert
            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacilities = unitOfWork.ObservingFacilities.GetAll();

                observingFacilities.Count().Should().Be(1);
            }
        }

        [Fact]
        public void Test2()
        {
            var observingFacility = new ObservingFacility(
                Guid.NewGuid(),
                DateTime.UtcNow);

            using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacilities = unitOfWork.ObservingFacilities.GetAll();

                observingFacilities.Count().Should().Be(1);
            }
        }
    }
}