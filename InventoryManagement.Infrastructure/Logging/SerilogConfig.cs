using Serilog;

namespace InventoryManagement.Infrastructure.Logging;

public static class SerilogConfig
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt",
                rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}