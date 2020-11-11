using System.IO;
using System.Windows.Forms;

namespace CryBits.Editors.Library
{
    static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Editor
        public static FileInfo Options = new FileInfo(Application.StartupPath + @"\Options" + Format);

        // Cliente
        public static DirectoryInfo Fonts = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fonts\");
        public static DirectoryInfo Sounds = new DirectoryInfo(Application.StartupPath + @"\Audio\Sounds\");
        public static DirectoryInfo Musics = new DirectoryInfo(Application.StartupPath + @"\Audio\Musics\");
        public static DirectoryInfo Tiles = new DirectoryInfo(Application.StartupPath + @"\Data\Tiles\");
        public static FileInfo Tools = new FileInfo(Application.StartupPath + @"\Data\Tools" + Format);
        public static DirectoryInfo Tex_Panels = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Panels\");
        public static FileInfo Tex_CheckBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\CheckBox");
        public static FileInfo Tex_TextBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\TextBox");
        public static DirectoryInfo Tex_Buttons = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Buttons\");
        public static DirectoryInfo Tex_Characters = new DirectoryInfo(Application.StartupPath + @"\Graphics\Characters\");
        public static DirectoryInfo Tex_Faces = new DirectoryInfo(Application.StartupPath + @"\Graphics\Faces\");
        public static DirectoryInfo Tex_Panoramas = new DirectoryInfo(Application.StartupPath + @"\Graphics\Panoramas\");
        public static DirectoryInfo Tex_Fogs = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fogs\");
        public static DirectoryInfo Tex_Tiles = new DirectoryInfo(Application.StartupPath + @"\Graphics\Tiles\");
        public static DirectoryInfo Tex_Items = new DirectoryInfo(Application.StartupPath + @"\Graphics\Items\");
        public static FileInfo Tex_Grid = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Grid");
        public static FileInfo Tex_Weather = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Weather");
        public static FileInfo Tex_Blanc = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Blank");
        public static FileInfo Tex_Directions = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Directions");
        public static FileInfo Tex_Transparent = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Transparent");
        public static FileInfo Tex_Lighting = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Lighting");

        public static void Create()
        {
            // Cria os diretórios
            Fonts.Create();
            Sounds.Create();
            Musics.Create();
            Tiles.Create();
            Tools.Directory.Create();
            Tex_Panoramas.Create();
            Tex_Fogs.Create();
            Tex_Characters.Create();
            Tex_Faces.Create();
            Tex_Panels.Create();
            Tex_Buttons.Create();
            Tex_CheckBox.Directory.Create();
            Tex_TextBox.Directory.Create();
            Tex_Tiles.Create();
            Tex_Grid.Directory.Create();
            Tex_Weather.Directory.Create();
            Tex_Blanc.Directory.Create();
            Tex_Directions.Directory.Create();
            Tex_Transparent.Directory.Create();
            Tex_Lighting.Directory.Create();
            Tex_Items.Create();

            // Lê os dados do lado do cliente
            Graphics.Init();
            Audio.Sound.Load();
            Read.Tiles();
            Read.Tools();
        }
    }
}