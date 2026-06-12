using System;
using System.IO;

namespace CryBits.Server.Persistence;

internal static class Directories
{
    // Data file format.
    public const string Format = ".json";

    // Base directory for server data.
    private static readonly string BaseDir = AppContext.BaseDirectory;

    // Data paths.
    public static readonly FileInfo Settings = new(Path.Combine(BaseDir, "Data", "settings.json"));
    public static readonly DirectoryInfo Accounts = new(Path.Combine(BaseDir, "Data", "Accounts"));
    public static readonly FileInfo Characters = new(Path.Combine(BaseDir, "Data", "Characters") + Format);

    public static void Create()
    {
        // Create all required data directories.
        Settings.Directory?.Create();
        Accounts.Create();
        Characters.Directory?.Create();
    }
}
