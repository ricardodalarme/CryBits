using CryBits.Client.Framework.Constants;
using CryBits.Utils;
using System.Text.Json;

namespace CryBits.Client.Framework.Persistence.Repositories;

public static class OptionsRepository
{
    /// <summary>Load application options from JSON into <see cref="Options.Instance"/>.</summary>
    public static void Read()
    {
        if (!Directories.Options.Exists)
        {
            Write();
            return;
        }

        using var stream = Directories.Options.OpenRead();
        Options.Instance = JsonSerializer.Deserialize<Options>(stream, JsonConfig.Options) ?? new Options();
    }

    /// <summary>Persist <see cref="Options.Instance"/> to JSON.</summary>
    public static void Write()
    {
        Directories.Options.Directory?.Create();
        using var stream = Directories.Options.Open(FileMode.Create, FileAccess.Write);
        JsonSerializer.Serialize(stream, Options.Instance, JsonConfig.Options);
    }
}
