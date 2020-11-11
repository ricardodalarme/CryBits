using System.IO;
using System.Windows.Forms;

namespace CryBits.Server.Library
{
    static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Diretório dos arquivos
        public static FileInfo Settings = new FileInfo(Application.StartupPath + @"\Data\Settings" + Format);
        public static DirectoryInfo Accounts = new DirectoryInfo(Application.StartupPath + @"\Data\Accounts\");
        public static FileInfo Characters = new FileInfo(Application.StartupPath + @"\Data\Characters" + Format);
        public static DirectoryInfo Classes = new DirectoryInfo(Application.StartupPath + @"\Data\Classes\");
        public static DirectoryInfo Maps = new DirectoryInfo(Application.StartupPath + @"\Data\Maps\");
        public static DirectoryInfo NPCs = new DirectoryInfo(Application.StartupPath + @"\Data\NPCs\");
        public static DirectoryInfo Items = new DirectoryInfo(Application.StartupPath + @"\Data\Items\");
        public static DirectoryInfo Shops = new DirectoryInfo(Application.StartupPath + @"\Data\Shops\");

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