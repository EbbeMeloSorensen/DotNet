using System.Data.Entity;

namespace Craft.Persistence.EntityFramework
{
    public class MSSQLServerDbConfiguration : DbConfiguration
    {
        public MSSQLServerDbConfiguration()
        {
            SetProviderServices(
                "System.Data.SqlClient", 
                System.Data.Entity.SqlServer.SqlProviderServices.Instance);
        }
    }
}
