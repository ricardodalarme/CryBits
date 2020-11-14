using System.IO;
using System.Windows.Forms;
using CryBits.Editors.Media;
using CryBits.Editors.Media.Audio;

namespace CryBits.Editors.Library
{
    internal static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Editor
        public static readonly FileInfo Options = new FileInfo(Application.StartupPath + @"\Options" + Format);

        // Cliente
        public static readonly DirectoryInfo Fonts = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fonts\");
        public static readonly DirectoryInfo Sounds = new DirectoryInfo(Application.StartupPath + @"\Audio\Sounds\");
        public static readonly DirectoryInfo Musics = new DirectoryInfo(Application.StartupPath + @"\Audio\Musics\");
        public static readonly DirectoryInfo Tiles = new DirectoryInfo(Application.StartupPath + @"\Data\Tiles\");
        public static readonly FileInfo Tools = new FileInfo(Application.StartupPath + @"\Data\Tools" + Format);
        public static readonly DirectoryInfo TexPanels = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Panels\");
        public static readonly FileInfo TexCheckBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\CheckBox");
        public static readonly FileInfo TexTextBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\TextBox");
        public static readonly DirectoryInfo TexButtons = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Buttons\");
        public static readonly DirectoryInfo TexCharacters = new DirectoryInfo(Application.StartupPath + @"\Graphics\Characters\");
        public static readonly DirectoryInfo TexFaces = new DirectoryInfo(Application.StartupPath + @"\Graphics\Faces\");
        public static readonly DirectoryInfo TexPanoramas = new DirectoryInfo(Application.StartupPath + @"\Graphics\Panoramas\");
        public static readonly DirectoryInfo TexFogs = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fogs\");
        public static readonly DirectoryInfo TexTiles = new DirectoryInfo(Application.StartupPath + @"\Graphics\Tiles\");
        public static readonly DirectoryInfo TexItems = new DirectoryInfo(Application.StartupPath + @"\Graphics\Items\");
        public static readonly FileInfo TexGrid = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Grid");
        public static readonly FileInfo TexWeather = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Weather");
        public static readonly FileInfo TexBlanc = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Blank");
        public static readonly FileInfo TexDirections = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Directions");
        public static readonly FileInfo TexTransparent = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Transparent");
        public static readonly FileInfo TexLighting = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Lighting");

        public static void Create()
        {
            // Cria os diretórios
            Fonts.Create();
            Sounds.Create();
            Musics.Create();
            Tiles.Create();
            Tools.Directory.Create();
            TexPanoramas.Create();
            TexFogs.Create();
            TexCharacters.Create();
            TexFaces.Create();
            TexPanels.Create();
            TexButtons.Create();
            TexCheckBox.Directory.Create();
            TexTextBox.Directory.Create();
            TexTiles.Create();
            TexGrid.Directory.Create();
            TexWeather.Directory.Create();
            TexBlanc.Directory.Create();
            TexDirections.Directory.Create();
            TexTransparent.Directory.Create();
            TexLighting.Directory.Create();
            TexItems.Create();

            // Lê os dados do lado do cliente
            Graphics.Init();
            Sound.Load();
            Read.Tiles();
            Read.Tools();
        }
    }
}