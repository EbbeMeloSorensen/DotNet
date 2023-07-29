using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using DMI.ObsDB.Domain.Entities;
using FluentAssertions;

namespace DMI.ObsDB.Persistence.PostgreSQL.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            ConnectionStringProvider.Initialize("nanoq.dmi.dk", 5432, "obsdb", "public", "ebs", "Vm6PAkPh");
            var unitOfWorkFactory = new UnitOfWorkFactory();

            IEnumerable<ObservingFacility> observingFacilities;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                //observingFacilities.Count().Should().Be(15920); // (statdb)
                observingFacilities.Count().Should().Be(37208); // Bemærk, at der tilsyneladende er væsentligt flere i obsdb end i statdb
            }
        }

        [Fact]
        public void Test2_GetExistingObservingFacilityWorks()
        {
            ConnectionStringProvider.Initialize("nanoq.dmi.dk", 5432, "obsdb", "public", "ebs", "Vm6PAkPh");
            var unitOfWorkFactory = new UnitOfWorkFactory();

            ObservingFacility observingFacility1;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacility1 = unitOfWork.ObservingFacilities.Get(601100);
                observingFacility1.StatId.Should().Be(601100);
            }
        }

        [Fact]
        public void Test2_GetExistingObservingFacilityThrows()
        {
            ConnectionStringProvider.Initialize("nanoq.dmi.dk", 5432, "obsdb", "public", "ebs", "Vm6PAkPh");
            var unitOfWorkFactory = new UnitOfWorkFactory();

            ObservingFacility observingFacility1;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacility1 = unitOfWork.ObservingFacilities.Get(601107);
            }
        }

        [Fact]
        public void Test2_GetObservingFacilityIncludingTimeSeries()
        {
            ConnectionStringProvider.Initialize("nanoq.dmi.dk", 5432, "obsdb", "public", "ebs", "Vm6PAkPh");
            var unitOfWorkFactory = new UnitOfWorkFactory();

            ObservingFacility observingFacility;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacility = unitOfWork.ObservingFacilities.GetIncludingTimeSeries(601100);
            }

            observingFacility.StatId.Should().Be(601100);
            observingFacility.TimeSeries.Single().ParamId.Should().Be("temp_dry");

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var timeSeries = unitOfWork.TimeSeries.GetIncludingObservations(
                    observingFacility.TimeSeries.Single().Id, 
                    new DateTime(1953, 1, 1, 0, 0, 0), 
                    new DateTime(1953, 1, 2, 0, 0, 0));

                timeSeries.Observations.Count().Should().Be(6);
            }
        }
    }
}