using System;
using System.IO;

namespace CryBits.Server.Library;

internal static class Directories
{
    // Data file format.
    public const string Format = ".dat";

    // Base directory for server data.
    private static readonly string BaseDir = AppContext.BaseDirectory;

    // Data paths.
    public static readonly FileInfo Settings = new(Path.Combine(BaseDir, "Data", "settings.json"));
    public static readonly DirectoryInfo Accounts = new(Path.Combine(BaseDir, "Data", "Accounts"));
    public static readonly FileInfo Characters = new(Path.Combine(BaseDir, "Data", "Characters") + Format);
    public static readonly DirectoryInfo Classes = new(Path.Combine(BaseDir, "Data", "Classes"));
    public static readonly DirectoryInfo Maps = new(Path.Combine(BaseDir, "Data", "Maps"));
    public static readonly DirectoryInfo Npcs = new(Path.Combine(BaseDir, "Data", "Npcs"));
    public static readonly DirectoryInfo Items = new(Path.Combine(BaseDir, "Data", "Items"));
    public static readonly DirectoryInfo Shops = new(Path.Combine(BaseDir, "Data", "Shops"));

    public static void Create()
    {
        // Create all required data directories.
        Settings.Directory?.Create();
        Accounts.Create();
        Characters.Directory?.Create();
        Classes.Create();
        Maps.Create();
        Npcs.Create();
        Items.Create();
        Shops.Create();
    }
}
