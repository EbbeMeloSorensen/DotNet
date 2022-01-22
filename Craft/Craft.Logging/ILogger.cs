namespace Craft.Logging
{
    public enum LogMessageCategory
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }

    public interface ILogger
    {
        void WriteLine(
            LogMessageCategory category,
            string message,
            string aspect = "general");
    }
}
