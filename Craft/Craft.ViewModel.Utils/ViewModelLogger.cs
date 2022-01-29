using Craft.Logging;

namespace Craft.ViewModel.Utils
{
    public class ViewModelLogger : ILogger
    {
        private ILogger _logger;
        private LogViewModel _logViewModel;

        public ViewModelLogger(
            ILogger logger, 
            LogViewModel logViewModel)
        {
            _logger = logger;
            _logViewModel = logViewModel;
        }

        public void WriteLine(
            LogMessageCategory category,
            string message,
            string aspect)
        {
            _logger.WriteLine(category, message, aspect);

            _logViewModel.Log += $"{message}\n";

            _logViewModel.LogUpdated = true;
        }
    }
}
