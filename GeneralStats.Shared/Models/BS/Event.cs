using System.ComponentModel;
using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class Event
{
    [JsonPropertyName("startTime")]
    public string StartTime { get; set; }
    [JsonPropertyName("endTime")]
    public string EndTime { get; set; }
    [JsonPropertyName("event")]
    public EventDesc EventDesc { get; set; }
}
