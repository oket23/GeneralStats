using Serilog;
using Serilog.Core;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;

namespace BrawlStarsService.Services;

public class ListenerService
{
    private readonly ILogger _logger;
    private readonly BSService _service;
    private readonly HttpListener _listener;

    public ListenerService(Logger logger, BSService service)
    {
        _logger = logger;
        _service = service;

        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:8000/");
        _listener.Start();

        _logger.Information("BS Server started!");
        Console.WriteLine("BS Server started!");
    }

    public async Task ListenerHandlerAsync()
    {
        while (true)
        {
            var context = await _listener.GetContextAsync();

            var method = context.Request.HttpMethod;
            var localPath = context.Request.Url.LocalPath;

            var response = context.Response;
            var request = context.Request;

            _logger.Information($"Gets Endpoints: {localPath}");
            Console.WriteLine($"Gets Endpoints: {localPath} on {DateTime.UtcNow}");

            await MethodHandlerAsync(method, localPath, response, request);

            response.Close();
        }
    }

    private async Task MethodHandlerAsync(string method, string localPath, HttpListenerResponse response, HttpListenerRequest request)
    {
        switch (method)
        {
            default:
                _logger.Error($"Cloud not found endpoint {localPath}");
                SendResponse(response, "Incorrect http method!", 501);
                break;

            case "GET":
                if (localPath.StartsWith("/player/"))
                {
                    try
                    {
                        var tag = localPath.TrimStart("/player/".ToCharArray());

                        var playerStats = await _service.GetPlayerStatsAsync(tag);

                        if (playerStats != null)
                        {
                            var json = JsonSerializer.Serialize(playerStats);
                            SendResponse(response, json, 200);

                            _logger.Information("Player stats successfully returning");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"{ex.Message}");
                        SendResponse(response, ex.Message, 400);
                    }
                }
                if (localPath.StartsWith("/club/"))
                {
                    try
                    {
                        var tag = localPath.TrimStart("/club/".ToCharArray());

                        var clubStats = await _service.GetClubStatsAsync(tag);

                        if (clubStats != null)
                        {
                            var json = JsonSerializer.Serialize(clubStats);
                            SendResponse(response, json, 200);

                            _logger.Information("Club stats successfully returning");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"{ex.Message}");
                        SendResponse(response, ex.Message, 400);
                    }
                }
                if (localPath.Equals("/events"))
                {
                    try
                    {
                        var events = await _service.GetEventsAsync();

                        foreach(var item in events)
                        {
                            item.StartTime = ValidTime(item.StartTime);
                            item.EndTime = ValidTime(item.EndTime);
                        }

                        if (events != null)
                        {
                            var json = JsonSerializer.Serialize(events);
                            SendResponse(response, json, 200);

                            _logger.Information("Events successfully returning");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"{ex.Message}");
                        SendResponse(response, ex.Message, 400);
                    }
                }
                else
                {
                    _logger.Error($"Cloud not found endpoint {localPath}");
                    SendResponse(response, $"Cloud not found endpoint {localPath}", 404);
                }
                break;
        }
    }

    private static void SendResponse(HttpListenerResponse response, string json, int statusCode)
    {
        response.StatusCode = statusCode;
        response.ContentType = "application/json";

        using(var stream = response.OutputStream)
        {
            stream.Write(Encoding.UTF8.GetBytes(json));
            stream.Flush();
        }
    }

    private static string ValidTime(string time)
    {
        return DateTime.ParseExact(
            time,
            "yyyyMMdd'T'HHmmss.fff'Z'",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
        ).ToLocalTime().ToString();
    }
}
