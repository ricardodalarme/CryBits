using System;
using System.Drawing;
using System.Windows.Forms;

public partial class Editor_Maps_Properties : Form
{
    // Usado para acessar os dados da janela
    private static Editor_Maps_Properties Objects = new Editor_Maps_Properties();

    // Dados temporários
    private static Lists.Structures.Map Selected;

    public Editor_Maps_Properties()
    {
        InitializeComponent();
    }

    public static void Open()
    {
        Selected = Editor_Maps.Selected;

        // Limpa as listas
        Objects.cmbMoral.Items.Clear();
        Objects.cmbWeather.Items.Clear();
        Objects.cmbMusic.Items.Clear();
        Objects.cmbMusic.Items.Add("None");
        Objects.cmbLink_Down.Items.Clear();
        Objects.cmbLink_Down.Items.Add("None");
        Objects.cmbLink_Left.Items.Clear();
        Objects.cmbLink_Left.Items.Add("None");
        Objects.cmbLink_Right.Items.Clear();
        Objects.cmbLink_Right.Items.Add("None");
        Objects.cmbLink_Up.Items.Clear();
        Objects.cmbLink_Up.Items.Add("None");

        // Lista os dados
        for (byte i = 0; i < (byte)Globals.Map_Morals.Count; i++) Objects.cmbMoral.Items.Add((Globals.Map_Morals)i);
        for (byte i = 0; i < (byte)Globals.Weathers.Count; i++) Objects.cmbWeather.Items.Add((Globals.Weathers)i);
        for (byte i = 1; i < (byte)Audio.Musics.Amount; i++) Objects.cmbMusic.Items.Add((Audio.Musics)i);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) Objects.cmbLink_Down.Items.Add(Map);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) Objects.cmbLink_Left.Items.Add(Map);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) Objects.cmbLink_Right.Items.Add(Map);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) Objects.cmbLink_Up.Items.Add(Map);

        // Limites
        Objects.numPanorama.Maximum = Graphics.Tex_Panorama.GetUpperBound(0);
        Objects.numFog_Texture.Maximum = Graphics.Tex_Fog.GetUpperBound(0);
        Objects.numWidth.Minimum = Globals.Min_Map_Width;
        Objects.numHeight.Minimum = Globals.Min_Map_Height;
        Objects.numWeather_Intensity.Maximum = Globals.Max_Weather_Intensity;
        Objects.numLight_Global.Maximum = Graphics.Tex_Light.GetUpperBound(0);

        // Define os valores
        Objects.txtName.Text = Selected.Name;
        Objects.numWidth.Value = Selected.Width;
        Objects.numHeight.Value = Selected.Height;
        Objects.cmbMoral.SelectedIndex = Selected.Moral;
        Objects.cmbMusic.SelectedIndex = Selected.Music;
        Objects.numPanorama.Value = Selected.Panorama;
        Objects.numColor_Red.Value = Color.FromArgb(Selected.Color).R;
        Objects.numColor_Green.Value = Color.FromArgb(Selected.Color).G;
        Objects.numColor_Blue.Value = Color.FromArgb(Selected.Color).B;
        Objects.cmbWeather.SelectedIndex = Selected.Weather.Type;
        Objects.numWeather_Intensity.Value = Selected.Weather.Intensity;
        Objects.numFog_Texture.Value = Selected.Fog.Texture;
        Objects.numFog_Speed_X.Value = Selected.Fog.Speed_X;
        Objects.numFog_Speed_Y.Value = Selected.Fog.Speed_Y;
        Objects.numFog_Transparency.Value = Selected.Fog.Alpha;
        Objects.cmbLink_Down.SelectedItem = Selected.Link[(byte)Globals.Directions.Up];
        Objects.cmbLink_Left.SelectedItem = Selected.Link[(byte)Globals.Directions.Down];
        Objects.cmbLink_Right.SelectedItem = Selected.Link[(byte)Globals.Directions.Left];
        Objects.cmbLink_Up.SelectedItem = Selected.Link[(byte)Globals.Directions.Right];
        Objects.numLight_Global.Value = Selected.Light_Global;
        Objects.numLighting.Value = Selected.Lighting;

        // Abre a janela
        Objects.ShowDialog();
    }

    private void Map_Resize()
    {
        byte Width_New = (byte)numWidth.Value, Height_New = (byte)numHeight.Value;
        int Width_Difference, Height_Difference;

        // Somente se necessário
        if (Selected.Width == Width_New && Selected.Height == Height_New) return;

        // Redimensiona os azulejos
        Lists.Structures.Map_Tile_Data[,] TempTile;
        Lists.Structures.Map_Tile[,] TempTile2;

        // Calcula a diferença
        Width_Difference = Width_New - Selected.Width;
        Height_Difference = Height_New - Selected.Height;

        // Azulejo1
        for (byte c = 0; c < Selected.Layer.Count; c++)
        {
            TempTile = new Lists.Structures.Map_Tile_Data[Width_New + 1, Height_New + 1];

            for (byte x = 0; x <= Width_New; x++)
                for (byte y = 0; y <= Height_New; y++)
                {
                    // Redimensiona para frente
                    if (!chkReverse.Checked)
                        if (x <= Selected.Width && y <= Selected.Height)
                            TempTile[x, y] = Selected.Layer[c].Tile[x, y];
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
                            TempTile[x, y] = Selected.Layer[c].Tile[x - Width_Difference, y - Height_Difference];
                    }
                }

            // Define os dados
            Selected.Layer[c].Tile = TempTile;
        }

        // Dados do azulejo
        TempTile2 = new Lists.Structures.Map_Tile[Width_New + 1, Height_New + 1];
        for (byte x = 0; x <= Width_New; x++)
            for (byte y = 0; y <= Height_New; y++)
            {
                // Redimensiona para frente
                if (!chkReverse.Checked)
                    if (x <= Selected.Width && y <= Selected.Height)
                        TempTile2[x, y] = Selected.Tile[x, y];
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
                        TempTile2[x, y] = Selected.Tile[x - Width_Difference, y - Height_Difference];
                }
            }

        // Define os dados
        Selected.Tile = TempTile2;
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Redimensiona os azulejos
        Map_Resize();

        // Salva os valores
        Selected.Name = txtName.Text;
        Selected.Width = (byte)numWidth.Value;
        Selected.Height = (byte)numHeight.Value;
        Selected.Moral = (byte)Objects.cmbMoral.SelectedIndex;
        Selected.Music = (byte)Objects.cmbMusic.SelectedIndex;
        Selected.Panorama = (byte)Objects.numPanorama.Value;
        Selected.Weather.Type = (byte)Objects.cmbWeather.SelectedIndex;
        Selected.Weather.Intensity = (byte)Objects.numWeather_Intensity.Value;
        Selected.Fog.Texture = (byte)Objects.numFog_Texture.Value;
        Selected.Fog.Speed_X = (sbyte)Objects.numFog_Speed_X.Value;
        Selected.Fog.Speed_Y = (sbyte)Objects.numFog_Speed_Y.Value;
        Selected.Fog.Alpha = (byte)Objects.numFog_Transparency.Value;
        Selected.Color = Color.FromArgb((byte)Objects.numColor_Red.Value, (int)Objects.numColor_Green.Value, (int)Objects.numColor_Blue.Value).ToArgb();
        Selected.Link[(byte)Globals.Directions.Up] = (Lists.Structures.Map)Objects.cmbLink_Down.SelectedItem;
        Selected.Link[(byte)Globals.Directions.Down] = (Lists.Structures.Map)Objects.cmbLink_Left.SelectedItem;
        Selected.Link[(byte)Globals.Directions.Left] = (Lists.Structures.Map)Objects.cmbLink_Right.SelectedItem;
        Selected.Link[(byte)Globals.Directions.Right] = (Lists.Structures.Map)Objects.cmbLink_Up.SelectedItem;
        Selected.Light_Global = (byte)Objects.numLight_Global.Value;
        Selected.Lighting = (byte)Objects.numLighting.Value;

        // Define a nova dimensão dos azulejos
        Editor_Maps.Update_Data();

        // Altera o nome na lista
        Editor_Maps.Objects.List.SelectedNode.Text = txtName.Text;

        // Reseta os valores
        Globals.Weather_Update();
        Editor_Maps.Objects.numLighting.Value = Selected.Lighting;
        Editor_Maps.Objects.numLight_Global.Value = Selected.Light_Global;

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