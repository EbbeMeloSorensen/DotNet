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

        public async Task MakeBreakfast(
            DateTime? cutDate,
            ProgressCallback progressCallback = null)
        {
            await Task.Run(() =>
            {
                var result = 0.0;
                var currentActivity = "Baking bread";
                var count = 0;
                var total = 317;

                while (count < total)
                {
                    if (count >= 160)
                    {
                        currentActivity = "Poring Milk";
                    }
                    else if (count >= 80)
                    {
                        currentActivity = "Frying eggs";
                    }

                    for (var j = 0; j < 999999999 / 100; j++)
                    {
                        result += 1.0;
                    }

                    count++;

                    if (progressCallback?.Invoke(100.0 * count / total, currentActivity) is true)
                    {
                        break;
                    }
                }
            });
        }
    }
}
