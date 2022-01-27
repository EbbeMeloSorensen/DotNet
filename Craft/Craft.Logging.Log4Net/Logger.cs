using System;
using System.Collections.Generic;
using log4net;

namespace Craft.Logging.Log4Net
{
    public class Logger : ILogger
    {
        private Dictionary<string, ILog> _aspectToLoggerMap;

        public Logger()
        {
            _aspectToLoggerMap = new Dictionary<string, ILog>();
        }

        public void WriteLine(
            LogMessageCategory category,
            string message,
            string aspect)
        {
            _aspectToLoggerMap.TryGetValue(aspect, out ILog log);

            if (log == null)
            {
                log = LogManager.GetLogger(aspect);
                _aspectToLoggerMap[aspect] = log;
            }

            switch (category)
            {
                case LogMessageCategory.Debug:
                    log.Debug(message);
                    break;
                case LogMessageCategory.Information:
                    log.Info(message);
                    break;
                case LogMessageCategory.Warning:
                    log.Warn(message);
                    break;
                case LogMessageCategory.Error:
                    log.Error(message);
                    break;
                case LogMessageCategory.Fatal:
                    log.Fatal(message);
                    break;
                default:
                    throw new ArgumentException("Invalid Log message category");
            }
        }
    }
}
