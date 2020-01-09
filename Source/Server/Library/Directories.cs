using System.IO;
using System.Windows.Forms;

class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Diretório dos arquivos
    public static FileInfo Server_Data = new FileInfo(Application.StartupPath + @"\Data\General" + Format);
    public static DirectoryInfo Accounts = new DirectoryInfo(Application.StartupPath + @"\Data\Accounts\");
    public static FileInfo Characters = new FileInfo(Application.StartupPath + @"\Data\Characters" + Format);
    public static DirectoryInfo Classes = new DirectoryInfo(Application.StartupPath + @"\Data\Classes\");
    public static DirectoryInfo Maps = new DirectoryInfo(Application.StartupPath + @"\Data\Maps\");
    public static DirectoryInfo NPCs = new DirectoryInfo(Application.StartupPath + @"\Data\NPCs\");
    public static DirectoryInfo Items = new DirectoryInfo(Application.StartupPath + @"\Data\Items\");
    public static DirectoryInfo Tiles = new DirectoryInfo(Application.StartupPath + @"\Data\Tiles\");

    public static void Create()
    {
        // Cria todos os diretórios do jogo
        Server_Data.Directory.Create();
        Accounts.Create();
        Characters.Directory.Create();
        Classes.Create();
        Maps.Create();
        NPCs.Create();
        Items.Create();
        Tiles.Create();
    }
}