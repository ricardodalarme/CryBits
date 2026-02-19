using System.IO;
using System.Text.Json;
using static CryBits.Globals;

namespace CryBits.Server.Library.Repositories;

internal static class SettingsRepository
{
    public static void Read()
    {
        if (!Directories.Settings.Exists)
        {
            Write();
            return;
        }

        var json = File.ReadAllText(Directories.Settings.FullName);
        Config = JsonSerializer.Deserialize<ServerConfig>(json) ?? new ServerConfig();
    }

    public static void Write()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(Directories.Settings.FullName, JsonSerializer.Serialize(Config, options));
    }
}
