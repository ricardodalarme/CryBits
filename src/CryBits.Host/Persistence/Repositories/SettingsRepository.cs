using CryBits.Persistence.Serialization;
using System.IO;
using System.Text.Json;
using static CryBits.Definitions.Globals;

namespace CryBits.Host.Persistence.Repositories;

internal sealed class SettingsRepository
{
    public static SettingsRepository Instance { get; } = new();

    public void Read()
    {
        if (!Directories.Settings.Exists)
        {
            Write();
            return;
        }

        var json = File.ReadAllText(Directories.Settings.FullName);
        Config = JsonSerializer.Deserialize<ServerConfig>(json) ?? new ServerConfig();
    }

    public void Write()
    {
        File.WriteAllText(Directories.Settings.FullName, JsonSerializer.Serialize(Config, JsonConfig.Options));
    }
}
