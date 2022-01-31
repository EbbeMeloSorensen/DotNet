using System;
using Craft.Logging;

namespace DMI.SMS.Application
{
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

        public void ExtractFrieDataMeteorologicalStationList(
            DateTime? cutDate)
        {
            throw new NotImplementedException();
        }

        public void ExtractFrieDataOceanographicalStationList(
            DateTime? cutDate)
        {
            throw new NotImplementedException();
        }
    }
}
