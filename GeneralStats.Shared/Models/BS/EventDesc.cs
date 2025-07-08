using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class EventDesc
{
    [JsonPropertyName("mode")]
    public string Mode { get; set; }
    [JsonPropertyName("map")]
    public string Map { get; set; }
}
