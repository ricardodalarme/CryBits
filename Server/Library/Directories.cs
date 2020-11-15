using System.IO;
using System.Windows.Forms;

namespace CryBits.Server.Library
{
    internal static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Diretório dos arquivos
        public static readonly FileInfo Settings = new FileInfo(Application.StartupPath + @"\Data\Settings" + Format);
        public static readonly DirectoryInfo Accounts = new DirectoryInfo(Application.StartupPath + @"\Data\Accounts\");
        public static readonly FileInfo Characters = new FileInfo(Application.StartupPath + @"\Data\Characters" + Format);
        public static readonly DirectoryInfo Classes = new DirectoryInfo(Application.StartupPath + @"\Data\Classes\");
        public static readonly DirectoryInfo Maps = new DirectoryInfo(Application.StartupPath + @"\Data\Maps\");
        public static readonly DirectoryInfo NPCs = new DirectoryInfo(Application.StartupPath + @"\Data\NPCs\");
        public static readonly DirectoryInfo Items = new DirectoryInfo(Application.StartupPath + @"\Data\Items\");
        public static readonly DirectoryInfo Shops = new DirectoryInfo(Application.StartupPath + @"\Data\Shops\");

        public static void Create()
        {
            // Cria todos os diretórios do jogo
            Settings.Directory.Create();
            Accounts.Create();
            Characters.Directory.Create();
            Classes.Create();
            Maps.Create();
            NPCs.Create();
            Items.Create();
            Shops.Create();
        }
    }
}