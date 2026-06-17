namespace CryBits.Persistence;

public static class Directories
{
    // Base directory for server data.
    private static readonly string BaseDir = AppContext.BaseDirectory;

    // Paths.
    public static readonly DirectoryInfo Content = new(Path.Combine(BaseDir, "Content"));
    public static readonly FileInfo Database = new(Path.Combine(BaseDir, "data.db"));

    public static void Create()
    {
        // Create all required data directories.
        Database.Directory?.Create();
        Content.Create();
    }
}
