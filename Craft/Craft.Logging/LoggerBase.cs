using System;
using System.Diagnostics;

namespace Craft.Logging
{
    public abstract class LoggerBase : ILogger
    {
        protected Stopwatch _stopwatch;

        public LoggerBase()
        {
            _stopwatch = new Stopwatch();
        }

        public abstract void WriteLine(
            LogMessageCategory category,
            string message,
            string aspect,
            bool startStopwatch);
    }
}
