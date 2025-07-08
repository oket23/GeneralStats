using BrawlStarsService.Models;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Serilog.Core;
using System.Text.Json;

namespace BrawlStarsService.Services;

public class BSService
{
    private readonly ILogger _logger;
    private readonly MemoryCache _cache;
    private readonly HttpClient _client;

    public BSService(Logger logger, string apiKey)
    {
        _logger = logger;
        _client = new HttpClient();
        _client.BaseAddress = new Uri("https://api.brawlstars.com/v1/");
        _client.DefaultRequestHeaders.Add("Authorization", apiKey);
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public async Task<PlayerStats> GetPlayerStatsAsync(string playerTag)
    {
        if (string.IsNullOrWhiteSpace(playerTag))
        {
            _logger.Error("Player tag is empty");
            throw new ArgumentException("Player tag cannot be empty");
        }

        var result = new PlayerStats();
        var key = $"playerStats:{playerTag}";

        if (!_cache.TryGetValue(key, out string json))
        {
            var response = await _client.GetAsync($"players/%23{playerTag}");
            var context = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("Failed to got player stats from API");
                throw new HttpRequestException("Failed to player stats from API");
            }

            result = JsonSerializer.Deserialize<PlayerStats>(context);

            _logger.Information("BSService got player stats from API");
            _cache.Set(key, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));
            return result;
        }
        else
        {
            _logger.Information("BSService got player stats from cache");
            return JsonSerializer.Deserialize<PlayerStats>(json);
        }
    }

    public async Task<ClubStats> GetClubStatsAsync(string clubTag)
    {
        if (string.IsNullOrWhiteSpace(clubTag))
        {
            _logger.Error("Club tag is empty");
            throw new ArgumentException("Club tag cannot be empty");
        }

        var result = new ClubStats();
        var key = $"clubstats:{clubTag}";

        if (!_cache.TryGetValue(key, out string json))
        {
            var response = await _client.GetAsync($"clubs/%23{clubTag}");
            var context = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("Failed to got club stats from API");
                throw new HttpRequestException("Failed to club stats from API");
            }

            result = JsonSerializer.Deserialize<ClubStats>(context);

            _logger.Information("BSService got club stats from API");
            _cache.Set(key, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));
            return result;
        }
        else
        {
            _logger.Information("BSService got club stats from cache");
            return JsonSerializer.Deserialize<ClubStats>(json);
        }
    }

    public async Task<List<Event>> GetEventsAsync()
    {
        var result = new List<Event>();
        var key = "events";

        if (!_cache.TryGetValue(key, out string json))
        {
            var response = await _client.GetAsync("events/rotation");
            var context = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error("Failed to got events from API");
                throw new HttpRequestException("Failed to events from API");
            }

            result = JsonSerializer.Deserialize<List<Event>>(context);

            _logger.Information("BSService got events from API");
            _cache.Set(key, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));
            return result;
        }
        else
        {
            _logger.Information("BSService got events from cache");
            return JsonSerializer.Deserialize<List<Event>>(json);
        }
    }
}
