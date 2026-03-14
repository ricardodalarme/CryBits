namespace CryBits.Client.Framework;

public sealed class Options
{
    public static Options Instance { get; set; } = new();

    public bool SaveUsername { get; set; } = true;
    public string Username { get; set; } = string.Empty;
    public bool Sounds { get; set; } = true;
    public bool Musics { get; set; } = true;
    public bool Chat { get; set; } = true;
    public bool ShowMetrics { get; set; } = false;
    public bool Party { get; set; } = true;
    public bool Trade { get; set; } = true;
    public bool PreMapGrid { get; set; } = false;
    public bool PreMapView { get; set; } = false;
    public bool PreMapAudio { get; set; } = false;
}
