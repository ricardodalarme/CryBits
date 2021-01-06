using System.IO;
using System.Windows.Forms;

namespace CryBits.Client.Library
{
    internal static class Directories
    {
        // Formato de todos os arquivos de dados
        public const string Format = ".dat";

        // Diretório dos arquivos
        public static readonly DirectoryInfo Sounds = new(Application.StartupPath + @"\Audio\Sounds\");
        public static readonly DirectoryInfo Musics = new(Application.StartupPath + @"\Audio\Musics\");
        public static readonly DirectoryInfo Fonts = new(Application.StartupPath + @"\Graphics\Fonts\");
        public static readonly FileInfo Options = new(Application.StartupPath + @"\Data\Options" + Format);
        public static readonly DirectoryInfo MapsData = new(Application.StartupPath + @"\Data\Maps\");
        public static readonly FileInfo ToolsData = new(Application.StartupPath + @"\Data\Tools" + Format);
        public static readonly FileInfo TexBackground = new(Application.StartupPath + @"\Graphics\Interface\Background");
        public static readonly FileInfo TexChat = new(Application.StartupPath + @"\Graphics\Interface\Chat");
        public static readonly FileInfo TexEquipments = new(Application.StartupPath + @"\Graphics\Interface\Equipments");
        public static readonly DirectoryInfo TexPanels = new(Application.StartupPath + @"\Graphics\Interface\Panels\");
        public static readonly DirectoryInfo TexButtons = new(Application.StartupPath + @"\Graphics\Interface\Buttons\");
        public static readonly FileInfo TexCheckBox = new(Application.StartupPath + @"\Graphics\Interface\CheckBox");
        public static readonly FileInfo TexTextBox = new(Application.StartupPath + @"\Graphics\Interface\TextBox");
        public static readonly DirectoryInfo TexCharacters = new(Application.StartupPath + @"\Graphics\Characters\");
        public static readonly DirectoryInfo TexTiles = new(Application.StartupPath + @"\Graphics\Tiles\");
        public static readonly DirectoryInfo TexFaces = new(Application.StartupPath + @"\Graphics\Faces\");
        public static readonly DirectoryInfo TexPanoramas = new(Application.StartupPath + @"\Graphics\Panoramas\");
        public static readonly DirectoryInfo TexFogs = new(Application.StartupPath + @"\Graphics\Fogs\");
        public static readonly FileInfo TexWeather = new(Application.StartupPath + @"\Graphics\Misc\Weather");
        public static readonly FileInfo TexBlank = new(Application.StartupPath + @"\Graphics\Misc\Blank");
        public static readonly FileInfo TexDirections = new(Application.StartupPath + @"\Graphics\Misc\Directions");
        public static readonly FileInfo TexShadow = new(Application.StartupPath + @"\Graphics\Misc\Shadow");
        public static readonly FileInfo TexBars = new(Application.StartupPath + @"\Graphics\Misc\Bars");
        public static readonly FileInfo TexBarsPanel = new(Application.StartupPath + @"\Graphics\Misc\Bars_Panel");
        public static readonly DirectoryInfo TexLights = new(Application.StartupPath + @"\Graphics\Lights\");
        public static readonly DirectoryInfo TexItems = new(Application.StartupPath + @"\Graphics\Items\");
        public static readonly FileInfo TexGrid = new(Application.StartupPath + @"\Graphics\Misc\Grid");
        public static readonly FileInfo TexBlood = new(Application.StartupPath + @"\Graphics\Misc\Blood");
        public static readonly FileInfo TexPartyBars = new(Application.StartupPath + @"\Graphics\Misc\Party_Bars");

        public static void Create()
        {
            // Cria todos os diretórios do jogo
            Fonts.Create();
            Sounds.Create();
            Musics.Create();
            MapsData.Create();
            ToolsData.Directory?.Create();
            TexPanoramas.Create();
            TexFogs.Create();
            TexCharacters.Create();
            TexFaces.Create();
            TexPanels.Create();
            TexButtons.Create();
            TexCheckBox.Directory?.Create();
            TexTextBox.Directory?.Create();
            TexChat.Directory?.Create();
            TexBackground.Directory?.Create();
            TexTiles.Create();
            TexLights.Create();
            TexWeather.Directory?.Create();
            TexBlank.Directory?.Create();
            TexDirections.Directory?.Create();
            TexShadow.Directory?.Create();
            TexBars.Directory?.Create();
            TexBarsPanel.Directory?.Create();
            TexItems.Create();
            TexGrid.Directory?.Create();
            TexEquipments.Directory?.Create();
            TexBlood.Directory?.Create();
            TexPartyBars.Directory?.Create();
        }
    }
}