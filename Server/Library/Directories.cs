using System;
using System.IO;

namespace CryBits.Server.Library
{
    internal static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Diretório dos arquivos
        public static readonly FileInfo Defaults = new FileInfo(Environment.CurrentDirectory + @"\Data\Defaults" + Format);
        public static readonly DirectoryInfo Accounts = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Accounts\");
        public static readonly FileInfo Characters = new FileInfo(Environment.CurrentDirectory + @"\Data\Characters" + Format);
        public static readonly DirectoryInfo Classes = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Classes\");
        public static readonly DirectoryInfo Maps = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Maps\");
        public static readonly DirectoryInfo Npcs = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Npcs\");
        public static readonly DirectoryInfo Items = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Items\");
        public static readonly DirectoryInfo Shops = new DirectoryInfo(Environment.CurrentDirectory + @"\Data\Shops\");

        public static void Create()
        {
            // Cria todos os diretórios do jogo
            Defaults.Directory.Create();
            Accounts.Create();
            Characters.Directory.Create();
            Classes.Create();
            Maps.Create();
            Npcs.Create();
            Items.Create();
            Shops.Create();
        }
    }
}