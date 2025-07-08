using BrawlStarsService.Services;
using GeneralStats.Services;
using LoggerSevice;
using Microsoft.Extensions.Configuration;

namespace GeneralStats;

public class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appconfig.json")
            .Build();

        var logPath = configuration["Logging:LogPath"];
        var logger = LoggerService.GetLogger(logPath,"UI");
        var bsService = new BSService(logger);

        string token = configuration["Tg:Token"];
        var tgService = new TgService(logger, token, bsService);

        tgService.StartTelegramBotAsync();

        Console.ReadKey();
    }
}
