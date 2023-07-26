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
            ConnectionStringProvider.Initialize("localhost", 5432, "statdb", "public", "postgres", "L1on8Zebra");
            var unitOfWorkFactory = new UnitOfWorkFactory();

            IEnumerable<ObservingFacility> observingFacilities;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                observingFacilities.Count().Should().Be(11177);
            }
        }
    }
}