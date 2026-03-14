namespace CryBits.Client.Framework.Persistence.Dtos;

public sealed class OptionsDto
{
    public bool SaveUsername { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool Sounds { get; set; } = true;
    public bool Musics { get; set; } = true;
    public bool Chat { get; set; } = true;
    public bool ShowMetrics { get; set; }
    public bool Party { get; set; } = true;
    public bool Trade { get; set; } = true;
    public bool PreMapGrid { get; set; }
    public bool PreMapView { get; set; }
    public bool PreMapAudio { get; set; }
}
