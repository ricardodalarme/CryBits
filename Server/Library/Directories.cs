using System;
using System.IO;

namespace CryBits.Server.Library;

internal static class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Diretório base
    private static readonly string BaseDir = Environment.CurrentDirectory;

    // Diretório dos arquivos
    public static readonly FileInfo Defaults = new(Path.Combine(BaseDir, "Data", "Defaults") + Format);
    public static readonly DirectoryInfo Accounts = new(Path.Combine(BaseDir, "Data", "Accounts"));
    public static readonly FileInfo Characters = new(Path.Combine(BaseDir, "Data", "Characters") + Format);
    public static readonly DirectoryInfo Classes = new(Path.Combine(BaseDir, "Data", "Classes"));
    public static readonly DirectoryInfo Maps = new(Path.Combine(BaseDir, "Data", "Maps"));
    public static readonly DirectoryInfo Npcs = new(Path.Combine(BaseDir, "Data", "Npcs"));
    public static readonly DirectoryInfo Items = new(Path.Combine(BaseDir, "Data", "Items"));
    public static readonly DirectoryInfo Shops = new(Path.Combine(BaseDir, "Data", "Shops"));

    public static void Create()
    {
        // Cria todos os diretórios do jogo
        Defaults.Directory?.Create();
        Accounts.Create();
        Characters.Directory?.Create();
        Classes.Create();
        Maps.Create();
        Npcs.Create();
        Items.Create();
        Shops.Create();
    }
}
