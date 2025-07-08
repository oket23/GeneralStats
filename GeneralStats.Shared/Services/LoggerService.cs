using Serilog;
using Serilog.Core;

namespace LoggerSevice;

public static class LoggerService
{
    public static Logger GetLogger(string path,string serviceName)
    {
        var logDirectory = Path.GetDirectoryName(path);
        Directory.CreateDirectory(logDirectory);

        return new LoggerConfiguration().Enrich.WithProperty("ServiceName", serviceName)
               .WriteTo.File(path, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] [{ServiceName}]: {Message:lj}{NewLine}{Exception}")
               .MinimumLevel.Debug()
               .CreateLogger();
    }

}