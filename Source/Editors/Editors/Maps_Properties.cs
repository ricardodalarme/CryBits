using System;
using System.Drawing;
using System.Windows.Forms;

public partial class Editor_Maps_Properties : Form
{
    // Dados temporários
    private Lists.Structures.Map Selected;

    public Editor_Maps_Properties()
    {
        // Inicializa os componentes
        InitializeComponent();
        Selected = Editor_Maps.Form.Selected;

        // Limpa as listas
        cmbMusic.Items.Add("None");
        cmbLink_Down.Items.Add("None");
        cmbLink_Left.Items.Add("None");
        cmbLink_Right.Items.Add("None");
        cmbLink_Up.Items.Add("None");

        // Lista os dados
        for (byte i = 0; i < (byte)Globals.Map_Morals.Count; i++) cmbMoral.Items.Add((Globals.Map_Morals)i);
        for (byte i = 0; i < (byte)Globals.Weathers.Count; i++) cmbWeather.Items.Add((Globals.Weathers)i);
        for (byte i = 1; i < (byte)Audio.Musics.Amount; i++) cmbMusic.Items.Add((Audio.Musics)i);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) cmbLink_Down.Items.Add(Map);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) cmbLink_Left.Items.Add(Map);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) cmbLink_Right.Items.Add(Map);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) cmbLink_Up.Items.Add(Map);

        // Limites
        numPanorama.Maximum = Graphics.Tex_Panorama.GetUpperBound(0);
        numFog_Texture.Maximum = Graphics.Tex_Fog.GetUpperBound(0);
        numWidth.Minimum = Globals.Min_Map_Width;
        numHeight.Minimum = Globals.Min_Map_Height;
        numWeather_Intensity.Maximum = Globals.Max_Weather_Intensity;

        // Define os valores
        txtName.Text = Selected.Name;
        numWidth.Value = Selected.Width;
        numHeight.Value = Selected.Height;
        cmbMoral.SelectedIndex = Selected.Moral;
        cmbMusic.SelectedIndex = Selected.Music;
        numPanorama.Value = Selected.Panorama;
        numColor_Red.Value = Color.FromArgb(Selected.Color).R;
        numColor_Green.Value = Color.FromArgb(Selected.Color).G;
        numColor_Blue.Value = Color.FromArgb(Selected.Color).B;
        cmbWeather.SelectedIndex = Selected.Weather.Type;
        numWeather_Intensity.Value = Selected.Weather.Intensity;
        numFog_Texture.Value = Selected.Fog.Texture;
        numFog_Speed_X.Value = Selected.Fog.Speed_X;
        numFog_Speed_Y.Value = Selected.Fog.Speed_Y;
        numFog_Transparency.Value = Selected.Fog.Alpha;
        cmbLink_Down.SelectedItem = Selected.Link[(byte)Globals.Directions.Up];
        cmbLink_Left.SelectedItem = Selected.Link[(byte)Globals.Directions.Down];
        cmbLink_Right.SelectedItem = Selected.Link[(byte)Globals.Directions.Left];
        cmbLink_Up.SelectedItem = Selected.Link[(byte)Globals.Directions.Right];

        // Abre a janela
        ShowDialog();
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

        // Azulejo
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
        Selected.Moral = (byte)cmbMoral.SelectedIndex;
        Selected.Music = (byte)cmbMusic.SelectedIndex;
        Selected.Panorama = (byte)numPanorama.Value;
        Selected.Weather.Type = (byte)cmbWeather.SelectedIndex;
        Selected.Weather.Intensity = (byte)numWeather_Intensity.Value;
        Selected.Fog.Texture = (byte)numFog_Texture.Value;
        Selected.Fog.Speed_X = (sbyte)numFog_Speed_X.Value;
        Selected.Fog.Speed_Y = (sbyte)numFog_Speed_Y.Value;
        Selected.Fog.Alpha = (byte)numFog_Transparency.Value;
        Selected.Color = Color.FromArgb((byte)numColor_Red.Value, (int)numColor_Green.Value, (int)numColor_Blue.Value).ToArgb();
        Selected.Link[(byte)Globals.Directions.Up] = (Lists.Structures.Map)cmbLink_Down.SelectedItem;
        Selected.Link[(byte)Globals.Directions.Down] = (Lists.Structures.Map)cmbLink_Left.SelectedItem;
        Selected.Link[(byte)Globals.Directions.Left] = (Lists.Structures.Map)cmbLink_Right.SelectedItem;
        Selected.Link[(byte)Globals.Directions.Right] = (Lists.Structures.Map)cmbLink_Up.SelectedItem;

        // Altera o nome na lista
        Editor_Maps.Form.List.SelectedNode.Text = txtName.Text;

        // Reseta os valores
        Globals.Weather_Update();
        Editor_Maps.Form.Update_Map_Bounds();

        // Volta ao editor de mapas
        Visible = false;
        Editor_Maps.Form.Enabled = true;
        Editor_Maps.Form.Visible = true;
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