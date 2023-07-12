using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using DMI.ObsDB.Domain.Entities;

namespace DMI.ObsDB.Persistence.File.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test_OnePredicateAboutStatId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.Find(o => o.StatId == 601100);
            }
        }

        [Fact]
        public void Test_OnePredicateAboutParamId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.Find(o => o.ParamId == "temp_dry");
            }
        }

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
        public void Test_MultiplePredicates()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var predicates = new List<Expression<Func<Observation, bool>>>();

                var t1 = new DateTime(1953, 1, 1, 0, 0, 0);
                var t2 = new DateTime(1953, 1, 2, 0, 0, 0);

                predicates.Add(o => o.StatId == 601100);
                predicates.Add(o => o.ParamId == "temp_dry");
                predicates.Add(o => o.Time >= t1);
                predicates.Add(o => o.Time <= t2);

                var observations = unitOfWork.Observations.Find(predicates);
            }
        }
    }
}