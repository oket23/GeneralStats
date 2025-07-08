using BrawlStarsService.Models;
using Serilog;
using Serilog.Core;
using System.Text.Json;

namespace BrawlStarsService.Services;

public class BSService
{
    private readonly ILogger _logger;
    private readonly HttpClient _client;

    public BSService(Logger logger)
    {
        _logger = logger;
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:8000/");
    }

    public async Task<PlayerStats> GetPlayerStatsAsync(string playerTag)
    {
        var result = new PlayerStats();

        var response = await _client.GetAsync($"player/{playerTag}");
        var context = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error("Failed to got player stats from microService");
            throw new HttpRequestException("Failed to got player stats from microService");
        }

        result = JsonSerializer.Deserialize<PlayerStats>(context);

        _logger.Information("BSService got player stats from microService");
        return result;
    }

    public async Task<ClubStats> GetClubStatsAsync(string clubTag)
    {
        var result = new ClubStats();

        var response = await _client.GetAsync($"club/{clubTag}");
        var context = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error("Failed to got club stats from microService");
            throw new HttpRequestException("Failed to got club stats from microService");
        }

        result = JsonSerializer.Deserialize<ClubStats>(context);

        _logger.Information("BSService got club stats from microService");
        return result;
    }

    public async Task<List<Event>> GetEventsAsync()
    {
        var result = new List<Event>();

        var response = await _client.GetAsync("events");
        var context = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error("Failed to got events from microService");
            throw new HttpRequestException("Failed to got events from microService");
        }

        result = JsonSerializer.Deserialize<List<Event>>(context);

        _logger.Information("BSService got events from microService");
        return result;
    }
}
