using System;
using System.Threading.Tasks;
using Craft.Logging;

namespace DMI.SMS.Application
{
    public delegate bool ProgressCallback(
        double progress);

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

                for (var i = 0; i < 100; i++)
                {
                    if (progressCallback?.Invoke(i) is true)
                    {
                        break;
                    }

                    for (var j = 0; j < 999999999 / 100; j++)
                    {
                        result += 1.0;
                    }
                }

                progressCallback?.Invoke(100);
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
