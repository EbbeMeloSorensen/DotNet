using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

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
                var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

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
