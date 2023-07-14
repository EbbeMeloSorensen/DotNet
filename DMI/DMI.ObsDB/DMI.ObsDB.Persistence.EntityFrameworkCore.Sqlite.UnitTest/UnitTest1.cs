using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test_Read_All_ObservingFacilities()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                observingFacilities.Count().Should().Be(2);
            }
        }

        [Fact]
        public void Test_Read_All_TimeSeries()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var timeSeries = unitOfWork.TimeSeries.GetAll();
                timeSeries.Count().Should().Be(3);
            }
        }

        [Fact]
        public void Test_Read_All_Observations()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.GetAll();
                observations.Count().Should().Be(6);
            }
        }

        [Fact]
        public void Test_Read_ObservingFacility_With_TimeSeries()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            IEnumerable<ObservingFacility> observingFacilities;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                observingFacilities.Count().Should().Be(2);
            }

            var observingFacility1 = observingFacilities.First();
            var observingFacility2 = observingFacilities.Skip(1).First();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacility = unitOfWork.ObservingFacilities.GetIncludingTimeSeries(observingFacility1.Id);
                observingFacility.TimeSeries.Count().Should().Be(2);
                observingFacility.TimeSeries.First().ParamId.Should().Be("temp_dry");
                observingFacility.TimeSeries.Skip(1).First().ParamId.Should().Be("wind_speed");
            }
        }

        [Fact]
        public void Test_Read_TimeSeries_With_Observations()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            IEnumerable<TimeSeries> timeSeries;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                timeSeries = unitOfWork.TimeSeries.GetAll();
                timeSeries.Count().Should().Be(3);
            }

            var timeSeries1 = timeSeries.First();
            var timeSeries2 = timeSeries.Skip(1).First();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var ts = unitOfWork.TimeSeries.GetIncludingObservations(timeSeries1.Id);
                ts.Observations.Count().Should().Be(2);
                ts.Observations.First().Value.Should().BeApproximately(32.4, 0.000000001);
                ts.Observations.Skip(1).First().Value.Should().BeApproximately(34.5, 0.000000001);
            }
        }

        [Fact]
        public void Test_Read_Observations_WithSpecific_StatId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.Find(o => o.StatId == 601100);
                observations.Count().Should().Be(4);
            }
        }

        [Fact]
        public void Test_Read_Observations_WithSpecific_ParamId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.Find(o => o.ParamId == "temp_dry");
                observations.Count().Should().Be(4);
            }
        }

        [Fact]
        public void Test_Delete_All_Observations()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Observations.Clear();
                unitOfWork.Complete();
            }
        }

        [Fact]
        public void Test_Delete_All()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Observations.Clear();
                unitOfWork.TimeSeries.Clear();
                unitOfWork.ObservingFacilities.Clear();
                unitOfWork.Complete();
            }
        }
    }
}