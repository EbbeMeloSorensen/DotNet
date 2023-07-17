using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.File.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test_OnePredicateAboutTime_Equal()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var t = new DateTime(2023, 2, 1);
                var observations = unitOfWork.Observations.Find(o => o.Time == t);
            }
        }

        [Fact]
        public void Test_OnePredicateAboutTime_LessThan()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var time = new DateTime(2023, 2, 1);
                var observations = unitOfWork.Observations.Find(o => o.Time < time);
            }
        }

        [Fact]
        public void Test_MultiplePredicates_AnEntireTimeSeries()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var predicates = new List<Expression<Func<Observation, bool>>>();

                var t1 = new DateTime(1953, 1, 1, 0, 0, 0);
                var t2 = new DateTime(2023, 12, 31, 23, 59, 59);

                predicates.Add(o => o.Time >= t1);
                predicates.Add(o => o.Time <= t2);

                var observations = unitOfWork.Observations.Find(predicates);
                observations.Count().Should().Be(392755);
                observations.First().Time.Should().Be(new DateTime(1953, 1, 1, 6, 0, 0));
            }
        }

        [Fact]
        public void Test_Read_All_ObservingFacilities()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                observingFacilities.Count().Should().Be(11177);
                observingFacilities.First().Id.Should().Be(1);
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
                observingFacilities.Count().Should().Be(11177);
            }

            var observingFacility1 = observingFacilities.First();
            TimeSeries timeSeries1;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacility = unitOfWork.ObservingFacilities.GetIncludingTimeSeries(observingFacility1.Id);
                observingFacility.TimeSeries.Count().Should().Be(1);
                observingFacility.TimeSeries.Single().ParamId.Should().Be("temp_dry");
                timeSeries1 = observingFacility.TimeSeries.Single();
            }

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var timeSeries = unitOfWork.TimeSeries.GetIncludingObservations(
                    timeSeries1.Id, new DateTime(1953, 1, 1), new DateTime(1954, 1, 1));

                timeSeries.Observations.Count().Should().Be(392755);
            }
        }

        [Fact]
        public void Test_FindObservingFacility_WithSpecific_StatId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observingFacilities = unitOfWork.ObservingFacilities.Find(_ =>_.StatId == 601100);
                observingFacilities.Count().Should().Be(1);
                observingFacilities.Single().StatId.Should().Be(601100);
            }
        }
    }
}