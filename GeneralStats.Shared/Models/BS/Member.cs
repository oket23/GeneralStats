using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class Member
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("role")]
    public string Role { get; set; }
    [JsonPropertyName("trophies")]
    public int Trophies { get; set; }
}
