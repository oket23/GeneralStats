using System.Text;
using System.Text.Json.Serialization;

namespace BrawlStarsService.Models;

public class PlayerStats
{
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("trophies")]
    public int Trophies { get; set; }
    [JsonPropertyName("highestTrophies")]
    public int HighestTrophies { get; set; }
    [JsonPropertyName("expLevel")]
    public int ExpLevel { get; set; }
    [JsonPropertyName("3vs3Victories")]
    public int Victories3vs3 { get; set; }
    [JsonPropertyName("soloVictories")]
    public int VictoriesSolo { get; set; }
    [JsonPropertyName("duoVictories")]
    public int VictoriesDuo { get; set; }
    [JsonPropertyName("bestRoboRumbleTime")]
    public int BestRoboRumbleTime { get; set; }
    [JsonPropertyName("bestTimeAsBigBrawler")]
    public int BestTimeAsBigBrawler { get; set; }
    [JsonPropertyName("club")]
    public Club Club { get; set; }
    [JsonPropertyName("brawlers")]
    public List<Brawler> Brawlers { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Tag: {Tag}");
        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Trophies: {Trophies}");
        sb.AppendLine($"Highest Trophies: {HighestTrophies}");
        sb.AppendLine($"Exp Level: {ExpLevel}");
        sb.AppendLine($"3vs3 Victories: {Victories3vs3}");
        sb.AppendLine($"Solo Victories: {VictoriesSolo}");
        sb.AppendLine($"Duo Victories: {VictoriesDuo}");
        sb.AppendLine($"Best Robo Rumble Time: {BestRoboRumbleTime}");
        sb.AppendLine($"Best Time As Big Brawler: {BestTimeAsBigBrawler}");
        sb.AppendLine($"Club: {(Club != null ? $"Club: {Club.Name} (Tag: {Club.Tag})" : "Club: None")}");

        return sb.ToString();
    }

    public string GetAllBrawlers()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Brawlers:");
        foreach(var item in Brawlers)
        {
            sb.AppendLine(item.ToString());
        }

        return sb.ToString();
    }
}
