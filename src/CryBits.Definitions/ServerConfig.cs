using MemoryPack;

namespace CryBits;

/// <summary>Mutable server configuration values. Loaded from <c>settings.json</c> at startup and persisted by the editor.</summary>
[MemoryPackable]
public sealed partial class ServerConfig
{
    public string GameName { get; set; } = "CryBits";
    public string WelcomeMessage { get; set; } = "Welcome to CryBits.";
    public short Port { get; set; } = 7001;
    public byte MaxPlayers { get; set; } = 15;
    public byte MaxCharacters { get; set; } = 3;
    public byte MaxPartyMembers { get; set; } = 3;
    public byte NumPoints { get; set; } = 3;
    public byte MaxNameLength { get; set; } = 12;
    public byte MinNameLength { get; set; } = 3;
    public byte MaxPasswordLength { get; set; } = 12;
    public byte MinPasswordLength { get; set; } = 3;
}
