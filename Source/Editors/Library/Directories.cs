using System.IO;
using System.Windows.Forms;

class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Editor
    public static FileInfo Options = new FileInfo(Application.StartupPath + @"\Options" + Format);

    // Servidor
    public static FileInfo Server_Data;
    public static DirectoryInfo Classes_Data;
    public static DirectoryInfo Maps_Data;
    public static DirectoryInfo NPCs_Data;
    public static DirectoryInfo Items_Data;
    public static DirectoryInfo Tile_Data;

    // Cliente
    public static DirectoryInfo Fonts;
    public static DirectoryInfo Sounds;
    public static DirectoryInfo Musics;
    public static FileInfo Client_Data;
    public static DirectoryInfo Buttons_Data;
    public static DirectoryInfo TextBoxes_Data;
    public static DirectoryInfo Panels_Data;
    public static DirectoryInfo CheckBoxes_Data;
    public static DirectoryInfo Tex_Painel;
    public static FileInfo Tex_CheckBox;
    public static FileInfo Tex_TextBox;
    public static DirectoryInfo Tex_Buttons;
    public static DirectoryInfo Tex_Characters;
    public static DirectoryInfo Tex_Faces;
    public static DirectoryInfo Tex_Panoramas;
    public static DirectoryInfo Tex_Fogs;
    public static DirectoryInfo Tex_Tiles;
    public static DirectoryInfo Tex_Lights;
    public static DirectoryInfo Tex_Items;
    public static FileInfo Tex_Grid;
    public static FileInfo Tex_Weather;
    public static FileInfo Tex_Blanc;
    public static FileInfo Tex_Directions;
    public static FileInfo Tex_Transparent;
    public static FileInfo Tex_Lighting;

    public static void SetClient()
    {
        string Directory = Lists.Options.Directory_Client;

        // Previne erros
        if (!System.IO.Directory.Exists(Directory))
        {
            Lists.Options.Directory_Client = string.Empty;
            Write.Options();
            return;
        }

        // Demonstra o diretório
        Selection.Objects.txtDirectory_Client.Text = Directory;

        // Cliente
        Fonts = new DirectoryInfo(Directory + @"\Fonts\");
        Sounds = new DirectoryInfo(Directory + @"\Audio\Sounds\");
        Musics = new DirectoryInfo(Directory + @"\Audio\Musics\");
        Client_Data = new FileInfo(Directory + @"\Data\General" + Format);
        Buttons_Data = new DirectoryInfo(Directory + @"\Data\Tools\Buttons\");
        TextBoxes_Data = new DirectoryInfo(Directory + @"\Data\Tools\TextBoxes\");
        Panels_Data = new DirectoryInfo(Directory + @"\Data\Tools\Panels\");
        CheckBoxes_Data = new DirectoryInfo(Directory + @"\Data\Tools\CheckBoxes\");
        Tex_Panoramas = new DirectoryInfo(Directory + @"\Graphics\Panoramas\");
        Tex_Lights = new DirectoryInfo(Directory + @"\Graphics\Lights\");
        Tex_Fogs = new DirectoryInfo(Directory + @"\Graphics\Fogs\");
        Tex_Characters = new DirectoryInfo(Directory + @"\Graphics\Characters\");
        Tex_Faces = new DirectoryInfo(Directory + @"\Graphics\Faces\");
        Tex_Painel = new DirectoryInfo(Directory + @"\Graphics\Interface\Tools\Panels\");
        Tex_Buttons = new DirectoryInfo(Directory + @"\Graphics\Interface\Tools\Buttons\");
        Tex_CheckBox = new FileInfo(Directory + @"\Graphics\Interface\Tools\CheckBox");
        Tex_TextBox = new FileInfo(Directory + @"\Graphics\Interface\Tools\TextBox");
        Tex_Tiles = new DirectoryInfo(Directory + @"\Graphics\Tiles\");
        Tex_Grid = new FileInfo(Directory + @"\Graphics\Grid");
        Tex_Weather = new FileInfo(Directory + @"\Graphics\Weather");
        Tex_Blanc = new FileInfo(Directory + @"\Graphics\Blank");
        Tex_Directions = new FileInfo(Directory + @"\Graphics\Directions");
        Tex_Transparent = new FileInfo(Directory + @"\Graphics\Transparent");
        Tex_Lighting = new FileInfo(Directory + @"\Graphics\Lighting");
        Tex_Items = new DirectoryInfo(Directory + @"\Graphics\Items\");

        // Cria os diretórios
        Fonts.Create();
        Sounds.Create();
        Musics.Create();
        Client_Data.Directory.Create();
        Buttons_Data.Create();
        TextBoxes_Data.Create();
        Panels_Data.Create();
        CheckBoxes_Data.Create();
        Tex_Panoramas.Create();
        Tex_Fogs.Create();
        Tex_Lights.Create();
        Tex_Characters.Create();
        Tex_Faces.Create();
        Tex_Painel.Create();
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

        // Lê os dados gerais do cliente
        Read.Client_Data();
        Graphics.LoadTextures();
        Audio.Sound.Load();
    }

    public static void SetServer()
    {
        string Directory = Lists.Options.Directory_Server;

        // Previne erros
        if (!System.IO.Directory.Exists(Directory))
        {
            Lists.Options.Directory_Server = string.Empty;
            Write.Options();
            return;
        }

        // Demonstra o diretório
        Selection.Objects.txtDirectory_Server.Text = Directory;

        // Define os diretórios
        Server_Data = new FileInfo(Directory + @"\Data\General" + Format);
        Classes_Data = new DirectoryInfo(Directory + @"\Data\Classes\");
        Maps_Data = new DirectoryInfo(Directory + @"\Data\Maps\");
        NPCs_Data = new DirectoryInfo(Directory + @"\Data\NPCs\");
        Items_Data = new DirectoryInfo(Directory + @"\Data\Items\");
        Tile_Data = new DirectoryInfo(Directory + @"\Data\Tiles\");

        // Cria os diretórios
        Server_Data.Directory.Create();
        Classes_Data.Create();
        NPCs_Data.Create();
        Maps_Data.Create();
        Items_Data.Create();
        Tile_Data.Create();

        // Lê os dados gerais do servidor
        Read.Server_Data();
    }
}