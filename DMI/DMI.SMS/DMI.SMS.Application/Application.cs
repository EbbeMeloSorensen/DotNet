using System;
using System.Threading.Tasks;
using Craft.Logging;

namespace DMI.SMS.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private IUIDataProvider _uiDataProvider;
        private ILogger _logger;

        public IUIDataProvider UIDataProvider => _uiDataProvider;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
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

            _uiDataProvider.Initialize(logger);
        }

        public async Task ExtractFrieDataMeteorologicalStationList(
            DateTime? cutDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                var result = 0.0;
                var currentActivity = "Baking bread";

                for (var i = 0; i < 100; i++)
                {
                    if (i >= 70)
                    {
                        currentActivity = "Poring Milk";
                    }
                    else if (i >= 30)
                    {
                        currentActivity = "Frying eggs";
                    }

                    if (progressCallback?.Invoke(i, currentActivity) is true)
                    {
                        break;
                    }

                    for (var j = 0; j < 999999999 / 100; j++)
                    {
                        result += 1.0;
                    }
                }

                progressCallback?.Invoke(100, "Complete");
            });
        }

        public void ExtractFrieDataOceanographicalStationList(
            DateTime? cutDate)
        {
            var result = 0.0;

            for (int i = 0; i < int.MaxValue; i++)
            {
                result += 1.0;
            }
        }
    }
}
