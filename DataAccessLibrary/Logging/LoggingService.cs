using Serilog;

namespace DataAccessLibrary.Logging;

public static class LoggingService
{
    public static Serilog.ILogger Logger { get; private set; }

    public static void Initialize(string logFilePath)
    {
        Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File($"{logFilePath.Trim()}log-.txt", rollingInterval: RollingInterval.Hour)
            .CreateLogger();
    }
}