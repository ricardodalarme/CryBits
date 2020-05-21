using System.IO;
using System.Windows.Forms;

class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Editor
    public static FileInfo Options = new FileInfo(Application.StartupPath + @"\Options" + Format);

    // Cliente
    public static DirectoryInfo Fonts;
    public static DirectoryInfo Sounds;
    public static DirectoryInfo Musics;
    public static DirectoryInfo Tiles;
    public static FileInfo Tools;
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
        Sounds = new DirectoryInfo(Directory + @"\Audio\Sounds\");
        Musics = new DirectoryInfo(Directory + @"\Audio\Musics\");
        Tools = new FileInfo(Directory + @"\Data\Tools" + Format);
        Tiles = new DirectoryInfo(Directory + @"\Data\Tiles\");
        Fonts = new DirectoryInfo(Directory + @"\Graphics\Fonts\");
        Tex_Panoramas = new DirectoryInfo(Directory + @"\Graphics\Panoramas\");
        Tex_Lights = new DirectoryInfo(Directory + @"\Graphics\Lights\");
        Tex_Fogs = new DirectoryInfo(Directory + @"\Graphics\Fogs\");
        Tex_Characters = new DirectoryInfo(Directory + @"\Graphics\Characters\");
        Tex_Faces = new DirectoryInfo(Directory + @"\Graphics\Faces\");
        Tex_Painel = new DirectoryInfo(Directory + @"\Graphics\Interface\Panels\");
        Tex_Buttons = new DirectoryInfo(Directory + @"\Graphics\Interface\Buttons\");
        Tex_CheckBox = new FileInfo(Directory + @"\Graphics\Interface\CheckBox");
        Tex_TextBox = new FileInfo(Directory + @"\Graphics\Interface\TextBox");
        Tex_Tiles = new DirectoryInfo(Directory + @"\Graphics\Tiles\");
        Tex_Grid = new FileInfo(Directory + @"\Graphics\Misc\Grid");
        Tex_Weather = new FileInfo(Directory + @"\Graphics\Misc\Weather");
        Tex_Blanc = new FileInfo(Directory + @"\Graphics\Misc\Blank");
        Tex_Directions = new FileInfo(Directory + @"\Graphics\Misc\Directions");
        Tex_Transparent = new FileInfo(Directory + @"\Graphics\Misc\Transparent");
        Tex_Lighting = new FileInfo(Directory + @"\Graphics\Misc\Lighting");
        Tex_Items = new DirectoryInfo(Directory + @"\Graphics\Items\");

        // Cria os diretórios
        Fonts.Create();
        Sounds.Create();
        Musics.Create();
        Tiles.Create();
        Tools.Directory.Create();
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
        Graphics.LoadTextures();
        Audio.Sound.Load();
    }
}