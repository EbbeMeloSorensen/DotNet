using Xunit;

namespace PR.Persistence.APIClient.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var people = unitOfWork.People.GetAll();
            }
        }
    }
}