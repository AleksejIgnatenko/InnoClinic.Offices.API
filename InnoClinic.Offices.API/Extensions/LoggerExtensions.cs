using Serilog;
using Serilog.Core;

namespace InnoClinic.Offices.API.Extensions;

public static class LoggerExtensions
{
    public static Logger CreateSerilog(this LoggerConfiguration loggerConfiguration, IHostBuilder hostBuilder)
    {
        Logger logger = loggerConfiguration
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        hostBuilder.UseSerilog(logger);

        return logger;
    }
}
