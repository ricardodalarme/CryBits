using System.IO;
using System.Windows.Forms;

namespace CryBits.Client.Library
{
    internal static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Diretório dos arquivos
        public static readonly DirectoryInfo Sounds = new DirectoryInfo(Application.StartupPath + @"\Audio\Sounds\");
        public static readonly DirectoryInfo Musics = new DirectoryInfo(Application.StartupPath + @"\Audio\Musics\");
        public static readonly DirectoryInfo Fonts = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fonts\");
        public static readonly FileInfo Options = new FileInfo(Application.StartupPath + @"\Data\Options" + Format);
        public static readonly DirectoryInfo Maps_Data = new DirectoryInfo(Application.StartupPath + @"\Data\Maps\");
        public static readonly FileInfo Tools_Data = new FileInfo(Application.StartupPath + @"\Data\Tools" + Format);
        public static readonly FileInfo Tex_Background = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Background");
        public static readonly FileInfo Tex_Chat = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Chat");
        public static readonly FileInfo Tex_Equipments = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Equipments");
        public static readonly DirectoryInfo Tex_Panels = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Panels\");
        public static readonly DirectoryInfo Tex_Buttons = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Buttons\");
        public static readonly FileInfo Tex_CheckBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\CheckBox");
        public static readonly FileInfo Tex_TextBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\TextBox");
        public static readonly DirectoryInfo Tex_Characters = new DirectoryInfo(Application.StartupPath + @"\Graphics\Characters\");
        public static readonly DirectoryInfo Tex_Tiles = new DirectoryInfo(Application.StartupPath + @"\Graphics\Tiles\");
        public static readonly DirectoryInfo Tex_Faces = new DirectoryInfo(Application.StartupPath + @"\Graphics\Faces\");
        public static readonly DirectoryInfo Tex_Panoramas = new DirectoryInfo(Application.StartupPath + @"\Graphics\Panoramas\");
        public static readonly DirectoryInfo Tex_Fogs = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fogs\");
        public static readonly FileInfo Tex_Weather = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Weather");
        public static readonly FileInfo Tex_Blank = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Blank");
        public static readonly FileInfo Tex_Directions = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Directions");
        public static readonly FileInfo Tex_Shadow = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Shadow");
        public static readonly FileInfo Tex_Bars = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Bars");
        public static readonly FileInfo Tex_Bars_Panel = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Bars_Panel");
        public static readonly DirectoryInfo Tex_Lights = new DirectoryInfo(Application.StartupPath + @"\Graphics\Lights\");
        public static readonly DirectoryInfo Tex_Items = new DirectoryInfo(Application.StartupPath + @"\Graphics\Items\");
        public static readonly FileInfo Tex_Grid = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Grid");
        public static readonly FileInfo Tex_Blood = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Blood");
        public static readonly FileInfo Tex_Party_Bars = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Party_Bars");
        public static readonly FileInfo Tex_Intro = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Intro");

        public static void Create()
        {
            // Cria todos os diretórios do jogo
            Fonts.Create();
            Sounds.Create();
            Musics.Create();
            Maps_Data.Create();
            Tools_Data.Directory.Create();
            Tex_Panoramas.Create();
            Tex_Fogs.Create();
            Tex_Characters.Create();
            Tex_Faces.Create();
            Tex_Panels.Create();
            Tex_Buttons.Create();
            Tex_CheckBox.Directory.Create();
            Tex_TextBox.Directory.Create();
            Tex_Chat.Directory.Create();
            Tex_Background.Directory.Create();
            Tex_Tiles.Create();
            Tex_Lights.Create();
            Tex_Weather.Directory.Create();
            Tex_Blank.Directory.Create();
            Tex_Directions.Directory.Create();
            Tex_Shadow.Directory.Create();
            Tex_Bars.Directory.Create();
            Tex_Bars_Panel.Directory.Create();
            Tex_Items.Create();
            Tex_Grid.Directory.Create();
            Tex_Equipments.Directory.Create();
            Tex_Blood.Directory.Create();
            Tex_Party_Bars.Directory.Create();
        }
    }
}