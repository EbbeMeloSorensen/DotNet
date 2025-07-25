﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Craft.Domain;
using Craft.Logging;
using Craft.Utils;
using PR.Domain.Entities.PR;
using PR.IO;
using PR.Persistence;
using PR.Persistence.Versioned;

namespace PR.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private readonly IBusinessRuleCatalog _businessRuleCatalog;
        private IDataIOHandler _dataIOHandler;
        private ILogger _logger;

        public ILogger Logger
        {
            get => _logger;
            set
            {
                _logger = value;
                UnitOfWorkFactory.Logger = value;
            }
        }

        public IUnitOfWorkFactory UnitOfWorkFactory { get; set; }

        public Application(
            IUnitOfWorkFactory unitOfWorkFactory,
            IBusinessRuleCatalog businessRuleCatalog,
            IDataIOHandler dataIOHandler,
            ILogger logger)
        {
            UnitOfWorkFactory = unitOfWorkFactory;
            _businessRuleCatalog = businessRuleCatalog;
            _dataIOHandler = dataIOHandler;
            _logger = logger;
            UnitOfWorkFactory.Logger = logger;
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

        public async Task<Dictionary<string, string>> CreateNewPerson(
            Person person,
            ProgressCallback progressCallback = null)
        {
            return await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Creating Person..");
                progressCallback?.Invoke(0.0, "Creating Person");

                if (person.ID != Guid.Empty ||
                    person.ArchiveID != Guid.Empty)
                {
                    throw new InvalidOperationException("When creating a new person, the ID and ArchiveID should be empty");
                }

                var businessRuleViolations = _businessRuleCatalog.ValidateAtomic(person);

                if (businessRuleViolations.Any())
                {
                    progressCallback?.Invoke(100, "");
                    Logger?.WriteLine(LogMessageCategory.Information, "Aborting due to atomic business rule violations:");

                    foreach (var kvp in businessRuleViolations)
                    {
                        Logger?.WriteLine(LogMessageCategory.Information, $"{kvp.Value}");
                    }

                    return businessRuleViolations;
                }

                using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
                await unitOfWork.People.Add(person);
                unitOfWork.Complete();
                Logger?.WriteLine(LogMessageCategory.Information, "Completed creating Person");

                progressCallback?.Invoke(100, "");
                return businessRuleViolations;
            });
        }

        public Dictionary<string, string> CreatePersonVariant_ValidateInput(
             Person newPersonVariant,
             IEnumerable<Person> existingVariants,
             out List<Person> nonConflictingPersonVariants,
             out List<Person> coveredPersonVariants,
             out List<Person> trimmedPersonVariants,
             out List<Person> newPersonVariants)
        {
            if (newPersonVariant.ID == Guid.Empty ||
                newPersonVariant.ArchiveID != Guid.Empty)
            {
                throw new InvalidOperationException("When creating a new person variant, the ID should be set and the ArchiveID should be empty");
            }

            var businessRuleViolations = _businessRuleCatalog.ValidateAtomic(newPersonVariant);

            if (businessRuleViolations.Any())
            {
                nonConflictingPersonVariants =
                coveredPersonVariants =
                trimmedPersonVariants =
                newPersonVariants = new List<Person>();

                return businessRuleViolations;
            }

            existingVariants.InsertNewVariant(
                newPersonVariant,
                out nonConflictingPersonVariants,
                out coveredPersonVariants,
                out trimmedPersonVariants,
                out newPersonVariants);

            var newPotentialEntityCollection = nonConflictingPersonVariants;
            newPotentialEntityCollection.AddRange(trimmedPersonVariants);
            newPotentialEntityCollection.AddRange(newPersonVariants);
            newPotentialEntityCollection.Add(newPersonVariant);

            newPotentialEntityCollection = newPotentialEntityCollection
                .OrderBy(_ => _.Start)
                .ToList();

            return _businessRuleCatalog.ValidateCrossEntity(newPotentialEntityCollection);
        }

        public async Task<Dictionary<string, string>> CreatePersonVariant(
            Person person,
            ProgressCallback progressCallback = null)
        {
            return await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Creating Person variant..");
                progressCallback?.Invoke(0.0, "Creating Person variant");

                using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
                var otherVariants = (await unitOfWork.People.GetAllVariants(person.ID)).ToList();

                var businessRuleViolations = CreatePersonVariant_ValidateInput(
                    person,
                    otherVariants,
                    out var nonConflictingPersonVariants,
                    out var coveredEntities,
                    out var trimmedEntities,
                    out var newEntities);

                if (businessRuleViolations.Any())
                {
                    progressCallback?.Invoke(100, "");
                    Logger?.WriteLine(LogMessageCategory.Information, "Aborting due to business rule violations:");

                    foreach (var kvp in businessRuleViolations)
                    {
                        Logger?.WriteLine(LogMessageCategory.Information, $"{kvp.Value}");
                    }

                    return businessRuleViolations;
                }
                else
                {
                    if (coveredEntities.Any())
                    {
                        await unitOfWork.People.EraseRange(coveredEntities);
                    }

                    if (trimmedEntities.Any())
                    {
                        await unitOfWork.People.CorrectRange(trimmedEntities);
                    }

                    if (newEntities.Any())
                    {
                        await unitOfWork.People.AddRange(newEntities);
                    }

                    await unitOfWork.People.Add(person);

                    unitOfWork.Complete();
                    Logger?.WriteLine(LogMessageCategory.Information, "Completed creating Person variant");
                }

                progressCallback?.Invoke(100, "");
                return businessRuleViolations;
            });
        }

        public Dictionary<string, string> CorrectPersonVariant_ValidateInput(
             Person personVariant,
             IEnumerable<Person> existingVariants,
             out List<Person> nonConflictingPersonVariants,
             out List<Person> coveredPersonVariants,
             out List<Person> trimmedPersonVariants,
             out List<Person> newPersonVariants)
        {
            if (personVariant.ID == Guid.Empty ||
                personVariant.ArchiveID == Guid.Empty)
            {
                throw new InvalidOperationException("When correcting a person variant, the ID and the ArchiveID should be set");
            }

            var businessRuleViolations = _businessRuleCatalog.ValidateAtomic(personVariant);

            if (businessRuleViolations.Any())
            {
                nonConflictingPersonVariants =
                coveredPersonVariants =
                trimmedPersonVariants =
                newPersonVariants = new List<Person>();

                return businessRuleViolations;
            }

            existingVariants
                .Where(_ => _.ArchiveID != personVariant.ArchiveID)
                .InsertNewVariant(
                    personVariant,
                    out nonConflictingPersonVariants,
                    out coveredPersonVariants,
                    out trimmedPersonVariants,
                    out newPersonVariants);

            var newPotentialEntityCollection = nonConflictingPersonVariants;
            newPotentialEntityCollection.AddRange(trimmedPersonVariants);
            newPotentialEntityCollection.AddRange(newPersonVariants);
            newPotentialEntityCollection.Add(personVariant);

            newPotentialEntityCollection = newPotentialEntityCollection
                .OrderBy(_ => _.Start)
                .ToList();

            return _businessRuleCatalog.ValidateCrossEntity(newPotentialEntityCollection);
        }

        public async Task<Dictionary<string, string>> CorrectPersonVariant(
            Person person,
            ProgressCallback progressCallback = null)
        {
            return await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Correcting Person variant..");
                progressCallback?.Invoke(0.0, "Correcting Person variant");

                using var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
                var otherVariants = (await unitOfWork.People.GetAllVariants(person.ID)).ToList();

                var businessRuleViolations = CorrectPersonVariant_ValidateInput(
                    person,
                    otherVariants,
                    out var nonConflictingPersonVariants,
                    out var coveredEntities,
                    out var trimmedEntities,
                    out var newEntities);

                if (businessRuleViolations.Any())
                {
                    progressCallback?.Invoke(100, "");
                    Logger?.WriteLine(LogMessageCategory.Information, "Aborting due to business rule violations:");

                    foreach (var kvp in businessRuleViolations)
                    {
                        Logger?.WriteLine(LogMessageCategory.Information, $"{kvp.Value}");
                    }

                    return businessRuleViolations;
                }
                else
                {
                    var variantOfInterest = otherVariants.Single(_ => _.ArchiveID == person.ArchiveID);
                    await unitOfWork.People.Erase(variantOfInterest);

                    if (coveredEntities.Any())
                    {
                        await unitOfWork.People.EraseRange(coveredEntities);
                    }

                    if (trimmedEntities.Any())
                    {
                        await unitOfWork.People.CorrectRange(trimmedEntities);
                    }

                    if (newEntities.Any())
                    {
                        await unitOfWork.People.AddRange(newEntities);
                    }

                    person.ArchiveID = Guid.Empty;
                    await unitOfWork.People.Add(person);

                    unitOfWork.Complete();
                    Logger?.WriteLine(LogMessageCategory.Information, "Completed correcting Person variant");
                }

                progressCallback?.Invoke(100, "");
                return businessRuleViolations;
            });
        }

        public Dictionary<string, string> ErasePersonVariants_ValidateInput(
             IEnumerable<Person> variantsToDelete,
             IEnumerable<Person> existingVariants)
        {
            var newPotentialEntityCollection = existingVariants.Except(variantsToDelete);

            return _businessRuleCatalog.ValidateCrossEntity(newPotentialEntityCollection);
        }

        public async Task GetPersonDetails(
            Guid id,
            DateTime? databaseTime,
            bool writeToFile,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Getting Person details..");
                progressCallback?.Invoke(0.0, "Getting Person details");

                if (databaseTime.HasValue && UnitOfWorkFactory is IUnitOfWorkFactoryVersioned unitOfWorkFactoryVersioned)
                {
                    unitOfWorkFactoryVersioned.DatabaseTime = databaseTime.Value;
                }

                List<Person> personVariants = null;

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    //person = await unitOfWork.People.Get(id);
                    personVariants = (await unitOfWork.People.GetAllVariants(id)).ToList();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed getting Person details");

                var lines = new List<string>();

                if (!writeToFile)
                {
                    lines.Add("");
                }

                if (databaseTime.HasValue)
                {
                    lines.Add($"   Database Time: {databaseTime}");
                }

                lines.Add($"History of person with ID={id}:");
                lines.Add("");

                lines.AddRange(personVariants.Select(_ => 
                {
                    var sb = new StringBuilder($"{_.Start.AsDateString()}->");
                    sb.Append($"{_.End.AsDateString()}: ");
                    sb.Append(_.FirstName);
                    return sb.ToString();
                }));

                if (writeToFile)
                {
                    File.WriteAllLines("output.txt", lines);
                }
                else
                {
                    Console.WriteLine();

                    foreach (var line in lines)
                    {
                        Console.WriteLine(line);
                    }
                }

                /*
                var surname = string.IsNullOrEmpty(person.Surname) ? "-" : person.Surname;
                var nickname = string.IsNullOrEmpty(person.Nickname) ? "-" :person.Nickname;
                var address = string.IsNullOrEmpty(person.Address) ? "-" : person.Address;
                var zipCode = string.IsNullOrEmpty(person.ZipCode) ? "-" : person.ZipCode;
                var city = string.IsNullOrEmpty(person.City) ? "-" : person.City;
                var birthday = person.Birthday.HasValue ? person.Birthday.Value.AsDateTimeString(false) : "-";
                var category = string.IsNullOrEmpty(person.Category) ? "-" : person.Category;
                var description = string.IsNullOrEmpty(person.Description) ? "-" : person.Description;
                var latitude = person.Latitude.HasValue ? $"{person.Latitude.Value}" : "-";
                var longitude = person.Longitude.HasValue ? $"{person.Longitude.Value}" : "-";

                Console.WriteLine();
                Console.WriteLine("Person Details:");
                Console.WriteLine($"  ID:          {person.ID}");
                Console.WriteLine($"  First Name:  {person.FirstName}");
                Console.WriteLine($"  Surname:     {surname}");
                Console.WriteLine($"  Nickname:    {nickname}");
                Console.WriteLine($"  Address:     {address}");
                Console.WriteLine($"  ZipCode:     {zipCode}");
                Console.WriteLine($"  City:        {city}");
                Console.WriteLine($"  Birthday:    {birthday}");
                Console.WriteLine($"  Category:    {category}");
                Console.WriteLine($"  Description: {description}");
                Console.WriteLine($"  Latitude:    {latitude}");
                Console.WriteLine($"  Longitude:   {longitude}");
                */
            });
        }

        public async Task UpdatePerson(
            Person person,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Updating Person..");
                progressCallback?.Invoke(0.0, "Updating Person");

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    await unitOfWork.People.Update(person);
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed updating person");
            });
        }

        public async Task DeletePerson(
            Guid id,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Deleting Person..");
                progressCallback?.Invoke(0.0, "Deleting Person");

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    var person = await unitOfWork.People.Get(id);
                    await unitOfWork.People.Remove(person);
                    unitOfWork.Complete();
                }

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed deleting Person");
            });
        }

        public async Task ListPeople(
            DateTime? timeOfInterest,
            DateTime? databaseTime,
            bool excludeCurrentPeople,
            bool includeHistoricalPeople,
            bool writeToFile,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(async () =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Retrieving objects..");
                progressCallback?.Invoke(0.0, "Retrieving objects");

                var lines = new List<string>();

                if (!writeToFile)
                {
                    lines.Add("");
                }

                if (timeOfInterest.HasValue)
                {
                    lines.Add($" Historical Time: {timeOfInterest}");
                }

                if (databaseTime.HasValue)
                {
                    lines.Add($"   Database Time: {databaseTime}");
                }

                if (databaseTime.HasValue && UnitOfWorkFactory is IUnitOfWorkFactoryVersioned unitOfWorkFactoryVersioned)
                {
                    unitOfWorkFactoryVersioned.DatabaseTime = databaseTime.Value;
                }

                if (UnitOfWorkFactory is IUnitOfWorkFactoryHistorical unitOfWorkFactoryHistorical)
                {
                    unitOfWorkFactoryHistorical.IncludeCurrentObjects = !excludeCurrentPeople;
                    unitOfWorkFactoryHistorical.IncludeHistoricalObjects = includeHistoricalPeople;
                    timeOfInterest ??= DateTime.UtcNow;
                    unitOfWorkFactoryHistorical.HistoricalTime = timeOfInterest;
                }

                List<Person> people;

                using (var unitOfWork = UnitOfWorkFactory.GenerateUnitOfWork())
                {
                    people = (await unitOfWork.People.GetAll()).ToList();
                }

                progressCallback?.Invoke(100, "");

                lines.Add($"{people.Count} objects retrieved");

                lines.AddRange(people.Select(p =>
                {
                    var sb = new StringBuilder($"{p.ID}: {p.FirstName}");

                    if (!string.IsNullOrEmpty(p.Surname))
                    {
                        sb.Append($" {p.Surname}");
                    }

                    if (p.Latitude.HasValue && p.Longitude.HasValue)
                    {
                        //sb.Append($" ({p.Latitude}, {p.Longitude})");
                    }

                    if (timeOfInterest.HasValue && p.End < timeOfInterest)
                    {
                        sb.Append(" (historical)");
                    }

                    return sb.ToString();
                }).ToList());

                if (writeToFile)
                {
                    File.WriteAllLines("output.txt", lines);
                }
                else
                {
                    Console.WriteLine();

                    foreach (var line in lines)
                    {
                        Console.WriteLine(line);
                    }
                }
            });
        }

        public async Task ExportData(
            string fileName,
            ProgressCallback progressCallback = null)
        {
            //await Task.Run(() =>
            //{
            //    Logger?.WriteLine(LogMessageCategory.Information, "Exporting data..");
            //    progressCallback?.Invoke(0.0, "Exporting data");

            //    _logger?.WriteLine(LogMessageCategory.Information, $"Exporting data..");

            //    var extension = Path.GetExtension(fileName)?.ToLower();

            //    if (extension == null)
            //    {
            //        throw new ArgumentException();
            //    }

            //    _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieving all person records from repository..", "general", true);

            //    using (var unitOfWork = _unitOfWorkFactoryFacade.GenerateUnitOfWork())
            //    {
            //        var people = unitOfWork.People
            //            .GetAll()
            //            .OrderBy(p => p.Created)
            //            .ToList();

            //        var personAssociations = unitOfWork.PersonAssociations
            //            .GetAll()
            //            .OrderBy(pa => pa.Created)
            //            .ToList();

            //        _logger?.WriteLine(LogMessageCategory.Information, $"  Retrieved {people.Count} person records");

            //        var prData = new PRData
            //        {
            //            People = people,
            //            PersonAssociations = personAssociations.OrderBy(pa => pa.Created).ToList()
            //        };

            //        _logger?.WriteLine(LogMessageCategory.Information, $"  Done..");

            //        switch (extension)
            //        {
            //            case ".xml":
            //                {
            //                    _logger?.WriteLine(LogMessageCategory.Information, $"  Exporting as xml..", "general", true);
            //                    _dataIOHandler.ExportDataToXML(prData, fileName);
            //                    _logger?.WriteLine(LogMessageCategory.Information,
            //                        $"  Exported {people.Count} person records and {personAssociations.Count} person association records to xml file");
            //                    break;
            //                }
            //            case ".json":
            //                {
            //                    _logger?.WriteLine(LogMessageCategory.Information, $"  Exporting as json..", "general", true);
            //                    _dataIOHandler.ExportDataToJson(prData, fileName);
            //                    _logger?.WriteLine(LogMessageCategory.Information,
            //                        $"  Exported {people.Count} person records and {personAssociations.Count} person association records to json file");
            //                    break;
            //                }
            //            default:
            //                {
            //                    throw new ArgumentException();
            //                }
            //        }
            //    }

            //    progressCallback?.Invoke(100, "");
            //    Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            //});
        }

        public async Task ImportData(
            string fileName,
            ProgressCallback progressCallback = null)
        {
            //await Task.Run(() =>
            //{
            //    Logger?.WriteLine(LogMessageCategory.Information, "Importing data..");
            //    progressCallback?.Invoke(0.0, "Importing data");

            //    var extension = Path.GetExtension(fileName)?.ToLower();

            //    if (extension == null)
            //    {
            //        throw new ArgumentException();
            //    }

            //    var prData = new PRData();

            //    switch (extension)
            //    {
            //        case ".xml":
            //            {
            //                _dataIOHandler.ImportDataFromXML(
            //                    fileName, out prData);
            //                break;
            //            }
            //        case ".json":
            //            {
            //                _dataIOHandler.ImportDataFromJson(
            //                    fileName, out prData);
            //                break;
            //            }
            //        default:
            //            {
            //                throw new ArgumentException();
            //            }
            //    }

            //    using (var unitOfWork = _unitOfWorkFactory.GenerateUnitOfWork())
            //    {
            //        unitOfWork.People.AddRange(prData.People);
            //        unitOfWork.PersonAssociations.AddRange(prData.PersonAssociations);
            //        unitOfWork.Complete();
            //    }

            //    progressCallback?.Invoke(100, "");
            //    Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            //});
        }
    }
}
