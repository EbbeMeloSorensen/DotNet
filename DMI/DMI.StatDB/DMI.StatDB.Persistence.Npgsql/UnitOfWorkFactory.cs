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
            // MELO-HOME
            //var host = "localhost";
            //var port = 5432;
            //var schema = "public";
            //var database = "statdb";
            //var user = "postgres";
            //var password = "L1on8Zebra";

            // DMI
            var host = "nanoq.dmi.dk";
            var port = 5432;
            var schema = "public";
            var database = "statdb";
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
