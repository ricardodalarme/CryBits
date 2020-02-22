using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

partial class Editor_Maps : Form
{
    #region Data
    // Usado para acessar os dados da janela
    public static Editor_Maps Objects = new Editor_Maps();

    // Index do item selecionado
    public static short Selected;

    // Dados temporários
    private static bool Map_Pressed;

    // Posição do mouse
    public static Point Tile_Mouse;
    private static Point Map_Mouse;

    // Seleção retângular
    private static Rectangle Def_Tiles_Selection = new Rectangle(0, 0, 1, 1);
    private static Rectangle Def_Map_Selection = new Rectangle(0, 0, 1, 1);

    // Dados dos atributos
    private static short AData_1;
    private static short AData_2;
    private static short AData_3;
    private static short AData_4;

    // Azulejos copiados
    private static Copy_Struct Tiles_Copy = new Copy_Struct();

    public struct Copy_Struct
    {
        public Rectangle Area;
        public Lists.Structures.Map_Layer[] Data;

    }
    #endregion

    #region Base
    public Editor_Maps()
    {
        InitializeComponent();
    }

    private void Editor_Maps_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Previne erros
        if (!Visible) return;

        // Volta ao menu
        e.Cancel = true;
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void Editor_Maps_SizeChanged(object sender, EventArgs e)
    {
        // Atualiza os limites
        Update_Map_Bounds();
        Update_Tile_Bounds();
    }

    public static void Request()
    {
        // Lê os dados
        Globals.OpenEditor = Objects;
        Send.Request_NPCs();
        Send.Request_Tiles();
        Send.Request_Items();
        Send.Request_Maps(true);
    }

    public static void Open()
    {
        // Lista os itens
        Update_List();
        Update_List_Layers();

        // Limpa as listas
        Objects.cmbLayers_Type.Items.Clear();
        Objects.cmbTiles.Items.Clear();
        Objects.cmbNPC.Items.Clear();
        Objects.cmbA_Item.Items.Clear();

        // Lista os itens
        for (byte i = 0; i < (byte)Globals.Layers.Count; i++) Objects.cmbLayers_Type.Items.Add(((Globals.Layers)i).ToString());
        for (byte i = 1; i < Graphics.Tex_Tile.Length; i++) Objects.cmbTiles.Items.Add(i.ToString());
        for (byte i = 1; i < Lists.NPC.Length; i++) Objects.cmbNPC.Items.Add(Lists.NPC[i].Name);
        for (byte i = 1; i < Lists.Item.Length; i++) Objects.cmbA_Item.Items.Add(Lists.Item[i].Name);

        // Reseta os valores
        Objects.cmbA_Warp_Direction.SelectedIndex = 0;
        Objects.cmbNPC.SelectedIndex = 0;
        Objects.cmbA_Item.SelectedIndex = 0;
        Objects.cmbTiles.SelectedIndex = 0;
        Objects.cmbLayers_Type.SelectedIndex = 0;
        Objects.picTile.BringToFront();
        Objects.grpZones.BringToFront();
        Objects.grpNPCs.BringToFront();
        Objects.grpAttributes.BringToFront();
        Objects.grpLighting.BringToFront();
        Objects.butMNormal.Checked = true;
        Objects.butMZones.Checked = false;
        Objects.butMNPCs.Checked = false;
        Objects.butMAttributes.Checked = false;
        Objects.butMLighting.Checked = false;
        Objects.numLighting.Value = Lists.Map[Selected].Lighting;
        Objects.numLight_Global.Maximum = Graphics.Tex_Light.GetUpperBound(0);
        Objects.numLight_Global.Value = Lists.Map[Selected].Light_Global;

        // Define os limites
        Objects.scrlZone.Maximum = Globals.Num_Zones;
        Objects.numNPC_Zone.Maximum = Globals.Num_Zones;
        Update_Tile_Bounds();
        Update_Data();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void Update_List()
    {
        // Limpa as listas
        Objects.cmbList.Items.Clear();
        Objects.cmbA_Warp_Map.Items.Clear();

        // Adiciona os itens às listas
        for (byte i = 1; i < Lists.Map.Length; i++)
        {
            Objects.cmbList.Items.Add(Globals.Numbering(i, Lists.Map.GetUpperBound(0)) + ":" + Lists.Map[i].Name);
            Objects.cmbA_Warp_Map.Items.Add(Globals.Numbering(i, Lists.Map.GetUpperBound(0)) + ":" + Lists.Map[i].Name);
        }

        // Seleciona o primeiro item
        Objects.cmbList.SelectedIndex = 0;
        Objects.cmbA_Warp_Map.SelectedIndex = 0;
    }

    public static void Update_Data()
    {
        Selected = (short)(Objects.cmbList.SelectedIndex + 1);

        // Reseta o clima
        Globals.Weather_Update();

        // Faz os cálculos da autocriação
        Autotile.Update(Selected);

        // Atualiza os dados
        Update_Map_Bounds();
        Update_List_Layers();
        Update_NPCs();
    }

    private void Update_Tile_Selected()
    {
        // Altera o tamanho do azulejo selecionado
        switch (chkAuto.Checked)
        {
            case false: Def_Tiles_Selection.Size = new Size(1, 1); break;
            case true: Def_Tiles_Selection.Size = new Size(2, 3); break;
        }
    }

    public static void Change_Quantity()
    {
        short Quantity = (short)Editor_Quantity.Objects.numQuantity.Value;
        short Old = (short)Lists.Map.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.Map, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (short i = (short)(Old + 1); i <= Quantity; i++)
                Clear.Map(i);

        Update_List();
    }

    private static void Update_Strip()
    {
        // Atualiza as informações da barra
        Objects.Strip.Items[0].Text = "FPS: " + Globals.FPS;
        Objects.Strip.Items[2].Text = "Revision: " + Lists.Map[Selected].Revision;
        Objects.Strip.Items[4].Text = "Position: {" + Map_Mouse.X + ";" + Map_Mouse.Y + "}"; ;
    }

    private void cmbList_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Update_Data();
    }

    private static void Update_Tile_Bounds()
    {
        Size Tile_Size = Graphics.TSize(Graphics.Tex_Tile[Objects.cmbTiles.SelectedIndex + 1]);
        int Width = Tile_Size.Width - Objects.picTile.Width;
        int Height = Tile_Size.Height - Objects.picTile.Height;

        // Verifica se nada passou do limite minímo
        if (Width < 0) Width = 0;
        if (Height < 0) Height = 0;
        if (Width > 0) Width += Globals.Grid;
        if (Height > 0) Height += Globals.Grid;

        // Define os limites
        Objects.scrlTileX.Maximum = Width;
        Objects.scrlTileY.Maximum = Height;
    }

    private static void Update_Map_Bounds()
    {
        int Max_X = (Lists.Map[Selected].Width / Zoom() * Globals.Grid - Objects.picMap.Width) / Globals.Grid + 1;
        int Max_Y = (Lists.Map[Selected].Height / Zoom() * Globals.Grid - Objects.picMap.Height) / Globals.Grid + 1;

        // Valor máximo
        if (Max_X > 0) Objects.scrlMapX.Maximum = Max_X; else Objects.scrlMapX.Maximum = 0;
        if (Max_Y > 0) Objects.scrlMapY.Maximum = Max_Y; else Objects.scrlMapY.Maximum = 0;

        // Reseta os valores
        Objects.scrlMapX.Value = 0;
        Objects.scrlMapY.Value = 0;
    }
    #endregion

    #region Components
    private void butProperties_Click(object sender, EventArgs e)
    {
        // Para os áudios
        Audio.Sound.Stop_All();
        Audio.Music.Stop();

        // Abre as propriedades
        Editor_Maps_Properties.Open();
    }

    private void butQuantity_Click(object sender, EventArgs e)
    {
        // Abre a janela de alteração
        Editor_Quantity.Open(Lists.Map.GetUpperBound(0));
    }

    private void chkAuto_CheckedChanged(object sender, EventArgs e)
    {
        Update_Tile_Selected();
    }

    private void scrlZone_Clear_Click(object sender, EventArgs e)
    {
        // Reseta todas as zonas
        for (byte x = 0; x <= Lists.Map[Selected].Width; x++)
            for (byte y = 0; x <= Lists.Map[Selected].Height; y++)
                Lists.Map[Selected].Tile[x, y].Zone = 0;
    }

    private void butLight_Clear_Click(object sender, EventArgs e)
    {
        // Reseta todas as zonas
        Lists.Map[Selected].Light = new List<Lists.Structures.Map_Light>();
    }

    private void scrlZone_ValueChanged(object sender, EventArgs e)
    {
        // Atualiza os valores
        if (scrlZone.Value == 0)
            grpZones.Text = "Zone: Null";
        else
            grpZones.Text = "Zone: " + scrlZone.Value;
    }

    private void butAttributes_Clear_Click(object sender, EventArgs e)
    {
        // Reseta todas os atributos
        for (byte x = 0; x <= Lists.Map[Selected].Width; x++)
            for (byte y = 0; y <= Lists.Map[Selected].Height; y++)
            {
                Lists.Map[Selected].Tile[x, y].Attribute = 0;
                Lists.Map[Selected].Tile[x, y].Block = new bool[(byte)Globals.Directions.Count];
            }
    }

    private void butAttributes_Import_Click(object sender, EventArgs e)
    {
        // Importa os dados padrões dos azulejos
        for (byte x = 0; x <= Lists.Map[Selected].Width; x++)
            for (byte y = 0; y <= Lists.Map[Selected].Height; y++)
                for (byte c = 0; c < Lists.Map[Selected].Layer.Count; c++)
                {
                    // Dados do azulejo
                    Lists.Structures.Map_Tile_Data Data = Lists.Map[Selected].Layer[c].Tile[x, y];

                    if (Data.Tile > 0)
                    {
                        // Atributos
                        if (Lists.Tile[Data.Tile].Data[Data.X, Data.Y].Attribute > 0)
                            Lists.Map[Selected].Tile[x, y].Attribute = Lists.Tile[Data.Tile].Data[Data.X, Data.Y].Attribute;

                        // Bloqueio direcional
                        for (byte b = 0; b < (byte)Globals.Directions.Count; b++)
                            if (Lists.Tile[Data.Tile].Data[Data.X, Data.Y].Block[b])
                                Lists.Map[Selected].Tile[x, y].Block[b] = Lists.Tile[Data.Tile].Data[Data.X, Data.Y].Block[b];
                    }
                }
    }

    private void numLighting_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Map[Selected].Lighting = (byte)numLighting.Value;
    }

    private void numLight_Global_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Map[Selected].Light_Global = (byte)numLight_Global.Value;
    }

    private void butLight_Global_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        numLight_Global.Value = Preview.Select(Graphics.Tex_Light, (short)numLight_Global.Value);
    }
    #endregion

    #region Toolbox
    private void butSaveAll_Click(object sender, EventArgs e)
    {
        // Salva todos os dados
        Send.Write_Maps();
        MessageBox.Show("All maps have been saved");
    }

    private void butReload_Click(object sender, EventArgs e)
    {
        // Recarrega o mapa
        Send.Request_Map(Selected);
        Update_List_Layers();
        Autotile.Update(Selected);
    }

    private void butClearAllr_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Clear.Map(Selected);

        // Atualiza os valores
        cmbList.Items[Selected - 1] = Globals.Numbering(Selected, cmbList.Items.Count) + ":";
    }

    private void Copiar()
    {
        Tiles_Copy.Data = new Lists.Structures.Map_Layer[Lists.Map[Selected].Layer.Count];

        // Seleção
        Tiles_Copy.Area = Map_Selection;

        // Copia os dados das camadas
        for (byte c = 0; c < Tiles_Copy.Data.Length; c++)
        {
            Tiles_Copy.Data[c] = new Lists.Structures.Map_Layer();
            Tiles_Copy.Data[c].Name = Lists.Map[Selected].Layer[c].Name;

            // Tamanho da estrutura
            Tiles_Copy.Data[c].Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[Selected].Width + 1, Lists.Map[Selected].Height + 1];

            // Copia os dados dos azulejos
            for (byte x = 0; x <= Lists.Map[Selected].Width; x++)
                for (byte y = 0; y <= Lists.Map[Selected].Height; y++)
                    Tiles_Copy.Data[c].Tile[x, y] = Lists.Map[Selected].Layer[c].Tile[x, y];
        }
    }

    private void butCopy_Click(object sender, EventArgs e)
    {
        // Copia os dados
        Copiar();
    }

    private void butCut_Click(object sender, EventArgs e)
    {
        // Copia os dados
        Copiar();

        // Remove os azulejos copiados
        for (int x = Map_Selection.X; x < Map_Selection.X + Map_Selection.Width; x++)
            for (int y = Map_Selection.Y; y < Map_Selection.Y + Map_Selection.Height; y++)
                for (byte c = 0; c < Lists.Map[Selected].Layer.Count; c++)
                {
                    Lists.Map[Selected].Layer[c].Tile[x, y] = new Lists.Structures.Map_Tile_Data();
                    Lists.Map[Selected].Layer[c].Tile[x, y].Mini = new Point[4];
                }

        // Atualiza os azulejos Autos 
        Autotile.Update(Selected);
    }

    private void butPaste_Click(object sender, EventArgs e)
    {
        // Cola os azulejos
        for (int x = Tiles_Copy.Area.X; x < Tiles_Copy.Area.X + Tiles_Copy.Area.Width; x++)
            for (int y = Tiles_Copy.Area.Y; y < Tiles_Copy.Area.Y + Tiles_Copy.Area.Height; y++)
                for (byte c = 0; c < Tiles_Copy.Data.Length; c++)
                {
                    // Dados
                    int Layer = Find_Layer(Tiles_Copy.Data[c].Name);
                    int x2 = Map_Selection.X + x - Tiles_Copy.Area.X;
                    int y2 = y + Map_Selection.Y - Tiles_Copy.Area.Y;

                    // Previne erros
                    if (Layer < 0) continue;
                    if (x2 > Lists.Map[Selected].Width) continue;
                    if (y2 > Lists.Map[Selected].Height) continue;

                    // Cola
                    Lists.Map[Selected].Layer[Layer].Tile[x2, y2] = Tiles_Copy.Data[c].Tile[x, y];
                }

        // Atualiza os azulejos Autos 
        Autotile.Update(Selected);
    }

    private void butPencil_Click(object sender, EventArgs e)
    {
        // Reseta as outras ferramentas e escolhe essa
        if (butPencil.Checked)
        {
            butRectangle.Checked = false;
            butArea.Checked = false;
            butDiscover.Checked = false;
        }
        else
            butPencil.Checked = true;

        // Reseta o tamanho da seleção
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void butRectangle_Click(object sender, EventArgs e)
    {
        // Reseta as outras ferramentas e escolhe essa
        if (butRectangle.Checked)
        {
            butPencil.Checked = false;
            butArea.Checked = false;
            butDiscover.Checked = false;
        }
        else
            butRectangle.Checked = true;

        // Reseta o tamanho da seleção
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void butArea_Click(object sender, EventArgs e)
    {
        // Reseta as outras ferramentas e escolhe essa
        if (butArea.Checked)
        {
            butRectangle.Checked = false;
            butPencil.Checked = false;
            butDiscover.Checked = false;
        }
        else
            butArea.Checked = true;

        // Reseta o tamanho da seleção
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void butDiscover_Click(object sender, EventArgs e)
    {
        // Reseta as outras ferramentas e escolhe essa
        if (butDiscover.Checked)
        {
            butRectangle.Checked = false;
            butArea.Checked = false;
            butPencil.Checked = false;
        }
        else
            butDiscover.Checked = true;

        // Reseta o tamanho da seleção
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void butFill_Click(object sender, EventArgs e)
    {
        // Somente se necessário
        if (lstLayers.SelectedItems.Count == 0) return;

        // Preenche todos os azulejos iguais ao selecionado com o mesmo azulejo
        for (int x = 0; x <= Lists.Map[Selected].Width; x++)
            for (int y = 0; y <= Lists.Map[Selected].Height; y++)
                Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Tile[x, y] = Set_Tile();

        // Faz os cálculos da autocriação
        Autotile.Update(Selected);
    }

    private void butEraser_Click(object sender, EventArgs e)
    {
        // Somente se necessário
        if (lstLayers.SelectedItems.Count == 0) return;

        // Preenche todos os azulejos iguais ao selecionado com o mesmo azulejo
        for (int x = 0; x <= Lists.Map[Selected].Width; x++)
            for (int y = 0; y <= Lists.Map[Selected].Height; y++)
            {
                Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Tile[x, y] = new Lists.Structures.Map_Tile_Data();
                Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Tile[x, y].Mini = new Point[4];
            }
    }

    private void butEdition_Click(object sender, EventArgs e)
    {
        // Reseta as outras ferramentas e escolhe essa
        if (butEdition.Checked)
            butVisualization.Checked = false;
        else
            butEdition.Checked = true;

        // Salva a preferência
        Lists.Options.Pre_Map_View = butVisualization.Checked;
        Write.Options();
    }

    private void butVisualization_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butVisualization.Checked)
            butEdition.Checked = false;
        else
            butVisualization.Checked = true;

        // Salva a preferência
        Lists.Options.Pre_Map_View = butVisualization.Checked;
        Write.Options();
    }

    private void butGrids_Click(object sender, EventArgs e)
    {
        // Salva a preferência
        Lists.Options.Pre_Map_Grid = butGrid.Checked;
        Write.Options();
    }

    private void butAudio_Click(object sender, EventArgs e)
    {
        // Salva a preferência
        Lists.Options.Pre_Map_Audio = butAudio.Checked;
        Write.Options();

        // Desativa os áudios
        if (!butAudio.Checked)
        {
            Audio.Music.Stop();
            Audio.Sound.Stop_All();
        }
    }

    private void butZoom_Normal_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butZoom_Normal.Checked)
        {
            butZoom_2x.Checked = false;
            butZoom_4x.Checked = false;
        }
        else
            butZoom_Normal.Checked = true;

        // Atualiza os limites
        Objects.picMap.Image = null;
        Update_Map_Bounds();
    }

    private void butZoom_2x_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butZoom_2x.Checked)
        {
            butZoom_Normal.Checked = false;
            butZoom_4x.Checked = false;
        }
        else
            butZoom_2x.Checked = true;

        // Atualiza os limites
        Objects.picMap.Image = null;
        Update_Map_Bounds();
    }

    private void butZoom_4x_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butZoom_4x.Checked)
        {
            butZoom_Normal.Checked = false;
            butZoom_2x.Checked = false;
        }
        else
            butZoom_4x.Checked = true;

        // Atualiza os limites
        Objects.picMap.Image = null;
        Update_Map_Bounds();
    }

    private void butMNormal_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butMNormal.Checked)
        {
            butMZones.Checked = false;
            butMLighting.Checked = false;
            butMAttributes.Checked = false;
            butMNPCs.Checked = false;
        }
        else
            butMNormal.Checked = true;
    }

    private void butMAttributes_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butMAttributes.Checked)
        {
            butMNormal.Checked = false;
            butMLighting.Checked = false;
            butMZones.Checked = false;
            butMNPCs.Checked = false;
        }
        else
            butMAttributes.Checked = true;
    }

    private void butMZones_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butMZones.Checked)
        {
            butMNormal.Checked = false;
            butMLighting.Checked = false;
            butMAttributes.Checked = false;
            butMNPCs.Checked = false;
        }
        else
            butMZones.Checked = true;
    }

    private void butMLighting_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butMLighting.Checked)
        {
            butMZones.Checked = false;
            butMNormal.Checked = false;
            butMAttributes.Checked = false;
            butMNPCs.Checked = false;
        }
        else
            butMLighting.Checked = true;
    }

    private void butMNPCs_Click(object sender, EventArgs e)
    {
        // Reseta a marcação
        if (butMNPCs.Checked)
        {
            butMZones.Checked = false;
            butMNormal.Checked = false;
            butMAttributes.Checked = false;
            butMLighting.Checked = false;
        }
        else
            butMNPCs.Checked = true;
    }

    private void butMNormal_CheckedChanged(object sender, EventArgs e)
    {
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void butMLighting_CheckedChanged(object sender, EventArgs e)
    {
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void butMZones_CheckedChanged(object sender, EventArgs e)
    {
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void tmrUpdate_Tick(object sender, EventArgs e)
    {
        // Apenas se necessário
        if (!Objects.Visible) return;

        // Ferramentas em geral
        if (butMNormal.Checked)
        {
            grpZones.Visible = false;
            grpLighting.Visible = false;
            grpAttributes.Visible = false;
            grpAttributes_Set.Visible = false;
            grpNPCs.Visible = false;
            butPencil.Enabled = true;
            butRectangle.Enabled = true;
            butArea.Enabled = true;
            butDiscover.Enabled = true;
            butEraser.Enabled = true;
            butFill.Enabled = true;
            butGrid.Enabled = true;
            butEdition.Enabled = true;
            butVisualization.Enabled = true;
        }
        else
        {
            butPencil.Enabled = false;
            butRectangle.Enabled = false;
            butArea.Enabled = false;
            butDiscover.Enabled = false;
            butEraser.Enabled = false;
            butFill.Enabled = false;
            butGrid.Enabled = false;
            butEdition.Checked = true;
            butVisualization.Checked = false;
            butEdition.Enabled = false;
            butVisualization.Enabled = false;
        }

        // Grupos
        // Iluminação
        if (butMLighting.Checked)
        {
            grpLighting.Visible = true;
            grpZones.Visible = false;
            grpAttributes.Visible = false;
            grpAttributes_Set.Visible = false;
            grpNPCs.Visible = false;
        }
        // Zonas
        if (butMZones.Checked)
        {
            grpZones.Visible = true;
            grpLighting.Visible = false;
            grpAttributes.Visible = false;
            grpAttributes_Set.Visible = false;
            grpNPCs.Visible = false;
        }
        // Atributos
        if (butMAttributes.Checked)
        {
            grpAttributes.Visible = true;
            grpZones.Visible = false;
            grpLighting.Visible = false;
            grpNPCs.Visible = false;
        }
        // NPCs
        if (butMNPCs.Checked)
        {
            grpNPCs.Visible = true;
            grpZones.Visible = false;
            grpAttributes.Visible = false;
            grpAttributes_Set.Visible = false;
            grpLighting.Visible = false;
        }

        // Ferramentas de recorte e colagem
        if (!butMNormal.Checked || !butMNormal.Enabled || !butArea.Checked || !butArea.Enabled)
        {
            butPaste.Enabled = false;
            butCopy.Enabled = false;
            butCut.Enabled = false;
        }
        else
        {
            butPaste.Enabled = true;
            butCopy.Enabled = true;
            butCut.Enabled = true;
        }
        // Sem cópias
        if (Tiles_Copy.Data == null) butPaste.Enabled = false;

        // Atualiza os dados da faixa
        Update_Strip();
    }
    #endregion

    #region picTile
    private void picTile_MouseWheel(object sender, MouseEventArgs e)
    {
        // Previne erros
        if (Objects.picTile_Background.Size != picTile.Size) return;
        if (scrlTileY.Maximum <= 1) return;

        // Movimenta para baixo
        if (e.Delta > 0)
            if (scrlTileY.Value - Globals.Grid > 0)
                scrlTileY.Value -= Globals.Grid;
            else
                scrlTileY.Value = 0;
        // Movimenta para cima
        else if (e.Delta < 0)
            if (scrlTileY.Value < scrlTileY.Maximum - Globals.Grid)
                scrlTileY.Value += Globals.Grid;
            else
                scrlTileY.Value = scrlTileY.Maximum - Globals.Grid;
    }

    private void picTile_MouseDown(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // Previne erros
            if (e.X + scrlTileX.Value > Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]).Width) return;
            if (e.Y + scrlTileY.Value > Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]).Height) return;

            // Seleciona o azulejo;
            Def_Tiles_Selection.Location = new Point((e.X + scrlTileX.Value) / Globals.Grid, (e.Y + scrlTileY.Value) / Globals.Grid);
            Update_Tile_Selected();
        }
    }

    private void picTile_MouseMove(object sender, MouseEventArgs e)
    {
        int x = (e.X + scrlTileX.Value) / Globals.Grid;
        int y = (e.Y + scrlTileY.Value) / Globals.Grid;
        Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]);

        // Define os valores
        Tile_Mouse = new Point(x * Globals.Grid - scrlTileX.Value, y * Globals.Grid - scrlTileY.Value);

        // Somente se necessário
        if (e.Button != MouseButtons.Left) return;
        if (chkAuto.Checked) return;

        // Verifica se não passou do limite
        if (x < 0) x = 0;
        if (x > Texture_Size.Width / Globals.Grid - 1) x = Texture_Size.Width / Globals.Grid - 1;
        if (y < 0) y = 0;
        if (y > Texture_Size.Height / Globals.Grid - 1) y = Texture_Size.Height / Globals.Grid - 1;

        // Tamanho da grade
        Def_Tiles_Selection.Width = x - Def_Tiles_Selection.X + 1;
        Def_Tiles_Selection.Height = y - Def_Tiles_Selection.Y + 1;

        // Altera o tamanho da tela de azulejos
        if (picTile.Width < Texture_Size.Width - scrlTileX.Value) picTile.Width = Texture_Size.Width - scrlTileX.Value;
        if (picTile.Height < Texture_Size.Height - scrlTileY.Value) picTile.Height = Texture_Size.Height - scrlTileY.Value;
    }

    private void picTile_MouseLeave(object sender, EventArgs e)
    {
        // Reseta o tamanho da tela
        picTile.Size = Objects.picTile_Background.Size;
        Update_Tile_Bounds();
    }

    private void picTile_MouseUp(object sender, MouseEventArgs e)
    {
        // Reseta o tamanho da tela
        picTile.Size = Objects.picTile_Background.Size;
        Update_Tile_Bounds();
    }

    private void cmbTiles_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Reseta alguns valores
        scrlTileX.Value = 0;
        scrlTileY.Value = 0;
        chkAuto.Checked = false;
        Update_Tile_Bounds();

        // Verifica se a seleção de azulejos passou do limite
        Tile_Mouse = new Point(0);
        Def_Tiles_Selection = new Rectangle(0, 0, 1, 1);
        Def_Map_Selection.Size = new Size(1, 1);
    }
    #endregion

    #region picMap
    private void picMap_SizeChanged(object sender, EventArgs e)
    {
        // Recria as janelas de acordo com o novo tamanho
        Graphics.Win_Map.Dispose();
        Graphics.Win_Map = new SFML.Graphics.RenderWindow(picMap.Handle);
        Graphics.Win_Map_Lighting.Dispose();
        Graphics.Win_Map_Lighting = new SFML.Graphics.RenderTexture((uint)picMap.Width, (uint)picMap.Height);
    }

    private void picMap_MouseWheel(object sender, MouseEventArgs e)
    {
        // Movimenta para baixo
        if (e.Delta > 0)
            if (scrlMapY.Value - 1 > 0)
                scrlMapY.Value -= 1;
            else
                scrlMapY.Value = 0;
        // Movimenta para cima
        else if (e.Delta < 0)
            if (scrlMapY.Value + 1 < scrlMapY.Maximum)
                scrlMapY.Value += 1;
            else
                scrlMapY.Value = scrlMapY.Maximum;
    }

    private void picMap_MouseDown(object sender, MouseEventArgs e)
    {
        Point Tile_Dif = new Point(e.X - e.X / Globals.Grid * Globals.Grid, e.Y - e.Y / Globals.Grid * Globals.Grid);

        // Previne erros
        if (Selected <= 0) return;
        if (Map_Selection.X > Lists.Map[Selected].Width || Map_Selection.Y > Lists.Map[Selected].Height) return;

        // Executa um evento de acordo com a ferramenta selecionada
        if (butMNormal.Checked)
        {
            Tile_Events(e.Button);

            // Ferramentas
            if (butArea.Checked) Def_Map_Selection = new Rectangle(Map_Mouse, new Size(1, 1));
        }
        else if (butMAttributes.Checked && optA_DirBlock.Checked)
        {
            // Define o bloqueio direcional
            for (byte i = 0; i < (byte)Globals.Directions.Count; i++)
                if (Tile_Dif.X >= Globals.Block_Position(i).X && Tile_Dif.X <= Globals.Block_Position(i).X + 8)
                    if (Tile_Dif.Y >= Globals.Block_Position(i).Y && Tile_Dif.Y <= Globals.Block_Position(i).Y + 8)
                        // Altera o valor de bloqueio
                        Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Block[i] = !Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Block[i];
        }
        else if (butMAttributes.Checked && !optA_DirBlock.Checked)
            Set_Attribute(e);
        else if (butMZones.Checked)
        {
            // Define as zonas
            if (e.Button == MouseButtons.Left)
                Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Zone = (byte)scrlZone.Value;
            else if (e.Button == MouseButtons.Right)
                Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Zone = 0;
        }
        else if (butMLighting.Checked)
        {
            // Remove as luzes
            if (e.Button == MouseButtons.Right)
                Lighting_Remove();
        }
        else if (butMNPCs.Checked)
            // Adiciona o NPC
            if (e.Button == MouseButtons.Left)
                AddNPC(true, (byte)Map_Selection.X, (byte)Map_Selection.Y);
    }

    private static void Lighting_Remove()
    {
        // Encontra a luz que está nessa camada
        if (Lists.Map[Selected].Light.Count > 0)
            for (byte i = 0; i < Lists.Map[Selected].Light.Count; i++)
                if (Lists.Map[Selected].Light[i].X == Map_Selection.X)
                    if (Lists.Map[Selected].Light[i].Y == Map_Selection.Y)
                        Lists.Map[Selected].Light.RemoveAt(i);
    }

    private void picMap_MouseUp(object sender, MouseEventArgs e)
    {
        Map_Pressed = false;

        // Somente se necessário
        if (e.Button != MouseButtons.Left) return;
        if (lstLayers.SelectedIndices.Count == 0) return;
        if (Map_Selection.X > Lists.Map[Selected].Width || Map_Selection.Y > Lists.Map[Selected].Height) return;

        // Camada selecionada
        byte Layer = (byte)Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

        // Retângulo
        if (butRectangle.Checked)
        {
            if (Map_Selection.Width > 1 || Map_Selection.Height > 1)
                // Define os azulejos
                for (int x = Map_Selection.X; x < Map_Selection.X + Map_Selection.Width; x++)
                    for (int y = Map_Selection.Y; y < Map_Selection.Y + Map_Selection.Height; y++)
                    {
                        Lists.Map[Selected].Layer[Layer].Tile[x, y] = Set_Tile();
                        Autotile.Update(Selected, x, y, Layer);
                    }
        }
        // Iluminação
        else if (butMLighting.Checked)
            Lists.Map[Selected].Light.Add(new Lists.Structures.Map_Light(Map_Selection));
        // Nenhum
        else
            return;

        // Reseta o tamanho da seleção
        Def_Map_Selection.Size = new Size(1, 1);
    }

    private void picMap_MouseMove(object sender, MouseEventArgs e)
    {
        // Mouse
        Map_Mouse.X = e.X / Globals.Grid_Zoom + scrlMapX.Value;
        Map_Mouse.Y = e.Y / Globals.Grid_Zoom + scrlMapY.Value;

        // Impede que saia do limite da tela
        if (Map_Mouse.X < 0) Map_Mouse.X = 0;
        if (Map_Mouse.Y < 0) Map_Mouse.Y = 0;
        if (Map_Mouse.X > Lists.Map[Selected].Width) Map_Mouse.X = Lists.Map[Selected].Width;
        if (Map_Mouse.Y > Lists.Map[Selected].Height) Map_Mouse.Y = Lists.Map[Selected].Height;

        // Previne erros
        if (Selected <= 0) return;

        // Cria um retângulo
        if (Map_Rectangle(e)) return;

        // Não mover e nem executar nada caso for a ferramenta de área 
        if (butArea.Checked && butArea.Enabled) return;

        // Define a posição da seleção
        Def_Map_Selection.Location = Map_Mouse;

        // Executa um evento de acordo com a ferramenta selecionada
        if (butMNormal.Checked)
            Tile_Events(e.Button);
        else if (butMZones.Checked)
        {
            // Define as zonas
            if (e.Button == MouseButtons.Left)
                Lists.Map[Selected].Tile[Map_Mouse.X, Map_Mouse.Y].Zone = (byte)scrlZone.Value;
            else if (e.Button == MouseButtons.Right)
                Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Zone = 0;
        }
        else if (butMAttributes.Checked && !optA_DirBlock.Checked)
        {
            // Define as zonas
            if (e.Button == MouseButtons.Left)
                Lists.Map[Selected].Tile[Map_Mouse.X, Map_Mouse.Y].Attribute = (byte)Attribute_Selected();
            else if (e.Button == MouseButtons.Right)
                Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Attribute = 0;
        }
    }

    private void Tile_Events(MouseButtons e)
    {
        // Previne erros
        if (lstLayers.SelectedIndices.Count == 0) return;

        // Camada selecionada
        byte Layer = (byte)Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

        // Executa um evento de acordo com a ferramenta selecionada
        if (e == MouseButtons.Left)
        {
            if (butPencil.Checked) Tile_Set(Layer);
            if (butDiscover.Checked) Tile_Discover();
        }
        else if (e == MouseButtons.Right)
        {
            if (butPencil.Checked) Tile_Clear(Layer);
        }
    }

    private bool Map_Rectangle(MouseEventArgs e)
    {
        int x = e.X / Globals.Grid_Zoom + scrlMapX.Value, y = e.Y / Globals.Grid_Zoom + scrlMapY.Value;

        // Somente se necessário
        if (e.Button != MouseButtons.Left) return false;
        if (butMLighting.Checked) goto Continuation;
        if (butRectangle.Checked && butRectangle.Enabled) goto Continuation;
        if (butArea.Checked && butArea.Enabled) goto Continuation;
        return false;
    Continuation:

        // Cria um retângulo
        if (!Map_Pressed) Def_Map_Selection.Size = new Size(1, 1);

        // Verifica se não passou do limite
        if (x < 0) x = 0;
        if (x > Lists.Map[Selected].Width) x = Lists.Map[Selected].Width;
        if (y < 0) y = 0;
        if (y > Lists.Map[Selected].Height) y = Lists.Map[Selected].Height;

        // Define o tamanho
        Def_Map_Selection.Width = x - Def_Map_Selection.X + 1;
        Def_Map_Selection.Height = y - Def_Map_Selection.Y + 1;
        Map_Pressed = true;
        return true;
    }

    private void Tile_Discover()
    {
        Lists.Structures.Map_Tile_Data Data;

        for (int c = Lists.Map[Selected].Layer.Count - 1; c >= 0; c--)
        {
            Data = Lists.Map[Selected].Layer[c].Tile[Map_Selection.X, Map_Selection.Y];

            // Somente se necessário
            if (!Objects.lstLayers.Items[c].Checked) continue;
            if (Data.Tile == 0) continue;

            // Define o azulejo
            cmbTiles.SelectedIndex = Data.Tile - 1;
            chkAuto.Checked = Data.Auto;
            Def_Tiles_Selection = new Rectangle(Data.X, Data.Y, 1, 1);
            return;
        }
    }

    private Lists.Structures.Map_Tile_Data Set_Tile(byte x = 0, byte y = 0)
    {
        Lists.Structures.Map_Tile_Data Temp_Tile = new Lists.Structures.Map_Tile_Data();

        // Posição padrão
        if (x == 0) x = (byte)Tiles_Selection.X;
        if (y == 0) y = (byte)Tiles_Selection.Y;

        // Define os valores da camada
        Temp_Tile.Mini = new Point[4];
        Temp_Tile.X = x;
        Temp_Tile.Y = y;
        Temp_Tile.Tile = (byte)(cmbTiles.SelectedIndex + 1);
        Temp_Tile.Auto = chkAuto.Checked;

        // Retorna o azulejo
        return Temp_Tile;
    }

    private void Tile_Set(byte Layer_Num)
    {
        // Define múltiplos azulejos
        if (Tiles_Selection.Width > 1 || Tiles_Selection.Height > 1)
            Tile_Set_Multiples(Layer_Num);

        // Defini um único azulejo
        Lists.Map[Selected].Layer[Layer_Num].Tile[Map_Selection.X, Map_Selection.Y] = Set_Tile();
        Autotile.Update(Selected, Map_Selection.X, Map_Selection.Y, Layer_Num);
    }

    private void Tile_Set_Multiples(byte Layer_Num)
    {
        byte x2 = 0, y2;
        Size Size = new Size(Map_Selection.X + Tiles_Selection.Width - 1, Map_Selection.Y + Tiles_Selection.Height - 1);

        // Apenas se necessário
        if (chkAuto.Checked) return;

        // Define todos os azulejos selecionados
        for (int x = Map_Selection.X; x <= Size.Width; x++)
        {
            y2 = 0;
            for (int y = Map_Selection.Y; y <= Size.Height; y++)
            {
                // Define os azulejos
                if (!OutLimit(Selected, (short)x, (short)y))
                {
                    Lists.Map[Selected].Layer[Layer_Num].Tile[x, y] = Set_Tile((byte)(Tiles_Selection.X + x2), (byte)(Tiles_Selection.Y + y2));
                    Autotile.Update(Selected, x, y, Layer_Num);
                }
                y2++;
            }
            x2++;
        }
    }

    private void Tile_Clear(byte Layer_Num)
    {
        // Limpa a camada
        Lists.Map[Selected].Layer[Layer_Num].Tile[Map_Selection.X, Map_Selection.Y] = new Lists.Structures.Map_Tile_Data();
        Lists.Map[Selected].Layer[Layer_Num].Tile[Map_Selection.X, Map_Selection.Y].Mini = new Point[4];
        Autotile.Update(Selected, Map_Selection.X, Map_Selection.Y, Layer_Num);
    }
    #endregion

    #region Layers
    private static void Update_List_Layers()
    {
        // Limpa a lista
        Objects.lstLayers.Items.Clear();

        // Adiciona os itens à lista
        for (byte i = 0; i < Lists.Map[Selected].Layer.Count; i++)
        {
            Objects.lstLayers.Items.Add(string.Empty);
            Objects.lstLayers.Items[i].Checked = true;
            Objects.lstLayers.Items[i].SubItems.Add((i + 1).ToString());
            Objects.lstLayers.Items[i].SubItems.Add(Lists.Map[Selected].Layer[i].Name);
            Objects.lstLayers.Items[i].SubItems.Add(((Globals.Layers)Lists.Map[Selected].Layer[i].Type).ToString());
        }

        // Seleciona o primeiro item
        Objects.lstLayers.Items[0].Selected = true;
    }

    private void butLayer_Add_Click(object sender, EventArgs e)
    {
        Lists.Structures.Map_Layer Layer = new Lists.Structures.Map_Layer();

        // Verifica se o nome é válido
        if (txtLayer_Name.Text.Length < 1 || txtLayer_Name.Text.Length > 12) return;
        if (Find_Layer(txtLayer_Name.Text) >= 0)
        {
            MessageBox.Show("There is already a layer with that name.");
            return;
        }

        // Define os dados
        Layer.Name = txtLayer_Name.Text;
        Layer.Type = (byte)cmbLayers_Type.SelectedIndex;
        Layer.Tile = new Lists.Structures.Map_Tile_Data[Lists.Map[Selected].Width + 1, Lists.Map[Selected].Height + 1];
        for (byte x = 0; x <= Lists.Map[Selected].Width; x++)
            for (byte y = 0; y <= Lists.Map[Selected].Height; y++)
                Layer.Tile[x, y].Mini = new Point[4];

        // Adiciona a camada
        Lists.Map[Selected].Layer.Add(Layer);

        // Atualiza a lista
        Update_Layers();
        grpLayer_Add.Visible = false;
    }

    private void butLayer_Edit_Click(object sender, EventArgs e)
    {
        // Evita erros
        if (lstLayers.SelectedItems.Count == 0) return;
        if (txtLayer_Name.Text.Length < 1 || txtLayer_Name.Text.Length > 12) return;
        if (lstLayers.SelectedItems[0].SubItems[2].Text != txtLayer_Name.Text)
            if (Find_Layer(txtLayer_Name.Text) >= 0)
            {
                MessageBox.Show("There is already a layer with that name.");
                return;
            }

        // Define os dados
        Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Name = txtLayer_Name.Text;
        Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Type = (byte)cmbLayers_Type.SelectedIndex;

        // Atualiza a lista
        Update_Layers();
        grpLayer_Add.Visible = false;
    }

    public void Update_Layers()
    {
        List<Lists.Structures.Map_Layer> Temp = new List<Lists.Structures.Map_Layer>();

        // Reordena as camadas
        for (byte n = 0; n < (byte)Globals.Layers.Count; n++)
            for (byte i = 0; i < Lists.Map[Selected].Layer.Count; i++)
                if (Lists.Map[Selected].Layer[i].Type == n)
                    Temp.Add(Lists.Map[Selected].Layer[i]);

        // Atualiza os valores
        Lists.Map[Selected].Layer = Temp;
        Update_List_Layers();
    }

    private void butLayers_Add_Click(object sender, EventArgs e)
    {
        // Reseta os valores
        txtLayer_Name.Text = string.Empty;
        cmbLayers_Type.SelectedIndex = 0;

        // Abre a janela em modo de criação
        butLayer_Add.Visible = true;
        butLayer_Edit.Visible = false;
        grpLayer_Add.Text = "Add layer";
        grpLayer_Add.BringToFront();
        grpLayer_Add.Visible = true;
    }

    private void butLayers_Remove_Click(object sender, EventArgs e)
    {
        // Apenas se necessário
        if (Objects.lstLayers.Items.Count == 1) return;
        if (lstLayers.SelectedItems.Count == 0) return;

        // Index
        int Index = Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

        // Remove a camada
        if (Index >= 0)
        {
            Lists.Map[Selected].Layer.RemoveAt(Index);
            Update_List_Layers();
        }
    }

    private static int Find_Layer(string Nome)
    {
        // Encontra a camada
        for (byte i = 0; i < Lists.Map[Selected].Layer.Count; i++)
            if (Lists.Map[Selected].Layer[i].Name == Nome)
                return i;

        return -1;
    }

    private void butLayer_Cancel_Click(object sender, EventArgs e)
    {
        // Fecha a janela
        grpLayer_Add.Visible = false;
    }

    private void butLayers_Up_Click(object sender, EventArgs e)
    {
        // Somente se necessário
        if (Objects.lstLayers.Items.Count == 1) return;
        if (lstLayers.SelectedItems.Count == 0) return;
        if (lstLayers.SelectedItems[0].Index == 0) return;

        // Dados
        List<Lists.Structures.Map_Layer> Temp = new List<Lists.Structures.Map_Layer>(Lists.Map[Selected].Layer);
        int Layer_Num = lstLayers.SelectedItems[0].Index;

        if (Temp[Layer_Num - 1].Type == Temp[Layer_Num].Type)
        {
            // Altera as posições
            Temp[Layer_Num - 1] = Lists.Map[Selected].Layer[Layer_Num];
            Temp[Layer_Num] = Lists.Map[Selected].Layer[Layer_Num - 1];
            Lists.Map[Selected].Layer = Temp;

            // Atualiza a lista
            Update_List_Layers();
        }
    }

    private void butLayers_Down_Click(object sender, EventArgs e)
    {
        // Somente se necessário
        if (Objects.lstLayers.Items.Count == 1) return;
        if (lstLayers.SelectedItems.Count == 0) return;
        if (lstLayers.SelectedItems[0].Index == lstLayers.Items.Count - 1) return;

        // Dados
        List<Lists.Structures.Map_Layer> Temp = new List<Lists.Structures.Map_Layer>(Lists.Map[Selected].Layer);
        int Layer_Num = lstLayers.SelectedItems[0].Index;

        if (Temp[Layer_Num + 1].Type == Temp[Layer_Num].Type)
        {
            // Altera as posições
            Temp[Layer_Num + 1] = Lists.Map[Selected].Layer[Layer_Num];
            Temp[Layer_Num] = Lists.Map[Selected].Layer[Layer_Num + 1];
            Lists.Map[Selected].Layer = Temp;

            // Atualiza a lista
            Update_List_Layers();
        }
    }

    private void butLayers_Edit_Click(object sender, EventArgs e)
    {
        // Previne erros
        if (lstLayers.SelectedItems.Count == 0) return;

        // Define os valores
        txtLayer_Name.Text = Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Name;
        cmbLayers_Type.SelectedIndex = Lists.Map[Selected].Layer[lstLayers.SelectedItems[0].Index].Type;

        // Abre a janela em modo de edição
        butLayer_Add.Visible = false;
        butLayer_Edit.Visible = true;
        grpLayer_Add.Text = "Edit layer";
        grpLayer_Add.BringToFront();
        grpLayer_Add.Visible = true;
    }
    #endregion

    #region Calculations
    #region Zoom
    // Retângulo da seleção de azulejos
    private static Rectangle Tiles_Selection // Somente para obter
    {
        get
        {
            return Selection_Rec(Def_Tiles_Selection);
        }
    }

    // Retângulo do mapa
    public static Rectangle Map_Selection
    {
        get
        {
            if (Objects.chkAuto.Checked)
                return new Rectangle(Map_Mouse, new Size(1, 1));
            else if (Objects.butMNormal.Checked && Objects.butPencil.Checked)
                return new Rectangle(Map_Mouse, Tiles_Selection.Size);
            else
                return Selection_Rec(Def_Map_Selection);
        }
    }

    public static byte Zoom()
    {
        // Retorna o valor do zoom
        if (Objects.butZoom_2x.Checked) return 2;
        else if (Objects.butZoom_4x.Checked) return 4;
        else return 1;
    }

    public static int Zoom(int Value)
    {
        // Retorna o valor com o zoom
        return Value /= Zoom();
    }

    public static Rectangle Zoom(Rectangle Value)
    {
        // Retorna o valor com o zoom
        return new Rectangle(new Point(Value.X / Zoom(), Value.Y / Zoom()), new Size(Value.Width / Zoom(), Value.Height / Zoom()));
    }

    private static byte Grid_Zoom
    {
        // Tamanho da grade com o zoom
        get
        {
            return (byte)(Globals.Grid / Zoom());
        }
    }

    public static Rectangle Zoom_Grid(Rectangle Rectangle)
    {
        // Tamanho da grade com o zoom
        return new Rectangle(Rectangle.X * Grid_Zoom, Rectangle.Y * Grid_Zoom, Rectangle.Width * Grid_Zoom, Rectangle.Height * Grid_Zoom);
    }
    #endregion

    private static Rectangle Selection_Rec(Rectangle Temp)
    {
        // Largura
        if (Temp.Width <= 0)
        {
            Temp.X += Temp.Width - 1;
            Temp.Width = (Temp.Width - 2) * -1;
        }
        // Altura
        if (Temp.Height <= 0)
        {
            Temp.Y += Temp.Height - 1;
            Temp.Height = (Temp.Height - 2) * -1;
        }

        // Retorna o valor do retângulo
        return Temp;
    }

    public bool OutLimit(short Map_Num, short x, short y)
    {
        // Verifica se as coordenas estão no limite do mapa
        if (x > Lists.Map[Map_Num].Width || y > Lists.Map[Map_Num].Height || x < 0 || y < 0)
            return true;
        else
            return false;
    }

    public static Size Map_Size
    {
        // Quantidade de azulejos visíveis
        get
        {
            return new Size(Lists.Map[Selected].Width, Lists.Map[Selected].Height);
        }
    }

    public static Rectangle Tile_Source
    {
        // Retorna com o retângulo do azulejo em relação à fonte
        get
        {
            return new Rectangle(Tiles_Selection.X * Globals.Grid, Tiles_Selection.Y * Globals.Grid, Tiles_Selection.Width * Globals.Grid, Tiles_Selection.Height * Globals.Grid);
        }
    }
    #endregion

    class Autotile
    {
        // Formas de adicionar o mini azulejo
        public enum AddMode
        {
            None,
            Inside,
            Exterior,
            Horizontal,
            Vertical,
            Fill
        }

        public static void Update(short Map_Num)
        {
            // Atualiza os azulejos necessários
            for (byte x = 0; x <= Lists.Map[Map_Num].Width; x++)
                for (byte y = 0; y <= Lists.Map[Map_Num].Height; y++)
                    for (byte c = 0; c < Lists.Map[Map_Num].Layer.Count; c++)
                        if (Lists.Map[Map_Num].Layer[c].Tile[x, y].Auto)
                            // Faz os cálculos para a autocriação
                            Calculate(Map_Num, x, y, c);
        }

        public static void Update(short Map_Num, int x, int y, byte Layer_Num)
        {
            // Atualiza os azulejos necessários
            for (int x2 = x - 2; x2 <= x + 2; x2++)
                for (int y2 = y - 2; y2 <= y + 2; y2++)
                    if (x2 >= 0 && x2 <= Lists.Map[Map_Num].Width && y2 >= 0 && y2 <= Lists.Map[Map_Num].Height)
                        // Faz os cálculos para a autocriação
                        Calculate(Map_Num, (byte)x2, (byte)y2, Layer_Num);
        }

        private static void Set(short Map_Num, byte x, byte y, byte Layer_Num, byte Part, string Index)
        {
            Point Position = new Point(0);

            // Posições exatas dos mini azulejos (16x16)
            switch (Index)
            {
                // Quinas
                case "a": Position = new Point(32, 0); break;
                case "b": Position = new Point(48, 0); break;
                case "c": Position = new Point(32, 16); break;
                case "d": Position = new Point(48, 16); break;

                // Noroeste
                case "e": Position = new Point(0, 32); break;
                case "f": Position = new Point(16, 32); break;
                case "g": Position = new Point(0, 48); break;
                case "h": Position = new Point(16, 48); break;

                // Nordeste
                case "i": Position = new Point(32, 32); break;
                case "j": Position = new Point(48, 32); break;
                case "k": Position = new Point(32, 48); break;
                case "l": Position = new Point(48, 48); break;

                // Sudoeste
                case "m": Position = new Point(0, 64); break;
                case "n": Position = new Point(16, 64); break;
                case "o": Position = new Point(0, 80); break;
                case "p": Position = new Point(16, 80); break;

                // Sudeste
                case "q": Position = new Point(32, 64); break;
                case "r": Position = new Point(48, 64); break;
                case "s": Position = new Point(32, 80); break;
                case "t": Position = new Point(48, 80); break;
            }

            // Define a posição do mini azulejo
            Lists.Structures.Map_Tile_Data Data = Lists.Map[Map_Num].Layer[Layer_Num].Tile[x, y];
            Lists.Map[Map_Num].Layer[Layer_Num].Tile[x, y].Mini[Part].X = Data.X * Globals.Grid + Position.X;
            Lists.Map[Map_Num].Layer[Layer_Num].Tile[x, y].Mini[Part].Y = Data.Y * Globals.Grid + Position.Y;
        }

        private static bool Check(short Map_Num, int X1, int Y1, int X2, int Y2, byte Layer_Num)
        {
            Lists.Structures.Map_Tile_Data Data1, Data2;

            // Somente se necessário
            if (X2 < 0 || X2 > Lists.Map[Map_Num].Width || Y2 < 0 || Y2 > Lists.Map[Map_Num].Height) return true;

            // Dados
            Data1 = Lists.Map[Map_Num].Layer[Layer_Num].Tile[X1, Y1];
            Data2 = Lists.Map[Map_Num].Layer[Layer_Num].Tile[X2, Y2];

            // Verifica se são os mesmo azulejos
            if (!Data2.Auto) return false;
            if (Data1.Tile != Data2.Tile) return false;
            if (Data1.X != Data2.X) return false;
            if (Data1.Y != Data2.Y) return false;

            // Não há nada de errado
            return true;
        }

        private static void Calculate(short Map_Num, byte x, byte y, byte Layer_Num)
        {
            // Calcula as quatros partes do azulejo
            Calculate_NW(Map_Num, x, y, Layer_Num);
            Calculate_NE(Map_Num, x, y, Layer_Num);
            Calculate_SW(Map_Num, x, y, Layer_Num);
            Calculate_SE(Map_Num, x, y, Layer_Num);
        }

        private static void Calculate_NW(short Map_Num, byte x, byte y, byte Layer_Num)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(Map_Num, x, y, x - 1, y - 1, Layer_Num)) Tile[1] = true;
            if (Check(Map_Num, x, y, x, y - 1, Layer_Num)) Tile[2] = true;
            if (Check(Map_Num, x, y, x - 1, y, Layer_Num)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[2] && !Tile[3]) Mode = AddMode.Inside;
            if (!Tile[2] && Tile[3]) Mode = AddMode.Horizontal;
            if (Tile[2] && !Tile[3]) Mode = AddMode.Vertical;
            if (!Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(Map_Num, x, y, Layer_Num, 0, "e"); break;
                case AddMode.Exterior: Set(Map_Num, x, y, Layer_Num, 0, "a"); break;
                case AddMode.Horizontal: Set(Map_Num, x, y, Layer_Num, 0, "i"); break;
                case AddMode.Vertical: Set(Map_Num, x, y, Layer_Num, 0, "m"); break;
                case AddMode.Fill: Set(Map_Num, x, y, Layer_Num, 0, "q"); break;
            }
        }

        private static void Calculate_NE(short Map_Num, byte x, byte y, byte Layer_Num)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Norte, Oeste, Noroeste)
            if (Check(Map_Num, x, y, x, y - 1, Layer_Num)) Tile[1] = true;
            if (Check(Map_Num, x, y, x + 1, y - 1, Layer_Num)) Tile[2] = true;
            if (Check(Map_Num, x, y, x + 1, y, Layer_Num)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[1] && !Tile[3]) Mode = AddMode.Inside;
            if (!Tile[1] && Tile[3]) Mode = AddMode.Horizontal;
            if (Tile[1] && !Tile[3]) Mode = AddMode.Vertical;
            if (Tile[1] && !Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(Map_Num, x, y, Layer_Num, 1, "j"); break;
                case AddMode.Exterior: Set(Map_Num, x, y, Layer_Num, 1, "b"); break;
                case AddMode.Horizontal: Set(Map_Num, x, y, Layer_Num, 1, "f"); break;
                case AddMode.Vertical: Set(Map_Num, x, y, Layer_Num, 1, "r"); break;
                case AddMode.Fill: Set(Map_Num, x, y, Layer_Num, 1, "n"); break;
            }
        }

        private static void Calculate_SW(short Map_Num, byte x, byte y, byte Layer_Num)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudoeste)
            if (Check(Map_Num, x, y, x - 1, y, Layer_Num)) Tile[1] = true;
            if (Check(Map_Num, x, y, x - 1, y + 1, Layer_Num)) Tile[2] = true;
            if (Check(Map_Num, x, y, x, y + 1, Layer_Num)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[1] && !Tile[3]) Mode = AddMode.Inside;
            if (Tile[1] && !Tile[3]) Mode = AddMode.Horizontal;
            if (!Tile[1] && Tile[3]) Mode = AddMode.Vertical;
            if (Tile[1] && !Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(Map_Num, x, y, Layer_Num, 2, "o"); break;
                case AddMode.Exterior: Set(Map_Num, x, y, Layer_Num, 2, "c"); break;
                case AddMode.Horizontal: Set(Map_Num, x, y, Layer_Num, 2, "s"); break;
                case AddMode.Vertical: Set(Map_Num, x, y, Layer_Num, 2, "g"); break;
                case AddMode.Fill: Set(Map_Num, x, y, Layer_Num, 2, "k"); break;
            }
        }

        private static void Calculate_SE(short Map_Num, byte x, byte y, byte Layer_Num)
        {
            bool[] Tile = new bool[4];
            AddMode Mode = AddMode.None;

            // Verifica se existe algo para modificar nos azulejos em volta (Sul, Oeste, Sudeste)
            if (Check(Map_Num, x, y, x, y + 1, Layer_Num)) Tile[1] = true;
            if (Check(Map_Num, x, y, x + 1, y + 1, Layer_Num)) Tile[2] = true;
            if (Check(Map_Num, x, y, x + 1, y, Layer_Num)) Tile[3] = true;

            // Forma que será adicionado o mini azulejo
            if (!Tile[1] && !Tile[3]) Mode = AddMode.Inside;
            if (!Tile[1] && Tile[3]) Mode = AddMode.Horizontal;
            if (Tile[1] && !Tile[3]) Mode = AddMode.Vertical;
            if (Tile[1] && !Tile[2] && Tile[3]) Mode = AddMode.Exterior;
            if (Tile[1] && Tile[2] && Tile[3]) Mode = AddMode.Fill;

            // Define o mini azulejo
            switch (Mode)
            {
                case AddMode.Inside: Set(Map_Num, x, y, Layer_Num, 3, "t"); break;
                case AddMode.Exterior: Set(Map_Num, x, y, Layer_Num, 3, "d"); break;
                case AddMode.Horizontal: Set(Map_Num, x, y, Layer_Num, 3, "p"); break;
                case AddMode.Vertical: Set(Map_Num, x, y, Layer_Num, 3, "l"); break;
                case AddMode.Fill: Set(Map_Num, x, y, Layer_Num, 3, "h"); break;
            }
        }
    }

    private void butNPC_Remove_Click(object sender, EventArgs e)
    {
        // Previne erros
        if (Objects.lstNPC.SelectedIndex < 0) return;

        // Limpa todos os NPCs do mapa
        Lists.Map[Selected].NPC.RemoveAt(Objects.lstNPC.SelectedIndex);
        Update_NPCs();
    }

    private void butNPC_Clear_Click(object sender, EventArgs e)
    {
        // Limpa todos os NPCs do mapa
        Lists.Map[Selected].NPC = new List<Lists.Structures.Map_NPC>();
        Update_NPCs();
    }

    private void butNPC_Add_Click(object sender, EventArgs e)
    {
        // Adiciona o NPC
        AddNPC();
    }

    private static void AddNPC(bool Posição = false, byte X = 0, byte Y = 0)
    {
        Lists.Structures.Map_NPC Data = new Lists.Structures.Map_NPC();

        // Define os dados
        Data.Index = (short)(Objects.cmbNPC.SelectedIndex + 1);
        Data.Zone = (byte)Objects.numNPC_Zone.Value;
        Data.Spawn = Posição;
        Data.X = X;
        Data.Y = Y;

        // Adiciona o NPC
        Lists.Map[Selected].NPC.Add(Data);
        Update_NPCs();

        // Limpa os valores
        Objects.cmbNPC.SelectedIndex = 0;
        Objects.numNPC_Zone.Value = 0;
    }

    private static void Update_NPCs()
    {
        // Limpa a lista
        Objects.lstNPC.Items.Clear();

        // Atualiza a lista de NPCs do mapa
        if (Lists.Map[Selected].NPC.Count > 0)
        {
            for (byte i = 0; i < Lists.Map[Selected].NPC.Count; i++) Objects.lstNPC.Items.Add(i + 1 + ":" + Lists.NPC[Lists.Map[Selected].NPC[i].Index].Name);
            Objects.lstNPC.SelectedIndex = 0;
        }
    }

    private static void Set_Attribute(MouseEventArgs e)
    {
        // Define os Atributos
        if (e.Button == MouseButtons.Left)
        {
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_1 = AData_1;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_2 = AData_2;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_3 = AData_3;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_4 = AData_4;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Attribute = (byte)Attribute_Selected();
        }
        // Limpa os dados
        else if (e.Button == MouseButtons.Right)
        {
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_1 = 0;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_2 = 0;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_3 = 0;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Data_4 = 0;
            Lists.Map[Selected].Tile[Map_Selection.X, Map_Selection.Y].Attribute = 0;
        }
    }

    private static Globals.Tile_Attributes Attribute_Selected()
    {
        // Retorna com o atributo selecionado
        if (Objects.optA_Block.Checked) return Globals.Tile_Attributes.Block;
        else if (Objects.optA_Warp.Checked) return Globals.Tile_Attributes.Warp;
        else if (Objects.optA_Item.Checked) return Globals.Tile_Attributes.Item;
        else return Globals.Tile_Attributes.None;
    }

    private void optA_DirBlock_CheckedChanged(object sender, EventArgs e)
    {
        // Define a visibilidade do painel
        if (optA_DirBlock.Checked) grpAttributes_Set.Visible = false;
    }

    private void optA_Block_CheckedChanged(object sender, EventArgs e)
    {
        // Define a visibilidade do painel
        if (optA_Block.Checked) grpAttributes_Set.Visible = false;
    }

    private void optA_Warp_CheckedChanged(object sender, EventArgs e)
    {
        // Define a visibilidade do painel
        if (optA_Warp.Checked)
        {
            grpA_Warp.Visible = true;
            grpAttributes_Set.Visible = true;
        }
        else
            grpA_Warp.Visible = false;
    }

    private void optA_Item_CheckedChanged(object sender, EventArgs e)
    {
        // Define a visibilidade do painel
        if (optA_Item.Checked)
        {
            grpA_Item.Visible = true;
            grpAttributes_Set.Visible = true;
        }
        else
            grpA_Item.Visible = false;
    }

    private void cmbA_Warp_Map_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Define os limites
        cmbA_Warp_X.Maximum = Lists.Map[cmbA_Warp_Map.SelectedIndex + 1].Width;
        cmbA_Warp_Y.Maximum = Lists.Map[cmbA_Warp_Map.SelectedIndex + 1].Height;
    }

    private void butA_Warp_Click(object sender, EventArgs e)
    {
        // Fecha a janela e define os dodos
        grpAttributes_Set.Visible = false;
        AData_1 = (short)(cmbA_Warp_Map.SelectedIndex + 1);
        AData_2 = (short)cmbA_Warp_X.Value;
        AData_3 = (short)cmbA_Warp_Y.Value;
        AData_4 = (short)(cmbA_Warp_Direction.SelectedIndex);

        // Reseta as ferramentas
        cmbA_Warp_Map.SelectedIndex = 0;
        cmbA_Warp_X.Value = 0;
        cmbA_Warp_Y.Value = 0;
        cmbA_Warp_Direction.SelectedIndex = 0;
    }

    private void butA_Item_Click(object sender, EventArgs e)
    {
        // Fecha a janela e define os dodos
        grpAttributes_Set.Visible = false;
        AData_1 = (short)(cmbA_Item.SelectedIndex + 1);
        AData_2 = (short)cmbA_Item_Amount.Value;

        // Reseta as ferramentas
        cmbA_Warp_Map.SelectedIndex = 0;
        cmbA_Warp_X.Value = 0;
        cmbA_Warp_Y.Value = 0;
        cmbA_Warp_Direction.SelectedIndex = 0;
    }
}
