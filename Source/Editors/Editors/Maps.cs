using DarkUI.Forms;
using Entities;
using Library;
using Logic;
using Network;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static Logic.Utils;

namespace Editors
{
    partial class Editor_Maps : DarkForm
    {
        #region Data
        // Usado para acessar os dados da janela
        public static Editor_Maps Form;

        // Mapa selecionado
        public Map Selected;

        // Dados temporários
        private bool Map_Pressed;

        // Posição do mouse
        public Point Tile_Mouse { get; set; }
        private Point Map_Mouse;

        // Seleção retângular
        private Rectangle Def_Tiles_Selection = new Rectangle(0, 0, 1, 1);
        private Rectangle Def_Map_Selection = new Rectangle(0, 0, 1, 1);

        // Dados dos atributos
        private string AData_1;
        private short AData_2;
        private short AData_3;
        private short AData_4;

        // Azulejos copiados
        private Copy_Struct Tiles_Copy = new Copy_Struct();

        public struct Copy_Struct
        {
            public Rectangle Area;
            public MapLayer[] Data;

        }
        #endregion

        #region Base
        public Editor_Maps()
        {
            // Inicializa os componentes
            InitializeComponent();
            Graphics.Win_Map = new SFML.Graphics.RenderWindow(picMap.Handle);
            Graphics.Win_Map_Tile = new SFML.Graphics.RenderWindow(picTile.Handle);
            Graphics.Win_Map_Lighting = new SFML.Graphics.RenderTexture((uint)Width, (uint)Height);

            // Lista os dados
            for (byte i = 0; i < (byte)Layers.Count; i++) cmbLayers_Type.Items.Add(((Layers)i).ToString());
            for (byte i = 1; i < Graphics.Tex_Tile.Length; i++) cmbTiles.Items.Add(i.ToString());
            Update_List();

            // Define os limites
            scrlZone.Maximum = Map.Num_Zones;
            numNPC_Zone.Maximum = Map.Num_Zones;
            numA_Warp_X.Maximum = Map.Width - 1;
            numA_Warp_Y.Maximum = Map.Height - 1;

            // Reseta os valores
            grpAttributes.BringToFront();
            grpZones.BringToFront();
            grpNPCs.BringToFront();
            grpLighting.BringToFront();
            cmbTiles.SelectedIndex = 0;
            cmbLayers_Type.SelectedIndex = 0;
            butGrid.Checked = Lists.Options.Pre_Map_Grid;
            butAudio.Checked = Lists.Options.Pre_Map_Audio;
            if (!Lists.Options.Pre_Map_View)
            {
                butVisualization.Checked = false;
                butEdition.Checked = true;
            }

            // Abre a janela
            Login.Form.Visible = false;
            Show();
        }

        private void Editor_Maps_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.Working = false;
        }

        private void Editor_Maps_SizeChanged(object sender, EventArgs e)
        {
            // Atualiza os limites
            Update_Map_Bounds();
            Update_Tile_Bounds();
        }

        private void Update_List()
        {
            // Limpa as listas
            List.Nodes.Clear();
            cmbA_Warp_Map.Items.Clear();

            // Adiciona os itens às listas
            foreach (var Map in Map.List.Values)
            {
                if (Map.Name.StartsWith(txtFilter.Text))
                {
                    List.Nodes.Add(Map.Name);
                    List.Nodes[List.Nodes.Count - 1].Tag = Map.ID;
                }
                cmbA_Warp_Map.Items.Add(Map);
            }

            // Seleciona o primeiro item
            if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
        }

        private void List_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Selected = Map.List[(Guid)List.SelectedNode.Tag];

            // Conecta as listas com os componentes
            prgProperties.SelectedObject = new MapProperties(Selected);
            lstNPC.DataSource = Selected.NPC;

            // Reseta o clima
            Map.UpdateWeather();

            // Faz os cálculos da autocriação
            Selected.Update();

            // Atualiza os dados
            Update_Map_Bounds();
            Update_List_Layers();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            Update_List();
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            // Adiciona uma loja nova
            Map New = new Map(Guid.NewGuid());
            Map.List.Add(New.ID, New);
            cmbA_Warp_Map.Items.Add(New);
            New.Name = "New map";
            New.Layer.Add(new MapLayer());
            New.Layer[0].Name = "Ground";

            // Azulejos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                {
                    New.Layer[0].Tile[x, y] = new MapTileData();
                    New.Attribute[x, y] = new Map_Attribute();
                }

            // Adiciona na lista
            TreeNode Node = new TreeNode(New.Name);
            Node.Tag = New.ID;
            List.Nodes.Add(Node);
            List.SelectedNode = Node;
        }

        private void butRemove_Click(object sender, EventArgs e)
        {
            if (List.SelectedNode != null)
            {
                // Garante que sempre vai ter pelo menos uma mapa
                if (Map.List.Count == 1)
                {
                    MessageBox.Show("It must have at least one map registered.");
                    return;
                }

                // Remove o mapa selecionado
                cmbA_Warp_Map.Items.Remove(Selected);
                Map.List.Remove(Selected.ID);
                List.SelectedNode.Remove();
            }
        }

        private void Update_Strip()
        {
            // Atualiza as informações da barra
            Strip.Items[0].Text = "FPS: " + Program.FPS;
            Strip.Items[2].Text = "Revision: " + Selected.Revision;
            Strip.Items[4].Text = "Position: {" + Map_Mouse.X + ";" + Map_Mouse.Y + "}"; ;
        }
        #endregion

        #region Toolbox
        private void butSaveAll_Click(object sender, EventArgs e)
        {
            // Salva todos os dados
            Send.Write_Maps();
            MessageBox.Show("All maps has been saved");
        }

        private void butReload_Click(object sender, EventArgs e)
        {
            // Recarrega o mapa
            Send.Request_Map(Selected);
            Update_List_Layers();
            Selected.Update();
        }

        private void Copy()
        {
            Tiles_Copy.Data = new MapLayer[Selected.Layer.Count];

            // Seleção
            Tiles_Copy.Area = Map_Selection;

            // Copia os dados das camadas
            for (byte c = 0; c < Tiles_Copy.Data.Length; c++)
            {
                Tiles_Copy.Data[c] = new MapLayer();
                Tiles_Copy.Data[c].Name = Selected.Layer[c].Name;

                // Copia os dados dos azulejos
                for (byte x = 0; x < Map.Width; x++)
                    for (byte y = 0; y < Map.Height; y++)
                        Tiles_Copy.Data[c].Tile[x, y] = Selected.Layer[c].Tile[x, y];
            }
        }

        private void butCopy_Click(object sender, EventArgs e)
        {
            // Copia os dados
            Copy();
        }

        private void butCut_Click(object sender, EventArgs e)
        {
            // Copia os dados
            Copy();

            // Remove os azulejos copiados
            for (int x = Map_Selection.X; x < Map_Selection.X + Map_Selection.Width; x++)
                for (int y = Map_Selection.Y; y < Map_Selection.Y + Map_Selection.Height; y++)
                    for (byte c = 0; c < Selected.Layer.Count; c++)
                        Selected.Layer[c].Tile[x, y] = new MapTileData();

            // Atualiza os azulejos Autos 
            Selected.Update();
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
                        if (x2 >= Map.Width) continue;
                        if (y2 >= Map.Height) continue;

                        // Cola
                        Selected.Layer[Layer].Tile[x2, y2] = Tiles_Copy.Data[c].Tile[x, y];
                    }

            // Atualiza os azulejos Autos 
            Selected.Update();
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
            for (int x = 0; x < Map.Width; x++)
                for (int y = 0; y < Map.Height; y++)
                    Selected.Layer[lstLayers.SelectedItems[0].Index].Tile[x, y] = Set_Tile();

            // Faz os cálculos da autocriação
            Selected.Update();
        }

        private void butEraser_Click(object sender, EventArgs e)
        {
            // Somente se necessário
            if (lstLayers.SelectedItems.Count == 0) return;

            // Reseta todos os azulejos
            for (int x = 0; x < Map.Width; x++)
                for (int y = 0; y < Map.Height; y++)
                    Selected.Layer[lstLayers.SelectedItems[0].Index].Tile[x, y] = new MapTileData();
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
            picMap.Image = null;
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
            picMap.Image = null;
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
            picMap.Image = null;
            Update_Map_Bounds();
        }

        private void Modes_Visibiliy()
        {
            // Desmarca todos os botões
            butMNormal.Checked = false;
            butMZones.Checked = false;
            butMLighting.Checked = false;
            butMAttributes.Checked = false;
            butMNPCs.Checked = false;
        }

        private void butMNormal_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibiliy();
            butMNormal.Checked = true;
        }

        private void butMAttributes_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibiliy();
            butMAttributes.Checked = true;
        }

        private void butMZones_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibiliy();
            butMZones.Checked = true;
        }

        private void butMLighting_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibiliy();
            butMLighting.Checked = true;
        }

        private void butMNPCs_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibiliy();
            butMNPCs.Checked = true;

            // Adiciona os NPCs e reseta os valores
            foreach (var NPC in NPC.List.Values) cmbNPC.Items.Add(NPC);
            cmbNPC.SelectedIndex = 0;
            numNPC_Zone.Value = 0;
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

        private void butEditors_Classes_Click(object sender, EventArgs e)
        {
            Editor_Classes.Form = new Editor_Classes();
        }

        private void butEditors_Data_Click(object sender, EventArgs e)
        {
            new Editor_Data();
        }

        private void butEditors_Interface_Click(object sender, EventArgs e)
        {
            Editor_Interface.Form = new Editor_Interface();
        }

        private void butEditors_Items_Click(object sender, EventArgs e)
        {
            Editor_Items.Form = new Editor_Items();
        }

        private void butEditors_NPCs_Click(object sender, EventArgs e)
        {
            Editor_NPCs.Form = new Editor_NPCs();
        }

        private void butEditors_Shops_Click(object sender, EventArgs e)
        {
            new Editor_Shops();
        }

        private void butEditors_Tiles_Click(object sender, EventArgs e)
        {
            Editor_Tiles.Form = new Editor_Tiles();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (NPC.List.Count == 0)
            {
                if (butMNPCs.Checked) butMNormal.Checked = true;
                butMNPCs.Enabled = false;
            }
            else butMNPCs.Enabled = true;

            // Ferramentas em geral
            if (butMNormal.Checked)
            {
                grpZones.Visible = false;
                grpLighting.Visible = false;
                grpAttributes.Visible = false;
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
            else if (butMLighting.Checked)
            {
                butPencil.Enabled = false;
                butRectangle.Enabled = false;
                butArea.Enabled = false;
                butDiscover.Enabled = false;
                butEraser.Enabled = false;
                butFill.Enabled = false;
                butVisualization.Enabled = false;
                butVisualization.Checked = true;
                butEdition.Enabled = false;
                butEdition.Checked = false;
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
                grpNPCs.Visible = false;
            }
            // Zonas
            if (butMZones.Checked)
            {
                grpZones.Visible = true;
                grpLighting.Visible = false;
                grpAttributes.Visible = false;
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

        #region Tile
        private void Update_Tile_Bounds()
        {
            Size Tile_Size = Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]);
            int Width = Tile_Size.Width - picTile.Width;
            int Height = Tile_Size.Height - picTile.Height;

            // Verifica se nada passou do limite minímo
            if (Width < 0) Width = 0;
            if (Height < 0) Height = 0;
            if (Width > 0) Width += Grid;
            if (Height > 0) Height += Grid;

            // Define os limites
            scrlTileX.Maximum = Width;
            scrlTileY.Maximum = Height;
        }

        // Altera o tamanho do azulejo selecionado
        private void Update_Tile_Selected() => Def_Tiles_Selection.Size = chkAuto.Checked ? new Size(2, 3) : new Size(1, 1);

        private void picTile_MouseWheel(object sender, MouseEventArgs e)
        {
            // Previne erros
            if (picTile_Background.Size != picTile.Size) return;
            if (scrlTileY.Maximum <= 1) return;

            // Movimenta para baixo
            if (e.Delta > 0)
                if (scrlTileY.Value - Grid > 0)
                    scrlTileY.Value -= Grid;
                else
                    scrlTileY.Value = 0;
            // Movimenta para cima
            else if (e.Delta < 0)
                if (scrlTileY.Value < scrlTileY.Maximum - Grid)
                    scrlTileY.Value += Grid;
                else
                    scrlTileY.Value = scrlTileY.Maximum - Grid;
        }

        private void picTile_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Previne erros
                if (e.X + scrlTileX.Value > Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]).Width) return;
                if (e.Y + scrlTileY.Value > Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]).Height) return;

                // Seleciona o azulejo;
                Def_Tiles_Selection.Location = new Point((e.X + scrlTileX.Value) / Grid, (e.Y + scrlTileY.Value) / Grid);
                Update_Tile_Selected();
            }
        }

        private void picTile_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (e.X + scrlTileX.Value) / Grid;
            int y = (e.Y + scrlTileY.Value) / Grid;
            Size Texture_Size = Graphics.TSize(Graphics.Tex_Tile[cmbTiles.SelectedIndex + 1]);

            // Define os valores
            Tile_Mouse = new Point(x * Grid - scrlTileX.Value, y * Grid - scrlTileY.Value);

            // Somente se necessário
            if (e.Button != MouseButtons.Left) return;
            if (chkAuto.Checked) return;

            // Verifica se não passou do limite
            if (x < 0) x = 0;
            if (x > Texture_Size.Width / Grid - 1) x = Texture_Size.Width / Grid - 1;
            if (y < 0) y = 0;
            if (y > Texture_Size.Height / Grid - 1) y = Texture_Size.Height / Grid - 1;

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
            picTile.Size = picTile_Background.Size;
            Update_Tile_Bounds();
        }

        private void picTile_MouseUp(object sender, MouseEventArgs e)
        {
            // Reseta o tamanho da tela
            picTile.Size = picTile_Background.Size;
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

        #region Map
        public void Update_Map_Bounds()
        {
            // Tamanho do scroll do mapa
            scrlMapX.Maximum = Math.Max(0, (Map.Width / Zoom() * Grid - picBackground.Width) / Grid);
            scrlMapY.Maximum = Math.Max(0, (Map.Height / Zoom() * Grid - picBackground.Height) / Grid);
            scrlMapX.Value = 0;
            scrlMapY.Value = 0;

            // Tamanho da tela do mapa
            picMap.Width = Math.Min(Map.Width * Grid, Map.Width / Zoom() * Grid);
            picMap.Height = Math.Min(Map.Height * Grid, Map.Height / Zoom() * Grid);
        }

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
            Point Tile_Dif = new Point(e.X - e.X / Grid * Grid, e.Y - e.Y / Grid * Grid);

            // Previne erros
            if (Map_Selection.X >= Map.Width || Map_Selection.Y >= Map.Height) return;

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
                for (byte i = 0; i < (byte)Directions.Count; i++)
                    if (Tile_Dif.X >= Block_Position(i).X && Tile_Dif.X <= Block_Position(i).X + 8)
                        if (Tile_Dif.Y >= Block_Position(i).Y && Tile_Dif.Y <= Block_Position(i).Y + 8)
                            // Altera o valor de bloqueio
                            Selected.Attribute[Map_Selection.X, Map_Selection.Y].Block[i] = !Selected.Attribute[Map_Selection.X, Map_Selection.Y].Block[i];
            }
            else if (butMAttributes.Checked && !optA_DirBlock.Checked)
                Set_Attribute(e);
            else if (butMZones.Checked)
            {
                // Define as zonas
                if (e.Button == MouseButtons.Left)
                    Selected.Attribute[Map_Selection.X, Map_Selection.Y].Zone = (byte)scrlZone.Value;
                else if (e.Button == MouseButtons.Right)
                    Selected.Attribute[Map_Selection.X, Map_Selection.Y].Zone = 0;
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

        private void Lighting_Remove()
        {
            // Encontra a luz que está nessa camada
            if (Selected.Light.Count > 0)
                for (byte i = 0; i < Selected.Light.Count; i++)
                    if (Selected.Light[i].X == Map_Selection.X)
                        if (Selected.Light[i].Y == Map_Selection.Y)
                            Selected.Light.RemoveAt(i);
        }

        private void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            Map_Pressed = false;

            // Somente se necessário
            if (e.Button != MouseButtons.Left) return;
            if (lstLayers.SelectedIndices.Count == 0) return;
            if (Map_Selection.X >= Map.Width || Map_Selection.Y >= Map.Height) return;

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
                            Selected.Layer[Layer].Tile[x, y] = Set_Tile();
                            Selected.Layer[Layer].Update(x, y);
                        }
            }
            // Iluminação
            else if (butMLighting.Checked)
                Selected.Light.Add(new Map_Light(Map_Selection));
            // Nenhum
            else
                return;

            // Reseta o tamanho da seleção
            Def_Map_Selection.Size = new Size(1, 1);
        }

        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            // Mouse
            Map_Mouse.X = e.X / Grid_Zoom + scrlMapX.Value;
            Map_Mouse.Y = e.Y / Grid_Zoom + scrlMapY.Value;

            // Impede que saia do limite da tela
            if (Map_Mouse.X < 0) Map_Mouse.X = 0;
            if (Map_Mouse.Y < 0) Map_Mouse.Y = 0;
            if (Map_Mouse.X >= Map.Width) Map_Mouse.X = Map.Width - 1;
            if (Map_Mouse.Y >= Map.Height) Map_Mouse.Y = Map.Height - 1;

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
                    Selected.Attribute[Map_Mouse.X, Map_Mouse.Y].Zone = (byte)scrlZone.Value;
                else if (e.Button == MouseButtons.Right)
                    Selected.Attribute[Map_Selection.X, Map_Selection.Y].Zone = 0;
            }
            else if (butMAttributes.Checked && !optA_DirBlock.Checked)
            {
                // Define as zonas
                if (e.Button == MouseButtons.Left)
                    Selected.Attribute[Map_Mouse.X, Map_Mouse.Y].Type = (byte)Attribute_Selected();
                else if (e.Button == MouseButtons.Right)
                    Selected.Attribute[Map_Selection.X, Map_Selection.Y].Type = 0;
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
            int x = e.X / Grid_Zoom + scrlMapX.Value, y = e.Y / Grid_Zoom + scrlMapY.Value;

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
            if (x >= Map.Width) x = Map.Width - 1;
            if (y < 0) y = 0;
            if (y >= Map.Height) y = Map.Height - 1;

            // Define o tamanho
            Def_Map_Selection.Width = x - Def_Map_Selection.X + 1;
            Def_Map_Selection.Height = y - Def_Map_Selection.Y + 1;
            Map_Pressed = true;
            return true;
        }

        private void Tile_Discover()
        {
            MapTileData Data;

            for (int c = Selected.Layer.Count - 1; c >= 0; c--)
            {
                Data = Selected.Layer[c].Tile[Map_Selection.X, Map_Selection.Y];

                // Somente se necessário
                if (!lstLayers.Items[c].Checked) continue;
                if (Data.Texture == 0) continue;

                // Define o azulejo
                cmbTiles.SelectedIndex = Data.Texture - 1;
                chkAuto.Checked = Data.IsAutotile;
                Def_Tiles_Selection = new Rectangle(Data.X, Data.Y, 1, 1);
                return;
            }
        }

        private MapTileData Set_Tile(byte x = 0, byte y = 0)
        {
            MapTileData Temp_Tile = new MapTileData();

            // Posição padrão
            if (x == 0) x = (byte)Tiles_Selection.X;
            if (y == 0) y = (byte)Tiles_Selection.Y;

            // Define os valores da camada
            Temp_Tile.Mini = new Point[4];
            Temp_Tile.X = x;
            Temp_Tile.Y = y;
            Temp_Tile.Texture = (byte)(cmbTiles.SelectedIndex + 1);
            Temp_Tile.IsAutotile = chkAuto.Checked;

            // Retorna o azulejo
            return Temp_Tile;
        }

        private void Tile_Set(byte Layer_Num)
        {
            // Define múltiplos azulejos
            if (Tiles_Selection.Width > 1 || Tiles_Selection.Height > 1)
                Tile_Set_Multiples(Layer_Num);

            // Defini um único azulejo
            Selected.Layer[Layer_Num].Tile[Map_Selection.X, Map_Selection.Y] = Set_Tile();
            Selected.Layer[Layer_Num].Update(Map_Selection.X, Map_Selection.Y);
        }

        private void Tile_Set_Multiples(byte Layer_Num)
        {
            byte x2 = 0, y2;

            // Apenas se necessário
            if (chkAuto.Checked) return;

            // Define todos os azulejos selecionados
            for (int x = Map_Selection.X; x < Map_Selection.X + Tiles_Selection.Width; x++)
            {
                y2 = 0;
                for (int y = Map_Selection.Y; y < Map_Selection.Y + Tiles_Selection.Height; y++)
                {
                    // Define os azulejos
                    if (!Selected.OutLimit((short)x, (short)y))
                    {
                        Selected.Layer[Layer_Num].Tile[x, y] = Set_Tile((byte)(Tiles_Selection.X + x2), (byte)(Tiles_Selection.Y + y2));
                        Selected.Layer[Layer_Num].Update(x, y);
                    }
                    y2++;
                }
                x2++;
            }
        }

        private void Tile_Clear(byte Layer_Num)
        {
            // Limpa a camada
            Selected.Layer[Layer_Num].Tile[Map_Selection.X, Map_Selection.Y] = new MapTileData();
            Selected.Layer[Layer_Num].Tile[Map_Selection.X, Map_Selection.Y].Mini = new Point[4];
            Selected.Layer[Layer_Num].Update(Map_Selection.X, Map_Selection.Y);
        }
        #endregion

        #region Layers
        private void chkAuto_CheckedChanged(object sender, EventArgs e)
        {
            Update_Tile_Selected();
        }

        private void Update_List_Layers()
        {
            // Limpa a lista
            lstLayers.Items.Clear();

            // Adiciona os itens à lista
            for (byte i = 0; i < Selected.Layer.Count; i++)
            {
                lstLayers.Items.Add(string.Empty);
                lstLayers.Items[i].BackColor = Color.FromArgb(60, 63, 65);
                lstLayers.Items[i].ForeColor = Color.White;
                lstLayers.Items[i].Checked = true;
                lstLayers.Items[i].SubItems.Add((i + 1).ToString());
                lstLayers.Items[i].SubItems.Add(Selected.Layer[i].Name);
                lstLayers.Items[i].SubItems.Add(((Layers)Selected.Layer[i].Type).ToString());
            }

            // Seleciona o primeiro item
            lstLayers.Items[0].Selected = true;
        }

        private void butLayer_Add_Click(object sender, EventArgs e)
        {
            MapLayer Layer = new MapLayer();

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
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    Layer.Tile[x, y] = new MapTileData();

            // Adiciona a camada
            Selected.Layer.Add(Layer);

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
                    MessageBox.Show("There is already a layer with this name.");
                    return;
                }

            // Define os dados
            Selected.Layer[lstLayers.SelectedItems[0].Index].Name = txtLayer_Name.Text;
            Selected.Layer[lstLayers.SelectedItems[0].Index].Type = (byte)cmbLayers_Type.SelectedIndex;

            // Atualiza a lista
            Update_Layers();
            grpLayer_Add.Visible = false;
        }

        public void Update_Layers()
        {
            List<MapLayer> Temp = new List<MapLayer>();

            // Reordena as camadas
            for (byte n = 0; n < (byte)Layers.Count; n++)
                for (byte i = 0; i < Selected.Layer.Count; i++)
                    if (Selected.Layer[i].Type == n)
                        Temp.Add(Selected.Layer[i]);

            // Atualiza os valores
            Selected.Layer = Temp;
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
            if (lstLayers.Items.Count == 1) return;
            if (lstLayers.SelectedItems.Count == 0) return;

            // Index
            int Index = Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

            // Remove a camada
            if (Index >= 0)
            {
                Selected.Layer.RemoveAt(Index);
                Update_List_Layers();
            }
        }

        private int Find_Layer(string Nome)
        {
            // Encontra a camada
            for (byte i = 0; i < Selected.Layer.Count; i++)
                if (Selected.Layer[i].Name == Nome)
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
            if (lstLayers.Items.Count == 1) return;
            if (lstLayers.SelectedItems.Count == 0) return;
            if (lstLayers.SelectedItems[0].Index == 0) return;

            // Dados
            List<MapLayer> Temp = new List<MapLayer>(Selected.Layer);
            int Layer_Num = lstLayers.SelectedItems[0].Index;

            if (Temp[Layer_Num - 1].Type == Temp[Layer_Num].Type)
            {
                // Altera as posições
                Temp[Layer_Num - 1] = Selected.Layer[Layer_Num];
                Temp[Layer_Num] = Selected.Layer[Layer_Num - 1];
                Selected.Layer = Temp;

                // Atualiza a lista
                Update_List_Layers();
            }
        }

        private void butLayers_Down_Click(object sender, EventArgs e)
        {
            // Somente se necessário
            if (lstLayers.Items.Count == 1) return;
            if (lstLayers.SelectedItems.Count == 0) return;
            if (lstLayers.SelectedItems[0].Index == lstLayers.Items.Count - 1) return;

            // Dados
            List<MapLayer> Temp = new List<MapLayer>(Selected.Layer);
            int Layer_Num = lstLayers.SelectedItems[0].Index;

            if (Temp[Layer_Num + 1].Type == Temp[Layer_Num].Type)
            {
                // Altera as posições
                Temp[Layer_Num + 1] = Selected.Layer[Layer_Num];
                Temp[Layer_Num] = Selected.Layer[Layer_Num + 1];
                Selected.Layer = Temp;

                // Atualiza a lista
                Update_List_Layers();
            }
        }

        private void butLayers_Edit_Click(object sender, EventArgs e)
        {
            // Previne erros
            if (lstLayers.SelectedItems.Count == 0) return;

            // Define os valores
            txtLayer_Name.Text = Selected.Layer[lstLayers.SelectedItems[0].Index].Name;
            cmbLayers_Type.SelectedIndex = Selected.Layer[lstLayers.SelectedItems[0].Index].Type;

            // Abre a janela em modo de edição
            butLayer_Add.Visible = false;
            butLayer_Edit.Visible = true;
            grpLayer_Add.Text = "Edit layer";
            grpLayer_Add.BringToFront();
            grpLayer_Add.Visible = true;
        }
        #endregion

        #region NPCs
        private void AddNPC(bool FixedSpawn = false, byte X = 0, byte Y = 0)
        {
            // Define os dados
            Map_NPC Data = new Map_NPC
            {
                NPC = (NPC)cmbNPC.SelectedItem,
                Zone = (byte)numNPC_Zone.Value,
                Spawn = FixedSpawn,
                X = X,
                Y = Y
            };

            // Adiciona o NPC
            Selected.NPC.Add(Data);
        }
        private void butNPC_Remove_Click(object sender, EventArgs e)
        {
            // Remove o NPC
            if (lstNPC.SelectedIndex >= 0) Selected.NPC.RemoveAt(lstNPC.SelectedIndex);
        }

        private void butNPC_Clear_Click(object sender, EventArgs e)
        {
            // Limpa todos os NPCs do mapa
            Selected.NPC.Clear();
        }

        private void butNPC_Add_Click(object sender, EventArgs e)
        {
            // Adiciona o NPC
            AddNPC();
        }
        #endregion

        #region Attributes
        private void Set_Attribute(MouseEventArgs e)
        {
            // Define os Atributos
            if (e.Button == MouseButtons.Left)
            {
                Selected.Attribute[Map_Selection.X, Map_Selection.Y].Data_1 = AData_1;
                Selected.Attribute[Map_Selection.X, Map_Selection.Y].Data_2 = AData_2;
                Selected.Attribute[Map_Selection.X, Map_Selection.Y].Data_3 = AData_3;
                Selected.Attribute[Map_Selection.X, Map_Selection.Y].Data_4 = AData_4;
                Selected.Attribute[Map_Selection.X, Map_Selection.Y].Type = (byte)Attribute_Selected();
            }
            // Limpa os dados
            else if (e.Button == MouseButtons.Right)
                Selected.Attribute[Map_Selection.X, Map_Selection.Y] = new Map_Attribute();
        }

        private void butAttributes_Clear_Click(object sender, EventArgs e)
        {
            // Reseta todas os atributos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    Selected.Attribute[x, y] = new Map_Attribute();
        }

        private void butAttributes_Import_Click(object sender, EventArgs e)
        {
            // Importa os dados padrões dos azulejos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    for (byte c = 0; c < Selected.Layer.Count; c++)
                    {
                        // Dados do azulejo
                        MapTileData Data = Selected.Layer[c].Tile[x, y];

                        if (Data.Texture > 0)
                        {
                            // Atributos
                            if (Lists.Tile[Data.Texture].Data[Data.X, Data.Y].Attribute > 0)
                                Selected.Attribute[x, y].Type = Lists.Tile[Data.Texture].Data[Data.X, Data.Y].Attribute;

                            // Bloqueio direcional
                            for (byte b = 0; b < (byte)Directions.Count; b++)
                                if (Lists.Tile[Data.Texture].Data[Data.X, Data.Y].Block[b])
                                    Selected.Attribute[x, y].Block[b] = Lists.Tile[Data.Texture].Data[Data.X, Data.Y].Block[b];
                        }
                    }
        }

        private Tile_Attributes Attribute_Selected()
        {
            // Retorna com o atributo selecionado
            if (optA_Block.Checked) return Tile_Attributes.Block;
            else if (optA_Warp.Checked) return Tile_Attributes.Warp;
            else if (optA_Item.Checked) return Tile_Attributes.Item;
            else return Tile_Attributes.None;
        }

        private void optA_Warp_CheckedChanged(object sender, EventArgs e)
        {
            // Reseta os valores
            cmbA_Warp_Map.SelectedIndex = 0;
            cmbA_Warp_Direction.SelectedIndex = 0;
            numA_Warp_X.Value = 0;
            numA_Warp_Y.Value = 0;
            cmbA_Warp_Direction.SelectedIndex = 0;
            grpA_Warp.Visible = optA_Warp.Checked;
        }

        private void optA_Item_CheckedChanged(object sender, EventArgs e)
        {
            // Verifica se é possível usar o atributo
            if (optA_Item.Checked)
            {
                if (Item.List.Count == 0)
                {
                    MessageBox.Show("It must have at least one item registered to use this attribute.");
                    optA_Block.Checked = true;
                    return;
                }

                // Adiciona os itens
                cmbA_Item.Items.Clear();
                foreach (var Item in Item.List.Values) cmbA_Item.Items.Add(Item);
                cmbA_Item.SelectedIndex = 0;
                numA_Item_Amount.Value = AData_2 = 1;
            }
            grpA_Item.Visible = optA_Item.Checked;
        }

        private void cmbA_Warp_Map_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reseta os valores
            var Warp_Map = (Map)cmbA_Warp_Map.SelectedItem;
            AData_1 = Warp_Map.GetID();
        }

        private void numA_Warp_X_ValueChanged(object sender, EventArgs e)
        {
            AData_2 = (short)numA_Warp_X.Value;
        }

        private void numA_Warp_Y_ValueChanged(object sender, EventArgs e)
        {
            AData_3 = (short)numA_Warp_Y.Value;
        }

        private void cmbA_Warp_Direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            AData_4 = (short)cmbA_Warp_Direction.SelectedIndex;
        }

        private void cmbA_Item_SelectedIndexChanged(object sender, EventArgs e)
        {
            AData_1 = ((Item)cmbA_Item.SelectedItem).GetID();
        }

        private void numA_Item_Amount_ValueChanged(object sender, EventArgs e)
        {
            AData_2 = (short)numA_Item_Amount.Value;
        }
        #endregion

        #region Zones
        private void scrlZone_Clear_Click(object sender, EventArgs e)
        {
            // Reseta todas as zonas
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    Selected.Attribute[x, y].Zone = 0;
        }

        private void scrlZone_ValueChanged(object sender, EventArgs e)
        {
            // Atualiza os valores
            if (scrlZone.Value == 0)
                grpZones.Text = "Zone: None";
            else
                grpZones.Text = "Zone: " + scrlZone.Value;
        }
        #endregion

        #region Lighting
        private void butLight_Clear_Click(object sender, EventArgs e)
        {
            // Reseta todas as zonas
            Selected.Light = new List<Map_Light>();
        }
        #endregion

        #region Utils
        // Usado pra movimentar as seleções
        private Rectangle Selection_Rec(Rectangle Temp)
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

        // Retângulo da seleção de azulejos
        private Rectangle Tiles_Selection => Selection_Rec(Def_Tiles_Selection);

        // Retângulo do mapa
        public Rectangle Map_Selection
        {
            get
            {
                if (chkAuto.Checked) return new Rectangle(Map_Mouse, new Size(1, 1));
                if (butMNormal.Checked && butPencil.Checked) return new Rectangle(Map_Mouse, Tiles_Selection.Size);
                return Selection_Rec(Def_Map_Selection);
            }
        }

        // Retorna com o retângulo do azulejo ajustado à grade
        public Rectangle Tile_Source => new Rectangle(Tiles_Selection.X * Grid, Tiles_Selection.Y * Grid, Tiles_Selection.Width * Grid, Tiles_Selection.Height * Grid);

        // Retorna o valor com o zoom
        public byte Zoom()
        {
            // Retorna o valor do zoom
            if (butZoom_2x.Checked) return 2;
            if (butZoom_4x.Checked) return 4;
            return 1;
        }

        public byte Grid_Zoom => (byte)(Grid / Zoom());
        public Rectangle Zoom(Rectangle Value) => new Rectangle(Value.X / Zoom(), Value.Y / Zoom(), Value.Width / Zoom(), Value.Height / Zoom());
        public Rectangle Zoom_Grid(Rectangle Rectangle) => new Rectangle(Rectangle.X * Grid_Zoom, Rectangle.Y * Grid_Zoom, Rectangle.Width * Grid_Zoom, Rectangle.Height * Grid_Zoom);
        public Point Zoom_Grid(int X, int Y) => new Point(X * Grid_Zoom, Y * Grid_Zoom);

        // Tamanho da grade com o zoom
        // public static byte Grid_Zoom => (byte)(Grid / Editor_Maps.Form.Zoom());
        //public static Point Zoom(int X, int Y) => new Point(X * Grid_Zoom, Y * Grid_Zoom);
        #endregion
    }
}