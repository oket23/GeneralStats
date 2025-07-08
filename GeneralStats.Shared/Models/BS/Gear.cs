using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class Gear
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
