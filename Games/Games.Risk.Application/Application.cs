using Craft.Logging;

namespace Games.Risk.Application
{
    public class Application
    {
        private ILogger _logger;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;

                if (Engine != null)
                {
                    Engine.Logger = value;
                }
            }
        }

        public Engine Engine { get; set; }

        public Application(
            ILogger logger)
        {
            _logger = logger;
        }
    }
}
