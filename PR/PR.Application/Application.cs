using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;
using PR.Persistence;

namespace PR.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private IUIDataProvider _uiDataProvider;
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private ILogger _logger;

        public IUIDataProvider UIDataProvider => _uiDataProvider;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            IUIDataProvider uiDataProvider,
            IUnitOfWorkFactory unitOfWorkFactory,
            ILogger logger)
        {
            _uiDataProvider = uiDataProvider;
            _unitOfWorkFactory = unitOfWorkFactory;
            _logger = logger;
        }

        public void Initialize(
            string host,
            string port,
            string database,
            string schema,
            string user,
            string password)
        {
            Logger?.WriteLine(LogMessageCategory.Debug, "DMI.SMS.UI.WPF - initializing application");

            _uiDataProvider.UnitOfWorkFactory.Host = host;
            _uiDataProvider.UnitOfWorkFactory.Port = port;
            _uiDataProvider.UnitOfWorkFactory.Database = database;
            _uiDataProvider.UnitOfWorkFactory.Schema = schema;
            _uiDataProvider.UnitOfWorkFactory.User = user;
            _uiDataProvider.UnitOfWorkFactory.Password = password;

            _uiDataProvider.UnitOfWorkFactory.Initialize(_logger);
        }

        public async Task MakeBreakfast(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Making breakfast..");

                var result = 0.0;
                var currentActivity = "Baking bread";
                var count = 0;
                var total = 317;

                Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");

                while (count < total)
                {
                    if (count >= 160)
                    {
                        currentActivity = "Poring Milk";

                        if (count == 160)
                        {
                            Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }
                    else if (count >= 80)
                    {
                        currentActivity = "Frying eggs";

                        if (count == 80)
                        {
                            Logger?.WriteLine(LogMessageCategory.Information, $"  {currentActivity}");
                        }
                    }

                    for (var j = 0; j < 499999999 / 100; j++)
                    {
                        result += 1.0;
                    }

                    count++;

                    if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                    {
                        break;
                    }
                }

                Logger?.WriteLine(LogMessageCategory.Information, "Completed breakfast");
            });
        }

        public async Task CreatePerson(
            Person person,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Creating Person..");
                progressCallback?.Invoke(0.0, "Creating Person");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    unitOfWork.People.Add(person);
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed creating Person");
            });
        }

        public async Task ListPeople(
            ProgressCallback progressCallback = null)
        {
            IList<Person>? people = null;

            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Retrieving people..");
                progressCallback?.Invoke(0.0, "Retrieving people");

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    people = unitOfWork.People.GetAll().ToList();
                }


                progressCallback?.Invoke(100, "");
            });

            Console.WriteLine();
            people?.ToList().ForEach(p => Console.WriteLine($"  {p.FirstName}"));
        }

        public async Task ExportData(
            string fileName,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Exporting data..");
                progressCallback?.Invoke(0.0, "Exporting data");

                UIDataProvider.ExportData(fileName, null);

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            });
        }

        public async Task ImportData(
            string fileName,
            bool legacy,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Importing data..");
                progressCallback?.Invoke(0.0, "Importing data");

                UIDataProvider.ImportData(fileName, legacy);

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            });
        }

    }
}
