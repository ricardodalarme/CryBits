namespace CryBits.Client.Framework.Constants;

public static class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Diretório dos arquivos
    public static readonly FileInfo EditorOptions = new(Environment.CurrentDirectory + @"\Data\Editors\Options" + Format);
    public static readonly DirectoryInfo Sounds = new(Environment.CurrentDirectory + @"\Audio\Sounds\");
    public static readonly DirectoryInfo Musics = new(Environment.CurrentDirectory + @"\Audio\Musics\");
    public static readonly DirectoryInfo Fonts = new(Environment.CurrentDirectory + @"\Graphics\Fonts\");
    public static readonly FileInfo Options = new(Environment.CurrentDirectory + @"\Data\Options" + Format);
    public static readonly DirectoryInfo MapsData = new(Environment.CurrentDirectory + @"\Data\Maps\");
    public static readonly DirectoryInfo Tiles = new(Environment.CurrentDirectory + @"\Data\Tiles\");
    public static readonly FileInfo ToolsData = new(Environment.CurrentDirectory + @"\Data\Tools" + Format);
    public static readonly FileInfo TexBackground = new(Environment.CurrentDirectory + @"\Graphics\Interface\Background");
    public static readonly FileInfo TexChat = new(Environment.CurrentDirectory + @"\Graphics\Interface\Chat");
    public static readonly FileInfo TexEquipments = new(Environment.CurrentDirectory + @"\Graphics\Interface\Equipments");
    public static readonly DirectoryInfo TexPanels = new(Environment.CurrentDirectory + @"\Graphics\Interface\Panels\");
    public static readonly DirectoryInfo TexButtons = new(Environment.CurrentDirectory + @"\Graphics\Interface\Buttons\");
    public static readonly FileInfo TexCheckBox = new(Environment.CurrentDirectory + @"\Graphics\Interface\CheckBox");
    public static readonly FileInfo TexTextBox = new(Environment.CurrentDirectory + @"\Graphics\Interface\TextBox");
    public static readonly DirectoryInfo TexCharacters = new(Environment.CurrentDirectory + @"\Graphics\Characters\");
    public static readonly DirectoryInfo TexTiles = new(Environment.CurrentDirectory + @"\Graphics\Tiles\");
    public static readonly DirectoryInfo TexFaces = new(Environment.CurrentDirectory + @"\Graphics\Faces\");
    public static readonly DirectoryInfo TexPanoramas = new(Environment.CurrentDirectory + @"\Graphics\Panoramas\");
    public static readonly DirectoryInfo TexFogs = new(Environment.CurrentDirectory + @"\Graphics\Fogs\");
    public static readonly FileInfo TexWeather = new(Environment.CurrentDirectory + @"\Graphics\Misc\Weather");
    public static readonly FileInfo TexBlank = new(Environment.CurrentDirectory + @"\Graphics\Misc\Blank");
    public static readonly FileInfo TexDirections = new(Environment.CurrentDirectory + @"\Graphics\Misc\Directions");
    public static readonly FileInfo TexShadow = new(Environment.CurrentDirectory + @"\Graphics\Misc\Shadow");
    public static readonly FileInfo TexBars = new(Environment.CurrentDirectory + @"\Graphics\Misc\Bars");
    public static readonly FileInfo TexBarsPanel = new(Environment.CurrentDirectory + @"\Graphics\Misc\Bars_Panel");
    public static readonly DirectoryInfo TexLights = new(Environment.CurrentDirectory + @"\Graphics\Lights\");
    public static readonly DirectoryInfo TexItems = new(Environment.CurrentDirectory + @"\Graphics\Items\");
    public static readonly FileInfo TexGrid = new(Environment.CurrentDirectory + @"\Graphics\Misc\Grid");
    public static readonly FileInfo TexBlood = new(Environment.CurrentDirectory + @"\Graphics\Misc\Blood");
    public static readonly FileInfo TexPartyBars = new(Environment.CurrentDirectory + @"\Graphics\Misc\Party_Bars");
    public static readonly FileInfo TexTransparent = new(Environment.CurrentDirectory + @"\Graphics\Misc\Transparent");

    public static void Create()
    {
        // Cria todos os diretórios do jogo
        EditorOptions.Directory?.Create();
        Sounds.Create();
        Musics.Create();
        Fonts.Create();
        Options.Directory?.Create();
        MapsData.Create();
        Tiles.Create();
        ToolsData.Directory?.Create();
        TexBackground.Directory?.Create();
        TexChat.Directory?.Create();
        TexEquipments.Directory?.Create();
        TexPanels.Create();
        TexButtons.Create();
        TexCheckBox.Directory?.Create();
        TexTextBox.Directory?.Create();
        TexCharacters.Create();
        TexTiles.Create();
        TexFaces.Create();
        TexPanoramas.Create();
        TexFogs.Create();
        TexWeather.Directory?.Create();
        TexBlank.Directory?.Create();
        TexDirections.Directory?.Create();
        TexShadow.Directory?.Create();
        TexBars.Directory?.Create();
        TexBarsPanel.Directory?.Create();
        TexLights.Create();
        TexItems.Create();
        TexGrid.Directory?.Create();
        TexBlood.Directory?.Create();
        TexPartyBars.Directory?.Create();
        TexTransparent.Directory?.Create();
    }
}