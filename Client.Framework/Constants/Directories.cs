namespace CryBits.Client.Framework.Constants;

public static class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Diretório base — AppContext.BaseDirectory always resolves to the folder
    // containing the running assembly, regardless of the process working directory.
    private static readonly string BaseDir = AppContext.BaseDirectory;

    // Diretório dos arquivos
    public static readonly FileInfo EditorOptions = new(Path.Combine(BaseDir, "Data", "Editors", "Options") + Format);
    public static readonly DirectoryInfo Sounds = new(Path.Combine(BaseDir, "Audio", "Sounds"));
    public static readonly DirectoryInfo Musics = new(Path.Combine(BaseDir, "Audio", "Musics"));
    public static readonly DirectoryInfo Fonts = new(Path.Combine(BaseDir, "Graphics", "Fonts"));
    public static readonly FileInfo Options = new(Path.Combine(BaseDir, "Data", "Options") + Format);
    public static readonly DirectoryInfo MapsData = new(Path.Combine(BaseDir, "Data", "Maps"));
    public static readonly DirectoryInfo Tiles = new(Path.Combine(BaseDir, "Data", "Tiles"));
    public static readonly FileInfo ToolsData = new(Path.Combine(BaseDir, "Data", "Tools") + Format);
    public static readonly FileInfo TexBackground = new(Path.Combine(BaseDir, "Graphics", "Interface", "Background"));
    public static readonly FileInfo TexChat = new(Path.Combine(BaseDir, "Graphics", "Interface", "Chat"));
    public static readonly FileInfo TexEquipments = new(Path.Combine(BaseDir, "Graphics", "Interface", "Equipments"));
    public static readonly DirectoryInfo TexPanels = new(Path.Combine(BaseDir, "Graphics", "Interface", "Panels"));
    public static readonly DirectoryInfo TexButtons = new(Path.Combine(BaseDir, "Graphics", "Interface", "Buttons"));
    public static readonly FileInfo TexCheckBox = new(Path.Combine(BaseDir, "Graphics", "Interface", "CheckBox"));
    public static readonly FileInfo TexTextBox = new(Path.Combine(BaseDir, "Graphics", "Interface", "TextBox"));
    public static readonly DirectoryInfo TexCharacters = new(Path.Combine(BaseDir, "Graphics", "Characters"));
    public static readonly DirectoryInfo TexTiles = new(Path.Combine(BaseDir, "Graphics", "Tiles"));
    public static readonly DirectoryInfo TexFaces = new(Path.Combine(BaseDir, "Graphics", "Faces"));
    public static readonly DirectoryInfo TexPanoramas = new(Path.Combine(BaseDir, "Graphics", "Panoramas"));
    public static readonly DirectoryInfo TexFogs = new(Path.Combine(BaseDir, "Graphics", "Fogs"));
    public static readonly FileInfo TexWeather = new(Path.Combine(BaseDir, "Graphics", "Misc", "Weather"));
    public static readonly FileInfo TexBlank = new(Path.Combine(BaseDir, "Graphics", "Misc", "Blank"));
    public static readonly FileInfo TexDirections = new(Path.Combine(BaseDir, "Graphics", "Misc", "Directions"));
    public static readonly FileInfo TexShadow = new(Path.Combine(BaseDir, "Graphics", "Misc", "Shadow"));
    public static readonly FileInfo TexBars = new(Path.Combine(BaseDir, "Graphics", "Misc", "Bars"));
    public static readonly FileInfo TexBarsPanel = new(Path.Combine(BaseDir, "Graphics", "Misc", "Bars_Panel"));
    public static readonly DirectoryInfo TexLights = new(Path.Combine(BaseDir, "Graphics", "Lights"));
    public static readonly DirectoryInfo TexItems = new(Path.Combine(BaseDir, "Graphics", "Items"));
    public static readonly FileInfo TexGrid = new(Path.Combine(BaseDir, "Graphics", "Misc", "Grid"));
    public static readonly FileInfo TexBlood = new(Path.Combine(BaseDir, "Graphics", "Misc", "Blood"));
    public static readonly FileInfo TexPartyBars = new(Path.Combine(BaseDir, "Graphics", "Misc", "Party_Bars"));
    public static readonly FileInfo TexTransparent = new(Path.Combine(BaseDir, "Graphics", "Misc", "Transparent"));

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
