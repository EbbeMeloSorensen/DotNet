using System;
using System.Linq;
using System.Collections.Generic;
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
                observingFacilities.Count().Should().Be(5);
            }
        }

        [Fact]
        public void Test_Read_All_TimeSeries()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var timeSeries = unitOfWork.TimeSeries.GetAll();
                timeSeries.Count().Should().Be(5);
            }
        }

        [Fact]
        public void Test_Read_All_Observations()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.GetAll();
                observations.Count().Should().Be(4598521);
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
                observingFacilities.Count().Should().Be(5);
            }

            var observingFacility1 = observingFacilities.First();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacility = unitOfWork.ObservingFacilities.GetIncludingTimeSeries(observingFacility1.Id);
                observingFacility.TimeSeries.Count().Should().Be(1);
                observingFacility.TimeSeries.First().ParamId.Should().Be("temp_dry");
                //observingFacility.TimeSeries.Skip(1).First().ParamId.Should().Be("wind_speed");
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
                timeSeries.Count().Should().Be(5);
            }

            var timeSeries1 = timeSeries.First();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var ts = unitOfWork.TimeSeries.GetIncludingObservations(
                    timeSeries1.Id,
                    new DateTime(1970, 7, 24, 0, 0, 0),
                    new DateTime(1975, 7, 24, 7, 9, 20));

                ts.Observations.Count().Should().Be(14385);
                ts.Observations.First().Value.Should().BeApproximately(9.0, 0.0001);
                ts.Observations.Skip(1).First().Value.Should().BeApproximately(9.4, 0.0001);
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