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
        public static readonly DirectoryInfo MapsData = new DirectoryInfo(Application.StartupPath + @"\Data\Maps\");
        public static readonly FileInfo ToolsData = new FileInfo(Application.StartupPath + @"\Data\Tools" + Format);
        public static readonly FileInfo TexBackground = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Background");
        public static readonly FileInfo TexChat = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Chat");
        public static readonly FileInfo TexEquipments = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Equipments");
        public static readonly DirectoryInfo TexPanels = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Panels\");
        public static readonly DirectoryInfo TexButtons = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Buttons\");
        public static readonly FileInfo TexCheckBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\CheckBox");
        public static readonly FileInfo TexTextBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\TextBox");
        public static readonly DirectoryInfo TexCharacters = new DirectoryInfo(Application.StartupPath + @"\Graphics\Characters\");
        public static readonly DirectoryInfo TexTiles = new DirectoryInfo(Application.StartupPath + @"\Graphics\Tiles\");
        public static readonly DirectoryInfo TexFaces = new DirectoryInfo(Application.StartupPath + @"\Graphics\Faces\");
        public static readonly DirectoryInfo TexPanoramas = new DirectoryInfo(Application.StartupPath + @"\Graphics\Panoramas\");
        public static readonly DirectoryInfo TexFogs = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fogs\");
        public static readonly FileInfo TexWeather = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Weather");
        public static readonly FileInfo TexBlank = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Blank");
        public static readonly FileInfo TexDirections = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Directions");
        public static readonly FileInfo TexShadow = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Shadow");
        public static readonly FileInfo TexBars = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Bars");
        public static readonly FileInfo TexBarsPanel = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Bars_Panel");
        public static readonly DirectoryInfo TexLights = new DirectoryInfo(Application.StartupPath + @"\Graphics\Lights\");
        public static readonly DirectoryInfo TexItems = new DirectoryInfo(Application.StartupPath + @"\Graphics\Items\");
        public static readonly FileInfo TexGrid = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Grid");
        public static readonly FileInfo TexBlood = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Blood");
        public static readonly FileInfo TexPartyBars = new FileInfo(Application.StartupPath + @"\Graphics\Misc\Party_Bars");
        public static readonly FileInfo TexIntro = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Intro");

        public static void Create()
        {
            // Cria todos os diretórios do jogo
            Fonts.Create();
            Sounds.Create();
            Musics.Create();
            MapsData.Create();
            ToolsData.Directory.Create();
            TexPanoramas.Create();
            TexFogs.Create();
            TexCharacters.Create();
            TexFaces.Create();
            TexPanels.Create();
            TexButtons.Create();
            TexCheckBox.Directory.Create();
            TexTextBox.Directory.Create();
            TexChat.Directory.Create();
            TexBackground.Directory.Create();
            TexTiles.Create();
            TexLights.Create();
            TexWeather.Directory.Create();
            TexBlank.Directory.Create();
            TexDirections.Directory.Create();
            TexShadow.Directory.Create();
            TexBars.Directory.Create();
            TexBarsPanel.Directory.Create();
            TexItems.Create();
            TexGrid.Directory.Create();
            TexEquipments.Directory.Create();
            TexBlood.Directory.Create();
            TexPartyBars.Directory.Create();
        }
    }
}