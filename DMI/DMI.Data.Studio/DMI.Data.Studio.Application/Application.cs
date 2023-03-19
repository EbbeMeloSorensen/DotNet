using System;
using Craft.Logging;

namespace DMI.Data.Studio.Application
{
    public delegate bool ProgressCallback(
        double progress,
        string currentActivity);

    public class Application
    {
        private ILogger _logger;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

    }
}
