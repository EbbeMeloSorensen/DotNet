using Xunit;

namespace PR.Persistence.APIClient.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public async void Test1()
        {
            ApiHelper.InitializeClient();

            var unitOfWorkFactory = new UnitOfWorkFactory();

            using (var unitOfWork = unitOfWorkFactory.GenerateUnitOfWork())
            {
                var people = await unitOfWork.People.GetAll();
            }
        }
    }
}