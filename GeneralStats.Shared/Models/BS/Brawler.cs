using System.Text;
using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class Brawler
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("power")]
    public int Power { get; set; }
    [JsonPropertyName("rank")]
    public int Rank { get; set; }
    [JsonPropertyName("trophies")]
    public int Trophies { get; set; }
    [JsonPropertyName("highestTrophies")]
    public int HighestTrophies { get; set; }
    [JsonPropertyName("gears")]
    public List<Gear>? Gears { get; set; }
    [JsonPropertyName("starPowers")]
    public List<StarPower>? StarPowers { get; set; }
    [JsonPropertyName("gadgets")]
    public List<Gadget>? Gadgets { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Power: {Power}");
        sb.AppendLine($"Rank: {Rank}");
        sb.AppendLine($"Trophies: {Trophies}");
        sb.AppendLine($"Highest Trophies: {HighestTrophies}");

        sb.AppendLine("Gears:");
        if (Gears.Any())
        {
            foreach (var gear in Gears)
            {
                sb.AppendLine($"\t- {gear.Name}");
            }
        }
        else
        {
            sb.AppendLine("\tNone");
        }                     

        sb.AppendLine("Star Powers:");
        if (StarPowers.Any())
        {
            foreach (var sp in StarPowers)
            {
                sb.AppendLine($"\t- {sp.Name}");

            }
        } 
        else
        {
            sb.AppendLine("\tNone");
        }

        sb.AppendLine("Gadgets:");
        if (Gadgets.Any())
        {
            foreach (var g in Gadgets)
            {
                sb.AppendLine($"\t- {g.Name}");

            }
        }    
        else
        {
            sb.AppendLine("\tNone");
        }

        return sb.ToString();
    }
}
