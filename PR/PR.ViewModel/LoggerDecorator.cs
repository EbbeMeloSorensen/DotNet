//using System;
//using System.Diagnostics;
//using Craft.Logging;
//using Craft.ViewModel.Utils;

//namespace PR.ViewModel
//{
//    // Deprecated - use the ViewModelLogger in Craft.ViewModel.Utils
//    public class LoggerDecoratorDeprecated : ILogger
//    {
//        private ILogger _logger;
//        private LogViewModel _logViewModel;
//        private Stopwatch _stopWatch;

//        public LoggerDecoratorDeprecated(
//            ILogger logger,
//            LogViewModel logViewModel)
//        {
//            _logger = logger;
//            _logViewModel = logViewModel;

//            _stopWatch = new Stopwatch();
//        }

//        public void WriteLine(string message)
//        {
//            //_logger.WriteLine(message);

//            _logViewModel.LogUpdated = true;
//        }

//        public void WriteLineAndStartStopWatch(string message)
//        {
//            WriteLine(message);

//            _stopWatch.Reset();
//            _stopWatch.Start();
//        }

//        public void StopStopWatchAndWriteLine(string message)
//        {
//            _stopWatch.Stop();
//            var processingTime = _stopWatch.Elapsed;
//            var hh = $"{processingTime.Hours}".PadLeft(2, '0');
//            var mm = $"{processingTime.Minutes}".PadLeft(2, '0');
//            var ss = $"{processingTime.Seconds}".PadLeft(2, '0');
//            var ms = $"{processingTime.Milliseconds}".PadLeft(3, '0');

//            WriteLine($"{message} (time elapsed: {hh}:{mm}:{ss}.{ms})");
//        }
        
//        public void WriteLine(
//            LogMessageCategory category, 
//            string message, 
//            string aspect,
//            bool startStopwatch)
//        {
//            //throw new NotImplementedException();
//        }
//    }
//}
