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

        // Her unders�ger vi, hvad der sker, n�r man bruger samme unit of work til at hente observationer fra samme repo.
        // Det er tilsyneladende ikke uden implikationer
        [Fact]
        public void Test_Read_TimeSeries_With_Observations_2()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            IEnumerable<TimeSeries> timeSeries;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                timeSeries = unitOfWork.TimeSeries.GetAll();
                timeSeries.Count().Should().Be(5);
            }

            var timeSeries1 = timeSeries.First();

            // Bem�rk, at vi har �rene som den ydre l�kke her og det at lave en unit of work som den indre,
            // hvilket indeb�rer, at vi laver en unit of work for hvert �r.
            // Hvis man bytter rundt p� det og dermed genbruger samme unit of work, s� virker det ikke
            for (var year = 1954; year <= 1954; year++)
            {
                using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
                {
                    var startTime = new DateTime(year, 1, 1);
                    var endTime = new DateTime(year, 12, 31, 23, 59, 59, 999);

                    var check = unitOfWork.TimeSeries.GetIncludingObservations(
                        timeSeries1.Id, startTime, endTime);

                    check.Observations.Count().Should().Be(2878);
                }
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