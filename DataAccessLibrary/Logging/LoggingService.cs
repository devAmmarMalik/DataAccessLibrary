using Serilog;

namespace DataAccessLibrary.Logging;

public static class LoggingService
{
    public static Serilog.ILogger Logger { get; private set; }

    public static void Initialize(string logFilePath = "logs/log-.txt")
    {
        Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}
