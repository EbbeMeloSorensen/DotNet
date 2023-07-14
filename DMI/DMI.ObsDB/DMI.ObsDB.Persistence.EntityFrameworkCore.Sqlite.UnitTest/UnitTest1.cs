using System.Linq;
using Xunit;
using FluentAssertions;

namespace DMI.ObsDB.Persistence.EntityFrameworkCore.Sqlite.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test_Read_All()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.GetAll();
                observations.Count().Should().Be(6);
            }
        }

        [Fact]
        public void Test_Read_StatId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.Find(o => o.StatId == 601100);
                observations.Count().Should().Be(4);
            }
        }

        [Fact]
        public void Test_Read_ParamId()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var observations = unitOfWork.Observations.Find(o => o.ParamId == "temp_dry");
                observations.Count().Should().Be(4);
            }
        }

        [Fact]
        public void Test_Delete_All()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                unitOfWork.Observations.Clear();
                unitOfWork.Complete();
            }
        }
    }
}