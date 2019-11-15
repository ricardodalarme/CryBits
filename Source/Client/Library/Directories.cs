using System.IO;
using System.Windows.Forms;

class Directories
{
    // Formato de todos os arquivos de dados
    public const string Format = ".dat";

    // Diretório dos arquivos
    public static DirectoryInfo Sounds = new DirectoryInfo(Application.StartupPath + @"\Audio\Sounds\");
    public static DirectoryInfo Musics = new DirectoryInfo(Application.StartupPath + @"\Audio\Musics\");
    public static DirectoryInfo Fonts = new DirectoryInfo(Application.StartupPath + @"\Fonts\");
    public static FileInfo Options = new FileInfo(Application.StartupPath + @"\Data\Options" + Format);
    public static FileInfo Client_Data = new FileInfo(Application.StartupPath + @"\Data\General" + Format);
    public static DirectoryInfo Maps_Data = new DirectoryInfo(Application.StartupPath + @"\Data\Maps\");
    public static DirectoryInfo Buttons_Data = new DirectoryInfo(Application.StartupPath + @"\Data\Tools\Buttons\");
    public static DirectoryInfo TextBoxes_Data = new DirectoryInfo(Application.StartupPath + @"\Data\Tools\TextBoxes\");
    public static DirectoryInfo Panels_Data = new DirectoryInfo(Application.StartupPath + @"\Data\Tools\Panels\");
    public static DirectoryInfo CheckBoxes_Data = new DirectoryInfo(Application.StartupPath + @"\Data\Tools\CheckBoxes\");
    public static FileInfo Tex_Background = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Background");
    public static FileInfo Tex_Chat = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Chat");
    public static FileInfo Tex_Equipments = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Equipments");
    public static DirectoryInfo Tex_Panels = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Tools\Panels\");
    public static DirectoryInfo Tex_Buttons = new DirectoryInfo(Application.StartupPath + @"\Graphics\Interface\Tools\Buttons\");
    public static FileInfo Tex_CheckBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Tools\CheckBox");
    public static FileInfo Tex_TextBox = new FileInfo(Application.StartupPath + @"\Graphics\Interface\Tools\TextBox");
    public static DirectoryInfo Tex_Characters = new DirectoryInfo(Application.StartupPath + @"\Graphics\Characters\");
    public static DirectoryInfo Tex_Tiles = new DirectoryInfo(Application.StartupPath + @"\Graphics\Tiles\");
    public static DirectoryInfo Tex_Faces = new DirectoryInfo(Application.StartupPath + @"\Graphics\Faces\");
    public static DirectoryInfo Tex_Panoramas = new DirectoryInfo(Application.StartupPath + @"\Graphics\Panoramas\");
    public static DirectoryInfo Tex_Fogs = new DirectoryInfo(Application.StartupPath + @"\Graphics\Fogs\");
    public static FileInfo Tex_Weather = new FileInfo(Application.StartupPath + @"\Graphics\Weather");
    public static FileInfo Tex_Blank = new FileInfo(Application.StartupPath + @"\Graphics\Blank");
    public static FileInfo Tex_Directions = new FileInfo(Application.StartupPath + @"\Graphics\Directions");
    public static FileInfo Tex_Shadow = new FileInfo(Application.StartupPath + @"\Graphics\Shadow");
    public static FileInfo Tex_Bars = new FileInfo(Application.StartupPath + @"\Graphics\Bars");
    public static FileInfo Tex_Bars_Panel = new FileInfo(Application.StartupPath + @"\Graphics\Bars_Panel");
    public static DirectoryInfo Tex_Lights = new DirectoryInfo(Application.StartupPath + @"\Graphics\Lights\");
    public static DirectoryInfo Tex_Items = new DirectoryInfo(Application.StartupPath + @"\Graphics\Items\");
    public static FileInfo Tex_Grid = new FileInfo(Application.StartupPath + @"\Graphics\Grid");

    public static void Create()
    {
        // Cria todos os diretórios do jogo
        Fonts.Create();
        Sounds.Create();
        Musics.Create();
        Client_Data.Directory.Create();
        Maps_Data.Create();
        Buttons_Data.Create();
        TextBoxes_Data.Create();
        Panels_Data.Create();
        CheckBoxes_Data.Create();
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
    }
}