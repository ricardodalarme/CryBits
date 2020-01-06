using System;
using System.Drawing;
using System.Windows.Forms;

public partial class Editor_Maps_Properties : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Maps_Properties Objects = new Editor_Maps_Properties();

    // Dados temporários
    public static int Selected;

    public Editor_Maps_Properties()
    {
        InitializeComponent();
    }

    public static void Open()
    {
        Selected = Editor_Maps.Selected;

        // Lista todos os valores
        Update_List();

        // Limites
        Objects.numPanorama.Maximum = Graphics.Tex_Panorama.GetUpperBound(0);
        Objects.numFog_Texture.Maximum = Graphics.Tex_Fog.GetUpperBound(0);
        Objects.numWidth.Minimum = Globals.Min_Map_Width;
        Objects.numHeight.Minimum = Globals.Min_Map_Height;
        Objects.numWeather_Intensity.Maximum = Globals.Max_Weather_Intensity;
        Objects.numLink_Up.Maximum = Lists.Map.GetUpperBound(0);
        Objects.numLink_Down.Maximum = Lists.Map.GetUpperBound(0);
        Objects.numLink_Left.Maximum = Lists.Map.GetUpperBound(0);
        Objects.numLink_Right.Maximum = Lists.Map.GetUpperBound(0);
        Objects.numLight_Global.Maximum = Graphics.Tex_Light.GetUpperBound(0);

        // Define os valores
        Objects.txtName.Text = Lists.Map[Selected].Name;
        Objects.numWidth.Value = Lists.Map[Selected].Width;
        Objects.numHeight.Value = Lists.Map[Selected].Height;
        Objects.cmbMoral.SelectedIndex = Lists.Map[Selected].Moral;
        Objects.cmbMusic.SelectedIndex = Lists.Map[Selected].Music;
        Objects.numPanorama.Value = Lists.Map[Selected].Panorama;
        Objects.numColor_Red.Value = Color.FromArgb(Lists.Map[Selected].Color).R;
        Objects.numColor_Green.Value = Color.FromArgb(Lists.Map[Selected].Color).G;
        Objects.numColor_Blue.Value = Color.FromArgb(Lists.Map[Selected].Color).B;
        Objects.cmbWeather.SelectedIndex = Lists.Map[Selected].Weather.Type;
        Objects.numWeather_Intensity.Value = Lists.Map[Selected].Weather.Intensity;
        Objects.numFog_Texture.Value = Lists.Map[Selected].Fog.Texture;
        Objects.numFog_Speed_X.Value = Lists.Map[Selected].Fog.Speed_X;
        Objects.numFog_Speed_Y.Value = Lists.Map[Selected].Fog.Speed_Y;
        Objects.numFog_Transparency.Value = Lists.Map[Selected].Fog.Alpha;
        Objects.numLink_Up.Value = Lists.Map[Selected].Link[(byte)Globals.Directions.Up];
        Objects.numLink_Down.Value = Lists.Map[Selected].Link[(byte)Globals.Directions.Down];
        Objects.numLink_Left.Value = Lists.Map[Selected].Link[(byte)Globals.Directions.Left];
        Objects.numLink_Right.Value = Lists.Map[Selected].Link[(byte)Globals.Directions.Right];
        Objects.numLight_Global.Value = Lists.Map[Selected].Light_Global;
        Objects.numLighting.Value = Lists.Map[Selected].Lighting;

        // Abre a janela
        Objects.ShowDialog();
    }

    private static void Update_List()
    {
        // Limpa
        Objects.cmbMoral.Items.Clear();
        Objects.cmbWeather.Items.Clear();
        Objects.cmbMusic.Items.Clear();
        Objects.cmbMusic.Items.Add("None");

        // Lista os valores
        for (byte i = 0; i < (byte)Globals.Map_Morals.Count; i++) Objects.cmbMoral.Items.Add((Globals.Map_Morals)i);
        for (byte i = 0; i < (byte)Globals.Weathers.Count; i++) Objects.cmbWeather.Items.Add((Globals.Weathers)i);
        for (byte i = 1; i < (byte)Audio.Musics.Amount; i++) Objects.cmbMusic.Items.Add((Audio.Musics)i);
    }

    private void Map_Resize()
    {
        byte Width_New = (byte)numWidth.Value, Height_New = (byte)numHeight.Value;
        int Width_Difference, Height_Difference;

        // Somente se necessário
        if (Lists.Map[Selected].Width == Width_New && Lists.Map[Selected].Height == Height_New) return;

        // Redimensiona os azulejos
        Lists.Structures.Map_Tile_Data[,] TempTile;
        Lists.Structures.Map_Tile[,] TempTile2;

        // Calcula a diferença
        Width_Difference = Width_New - Lists.Map[Selected].Width;
        Height_Difference = Height_New - Lists.Map[Selected].Height;

        // Azulejo1
        for (byte c = 0; c < Lists.Map[Selected].Layer.Count; c++)
        {
            TempTile = new Lists.Structures.Map_Tile_Data[Width_New + 1, Height_New + 1];

            for (byte x = 0; x <= Width_New; x++)
                for (byte y = 0; y <= Height_New; y++)
                {
                    // Redimensiona para frente
                    if (!chkReverse.Checked)
                        if (x <= Lists.Map[Selected].Width && y <= Lists.Map[Selected].Height)
                            TempTile[x, y] = Lists.Map[Selected].Layer[c].Tile[x, y];
                        else
                        {
                            TempTile[x, y] = new Lists.Structures.Map_Tile_Data();
                            TempTile[x, y].Mini = new Point[4];
                        }
                    // Redimensiona para trás
                    else
                    {
                        if (x < Width_Difference || y < Height_Difference)
                        {
                            TempTile[x, y] = new Lists.Structures.Map_Tile_Data();
                            TempTile[x, y].Mini = new Point[4];
                        }
                        else
                            TempTile[x, y] = Lists.Map[Selected].Layer[c].Tile[x - Width_Difference, y - Height_Difference];
                    }
                }

            // Define os dados
            Lists.Map[Selected].Layer[c].Tile = TempTile;
        }

        // Dados do azulejo
        TempTile2 = new Lists.Structures.Map_Tile[Width_New + 1, Height_New + 1];
        for (byte x = 0; x <= Width_New; x++)
            for (byte y = 0; y <= Height_New; y++)
            {
                // Redimensiona para frente
                if (!chkReverse.Checked)
                    if (x <= Lists.Map[Selected].Width && y <= Lists.Map[Selected].Height)
                        TempTile2[x, y] = Lists.Map[Selected].Tile[x, y];
                    else
                    {
                        TempTile2[x, y] = new Lists.Structures.Map_Tile();
                        TempTile2[x, y].Block = new bool[4];
                    }
                // Redimensiona para trás
                else
                {
                    if (x < Width_Difference || y < Height_Difference)
                    {
                        TempTile2[x, y] = new Lists.Structures.Map_Tile();
                        TempTile2[x, y].Block = new bool[4];
                    }
                    else
                        TempTile2[x, y] = Lists.Map[Selected].Tile[x - Width_Difference, y - Height_Difference];
                }
            }

        // Define os dados
        Lists.Map[Selected].Tile = TempTile2;
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Redimensiona os azulejos
        Map_Resize();

        // Salva os valores
        Lists.Map[Selected].Name = txtName.Text;
        Lists.Map[Selected].Width = (byte)numWidth.Value;
        Lists.Map[Selected].Height = (byte)numHeight.Value;
        Lists.Map[Selected].Moral = (byte)Objects.cmbMoral.SelectedIndex;
        Lists.Map[Selected].Music = (byte)Objects.cmbMusic.SelectedIndex;
        Lists.Map[Selected].Panorama = (byte)Objects.numPanorama.Value;
        Lists.Map[Selected].Weather.Type = (byte)Objects.cmbWeather.SelectedIndex;
        Lists.Map[Selected].Weather.Intensity = (byte)Objects.numWeather_Intensity.Value;
        Lists.Map[Selected].Fog.Texture = (byte)Objects.numFog_Texture.Value;
        Lists.Map[Selected].Fog.Speed_X = (sbyte)Objects.numFog_Speed_X.Value;
        Lists.Map[Selected].Fog.Speed_Y = (sbyte)Objects.numFog_Speed_Y.Value;
        Lists.Map[Selected].Fog.Alpha = (byte)Objects.numFog_Transparency.Value;
        Lists.Map[Selected].Color = Color.FromArgb((byte)Objects.numColor_Red.Value, (int)Objects.numColor_Green.Value, (int)Objects.numColor_Blue.Value).ToArgb();
        Lists.Map[Selected].Link[(byte)Globals.Directions.Up] = (short)Objects.numLink_Up.Value;
        Lists.Map[Selected].Link[(byte)Globals.Directions.Down] = (short)Objects.numLink_Down.Value;
        Lists.Map[Selected].Link[(byte)Globals.Directions.Left] = (short)Objects.numLink_Left.Value;
        Lists.Map[Selected].Link[(byte)Globals.Directions.Right] = (short)Objects.numLink_Right.Value;
        Lists.Map[Selected].Light_Global = (byte)Objects.numLight_Global.Value;
        Lists.Map[Selected].Lighting = (byte)Objects.numLighting.Value;

        // Define a nova dimensão dos azulejos
        Editor_Maps.Update_Data();

        // Altera o nome na lista
        Editor_Maps.Objects.cmbList.Items[Selected - 1] = Globals.Numbering(Selected, Editor_Maps.Objects.cmbList.Items.Count) + ":" + txtName.Text;

        // Reseta os valores
        Globals.Weather_Update();
        Editor_Maps.Objects.numLighting.Value = Lists.Map[Selected].Lighting;
        Editor_Maps.Objects.numLight_Global.Value = Lists.Map[Selected].Light_Global;

        // Volta ao editor de mapas
        Visible = false;
        Editor_Maps.Objects.Enabled = true;
        Editor_Maps.Objects.Visible = true;
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Se estiver tocando uma música, para-la
        Audio.Music.Stop();

        // Volta ao editor
        Visible = false;
    }

    private void butMusic_Play_Click(object sender, EventArgs e)
    {
        // Reproduz a música
        Audio.Music.Play((Audio.Musics)cmbMusic.SelectedIndex);
    }

    private void butMusic_Stop_Click(object sender, EventArgs e)
    {
        // Para a música
        Audio.Music.Stop();
    }

    private void butLight_Global_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        numLight_Global.Value = Preview.Select(Graphics.Tex_Light, (short)numLight_Global.Value);
    }

    private void butPanorama_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        numPanorama.Value = Preview.Select(Graphics.Tex_Panorama, (short)numPanorama.Value);
    }

    private void butFog_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        numFog_Texture.Value = Preview.Select(Graphics.Tex_Fog, (short)numFog_Texture.Value);
    }
}