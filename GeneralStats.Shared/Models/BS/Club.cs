using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class Club
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
