using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class Gadget
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
