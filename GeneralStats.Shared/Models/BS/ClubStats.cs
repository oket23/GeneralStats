using System.Text;
using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class ClubStats
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("requiredTrophies")]
    public int RequiredTrophies { get; set; }
    [JsonPropertyName("trophies")]
    public int Trophies { get; set; }
    [JsonPropertyName("members")]
    public List<Member> Members { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Tag: {Tag}");
        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Description: {(string.IsNullOrWhiteSpace(Description) ? "None" : Description)}");
        sb.AppendLine($"Type: {Type}");
        sb.AppendLine($"Trophies: {Trophies}");
        sb.AppendLine($"Required Trophies: {RequiredTrophies}");
        sb.AppendLine($"Members: {Members?.Count ?? 0}");
        sb.AppendLine();

        if (Members != null && Members.Any())
        {
            sb.AppendLine("Member list:");
            foreach (var member in Members)
            {
                sb.AppendLine($"- Name: {member.Name}");
                sb.AppendLine($"  Role: {member.Role}");
                sb.AppendLine($"  Trophies: {member.Trophies}");
                sb.AppendLine($"  Tag: {member.Tag}");
                sb.AppendLine();
            }
        }
        else
        {
            sb.AppendLine("No members found.");
        }

        return sb.ToString();
    }
}
