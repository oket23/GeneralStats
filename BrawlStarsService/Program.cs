using BrawlStarsService.Services;
using LoggerSevice;
using Microsoft.Extensions.Configuration;

namespace BrawlStarsService;

public  class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
             .AddJsonFile("appconfig.json", optional: false)
             .Build();

        var logPath = configuration["Logging:LogPath"];
        var logger = LoggerService.GetLogger(logPath, "BS Service");

        var apiKey = configuration["BS:Authorization"];
        var bSService = new BSService(logger, apiKey);
        var listener = new ListenerService(logger, bSService);

        await listener.ListenerHandlerAsync();
    }
}
