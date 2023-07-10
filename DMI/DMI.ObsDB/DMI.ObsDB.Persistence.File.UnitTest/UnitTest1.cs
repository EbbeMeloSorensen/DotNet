using Xunit;

namespace DMI.ObsDB.Persistence.File.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {

            }
        }
    }
}