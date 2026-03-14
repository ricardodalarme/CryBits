using System.Text.Json.Serialization;

namespace CryBits.Client.Framework.Persistence.Dtos;

public sealed class OptionsDto
{
    [JsonPropertyName("saveUsername")] public bool SaveUsername { get; set; }
    [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;
    [JsonPropertyName("sounds")] public bool Sounds { get; set; } = true;
    [JsonPropertyName("musics")] public bool Musics { get; set; } = true;
    [JsonPropertyName("chat")] public bool Chat { get; set; } = true;
    [JsonPropertyName("showMetrics")] public bool ShowMetrics { get; set; }
    [JsonPropertyName("party")] public bool Party { get; set; } = true;
    [JsonPropertyName("trade")] public bool Trade { get; set; } = true;
    [JsonPropertyName("preMapGrid")] public bool PreMapGrid { get; set; }
    [JsonPropertyName("preMapView")] public bool PreMapView { get; set; }
    [JsonPropertyName("preMapAudio")] public bool PreMapAudio { get; set; }
}
