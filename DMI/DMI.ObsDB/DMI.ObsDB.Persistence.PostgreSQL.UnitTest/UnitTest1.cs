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
            ConnectionStringProvider.Initialize("nanoq.dmi.dk", 5432, "statdb", "public", "ebs", "Vm6PAkPh");
            var unitOfWorkFactory = new UnitOfWorkFactory();

            IEnumerable<ObservingFacility> observingFacilities;

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                observingFacilities = unitOfWork.ObservingFacilities.GetAll();
                observingFacilities.Count().Should().Be(15920);
            }
        }
    }
}