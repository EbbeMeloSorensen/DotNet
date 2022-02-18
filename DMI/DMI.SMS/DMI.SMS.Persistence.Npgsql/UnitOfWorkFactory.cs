using Npgsql;
using Craft.Logging;

namespace DMI.SMS.Persistence.Npgsql
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        public void Initialize(
            ILogger logger)
        {
            var host = "172.25.7.23";
            var port = 5432;
            var schema = "sde";
            var database = "sms_prod";
            var user = "ebs";
            var password = "Vm6PAkPh";
            ConnectionStringProvider.Initialize(host, port, database, schema, user, password);
        }

        public async Task<bool> CheckRepositoryConnection()
        {
            return await Task.Run(() =>
            {
                try
                {
                    ConnectionStringProvider.InitializeFromSettingsFile();

                    var connectionString = ConnectionStringProvider.GetConnectionString();

                    using (var conn = new NpgsqlConnection(ConnectionStringProvider.GetConnectionString()))
                    {
                        conn.Open();
                        conn.Close();
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