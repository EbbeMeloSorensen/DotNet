using Craft.Logging;

namespace Craft.ViewModel.Utils
{
    // This is a decorator class for writing messages in a wpf control as well as to file
    public class ViewModelLogger : LoggerBase
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

        public override void WriteLine(
            LogMessageCategory category,
            string message,
            string aspect,
            bool startStopWatch)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                message = $"{message} ({_stopwatch.Elapsed})";
                _stopwatch.Reset();
            }
            else if (startStopWatch)
            {
                _stopwatch.Start();
            }

            _logger.WriteLine(category, message, aspect, startStopWatch);

            _logViewModel.Log += $"{message}\n";

            _logViewModel.LogUpdated = true;
        }
    }
}
