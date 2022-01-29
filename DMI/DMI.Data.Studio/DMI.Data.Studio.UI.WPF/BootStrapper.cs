using System;
using System.Configuration;
using StructureMap;
using Craft.Persistence;
using DMI.Data.Studio.ViewModel;

namespace DMI.Data.Studio.UI.WPF
{
    public class BootStrapper
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                try
                {
                    //var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    //var settings = configFile.AppSettings.Settings;

                    //// Try getting connection info for a sql server database
                    //var sqlServerDataSource = settings["SqlServerDataSource"]?.Value;
                    //var sqlServerInitialCatalog = settings["SqlServerInitialCatalog"]?.Value;
                    //var sqlServerUserID = settings["SqlServerUserID"]?.Value;
                    //var sqlServerPassword = settings["SqlServerPassword"]?.Value;

                    //// Try getting connection info for a postgresql database
                    //var postgreSqlHost = settings["PostgreSqlHost"]?.Value;
                    //var postgreSqlSchema = settings["PostgreSqlSchema"]?.Value;
                    //var postgreSqlDatabase = settings["PostgreSqlDatabase"]?.Value;
                    //var postgreSqlUser = settings["PostgreSqlUser"]?.Value;
                    //var postgreSqlPassword = settings["PostgreSqlPassword"]?.Value;

                    //// Try getting connection info for a file repository
                    //var repositoryFileName = settings["RepositoryFileName"]?.Value;

                    //ConnectionStringBuilder.InitializeSqlServerConnectionStringParameters(
                    //    sqlServerUserID,
                    //    sqlServerPassword,
                    //    sqlServerInitialCatalog,
                    //    sqlServerDataSource);

                    //ConnectionStringBuilder.InitializePostgreSqlConnectionStringParameters(
                    //    postgreSqlHost,
                    //    postgreSqlSchema,
                    //    postgreSqlDatabase,
                    //    postgreSqlUser,
                    //    postgreSqlPassword);

                    //ConnectionStringBuilder.InitializeRepositoryFileName(
                    //    repositoryFileName);

                    var mainWindowViewModel = Container.For<MainWindowViewModelRegistry>().GetInstance<MainWindowViewModel>();

                    return mainWindowViewModel;
                }
                catch (ConfigurationErrorsException)
                {
                    Console.WriteLine("Error reading app settings");
                    throw;
                }
            }
        }
    }
}
