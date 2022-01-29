using System;
using System.Threading.Tasks;
using Npgsql;
using Craft.Logging;

namespace DMI.StatDB.Persistence.Npgsql
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public void Initialize(ILogger logger)
        {
        }

        public async Task<bool> CheckRepositoryConnection()
        {
            return await Task.Run(() =>
            {
                try
                {
                    ConnectionStringProvider.InitializeFromSettingsFile();

                    using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
                    {
                        conn.Open();
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        public IUnitOfWork GenerateUnitOfWork()
        {
            return new UnitOfWork();
        }
    }
}
