using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;
using PR.IO;
using PR.Persistence;

namespace PR.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private IUnitOfWorkFactory _unitOfWorkFactory;
        private IDataIOHandler _dataIOHandler;
        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDataIOHandler dataIOHandler,
            ILogger logger)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _dataIOHandler = dataIOHandler;
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

            _unitOfWorkFactory.Host = host;
            _unitOfWorkFactory.Port = port;
            _unitOfWorkFactory.Database = database;
            _unitOfWorkFactory.Schema = schema;
            _unitOfWorkFactory.User = user;
            _unitOfWorkFactory.Password = password;

            _unitOfWorkFactory.Initialize(_logger);
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

                _logger?.WriteLine(LogMessageCategory.Information, $"Exporting data..");

                var extension = Path.GetExtension(fileName)?.ToLower();

                if (extension == null)
                {
                    throw new ArgumentException();
                }

                _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all person records from repository..", "general", true);

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    var people = unitOfWork.People
                        .GetAll()
                        .OrderBy(p => p.Created)
                        .ToList();

                    var personAssociations = unitOfWork.PersonAssociations
                        .GetAll()
                        .OrderBy(pa => pa.Created)
                        .ToList();

                    _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {people.Count} person records");

                    var prData = new PRData
                    {
                        People = people,
                        PersonAssociations = personAssociations.OrderBy(pa => pa.Created).ToList()
                    };

                    _logger?.WriteLine(LogMessageCategory.Information, $"  Done..");

                    switch (extension)
                    {
                        case ".xml":
                            {
                                _logger?.WriteLine(LogMessageCategory.Information, $"  Exporting as xml..", "general", true);
                                _dataIOHandler.ExportDataToXML(prData, fileName);
                                _logger?.WriteLine(LogMessageCategory.Information,
                                    $"  Exported {people.Count} person records and {personAssociations.Count} person association records to xml file");
                                break;
                            }
                        case ".json":
                            {
                                _logger?.WriteLine(LogMessageCategory.Information, $"  Exporting as json..", "general", true);
                                _dataIOHandler.ExportDataToJson(prData, fileName);
                                _logger?.WriteLine(LogMessageCategory.Information,
                                    $"  Exported {people.Count} person records and {personAssociations.Count} person association records to json file");
                                break;
                            }
                        default:
                            {
                                throw new ArgumentException();
                            }
                    }
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            });
        }

        public async Task ImportData(
            string fileName,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Importing data..");
                progressCallback?.Invoke(0.0, "Importing data");

                var extension = Path.GetExtension(fileName)?.ToLower();

                if (extension == null)
                {
                    throw new ArgumentException();
                }

                var prData = new PRData();

                switch (extension)
                {
                    case ".xml":
                        {
                            _dataIOHandler.ImportDataFromXML(
                                fileName, out prData);
                            break;
                        }
                    case ".json":
                        {
                            _dataIOHandler.ImportDataFromJson(
                                fileName, out prData);
                            break;
                        }
                    default:
                        {
                            throw new ArgumentException();
                        }
                }

                using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
                {
                    unitOfWork.People.AddRange(prData.People);
                    unitOfWork.PersonAssociations.AddRange(prData.PersonAssociations);
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            });
        }
    }
}
