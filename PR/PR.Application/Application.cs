﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Craft.Logging;
using PR.Domain.Entities;

namespace PR.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private IUIDataProvider _uiDataProvider;
        private ILogger _logger;

        public IUIDataProvider UIDataProvider => _uiDataProvider;

        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public Application(
            IUIDataProvider uiDataProvider,
            ILogger logger)
        {
            _uiDataProvider = uiDataProvider;
            _logger = logger;
        }

        public void Initialize()
        {
            Logger?.WriteLine(LogMessageCategory.Debug, "DMI.SMS.UI.WPF - initializing application");

            _uiDataProvider.Initialize(_logger);
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

                UIDataProvider.CreatePerson(person);

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

                people = UIDataProvider.GetAllPeople();

                progressCallback?.Invoke(100, "");
            });

            Console.WriteLine();
            people?.ToList().ForEach(p => Console.WriteLine($"  {p.FirstName}"));
        }

        public async Task ExportData(
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                Logger?.WriteLine(LogMessageCategory.Information, "Exporting data..");
                progressCallback?.Invoke(0.0, "Exporting data");

                UIDataProvider.ExportData("People.json", null);

                progressCallback?.Invoke(100, "");
                Logger?.WriteLine(LogMessageCategory.Information, "Completed exporting data");
            });
        }
    }
}