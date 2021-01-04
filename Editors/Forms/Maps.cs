using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Library;
using CryBits.Editors.Logic;
using CryBits.Editors.Media;
using CryBits.Editors.Media.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Enums;
using DarkUI.Forms;
using SFML.Graphics;
using static CryBits.Globals;
using static CryBits.Editors.Logic.Options;
using static CryBits.Editors.Logic.Utils;
using Color = System.Drawing.Color;
using Music = CryBits.Editors.Media.Audio.Music;
using Sound = CryBits.Editors.Media.Audio.Sound;

namespace CryBits.Editors.Forms
{
    internal partial class EditorMaps : DarkForm
    {
        #region Data
        // Usado para acessar os dados da janela
        public static EditorMaps Form;

        // Mapa selecionado
        public Map Selected;

        // Dados temporários
        private bool _mapPressed;

        // Posição do mouse
        public Point TileMouse { get; set; }
        private Point _mapMouse;

        // Seleção retângular
        private Rectangle _defTilesSelection = new(0, 0, 1, 1);
        private Rectangle _defMapSelection = new(0, 0, 1, 1);

        // Dados dos atributos
        private string _aData1;
        private short _aData2;
        private short _aData3;
        private short _aData4;

        // Azulejos copiados
        private CopyStruct _tilesCopy;

        public struct CopyStruct
        {
            public Rectangle Area;
            public MapLayer[] Data;
        }
        #endregion

        #region Base
        public EditorMaps()
        {
            // Inicializa os componentes
            InitializeComponent();

            // Abre a janela
            Login.Form.Visible = false;
            Show();
        }

        private void Editor_Maps_Load(object sender, EventArgs e)
        {
            Graphicss.WinMap = new RenderWindow(picMap.Handle);
            Graphicss.WinMapTile = new RenderWindow(picTile.Handle);
            Graphicss.WinMapLighting = new RenderTexture((uint)Width, (uint)Height);

            // Lista os dados
            for (byte i = 0; i < (byte)Layer.Count; i++) cmbLayers_Type.Items.Add(((Layer)i).ToString());
            for (byte i = 1; i < Textures.Tiles.Count; i++) cmbTiles.Items.Add(i.ToString());
            Update_List();

            // Define os limites
            scrlZone.Maximum = MaxZones;
            numNPC_Zone.Maximum = MaxZones;
            numA_Warp_X.Maximum = Map.Width - 1;
            numA_Warp_Y.Maximum = Map.Height - 1;

            // Reseta os valores
            grpAttributes.BringToFront();
            grpZones.BringToFront();
            grpNPCs.BringToFront();
            grpLighting.BringToFront();
            cmbTiles.SelectedIndex = 0;
            cmbLayers_Type.SelectedIndex = 0;
            butGrid.Checked = PreMapGrid;
            butAudio.Checked = PreMapAudio;
            if (!PreMapView)
            {
                butVisualization.Checked = false;
                butEdition.Checked = true;
            }
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
            foreach (var map in Map.List.Values)
            {
                if (map.Name.StartsWith(txtFilter.Text))
                {
                    List.Nodes.Add(map.Name);
                    List.Nodes[List.Nodes.Count - 1].Tag = map.ID;
                }
                cmbA_Warp_Map.Items.Add(map);
            }

            // Seleciona o primeiro item
            if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
        }

        private void List_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Selected = Map.List[(Guid)List.SelectedNode.Tag];

            // Conecta as listas com os componentes
            prgProperties.SelectedObject = new MapProperties(Selected);
            lstNPC.Tag = Selected.Npc;

            // Atualiza as listas
            lstNPC.UpdateData();

            // Reseta o clima
            TempMap.UpdateWeatherType();

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
            Map @new = new Map();
            Map.List.Add(@new.ID, @new);
            cmbA_Warp_Map.Items.Add(@new);
            @new.Name = "New map";
            @new.Layer.Add(new MapLayer());
            @new.Layer[0].Name = "Ground";

            // Azulejos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                {
                    @new.Layer[0].Tile[x, y] = new MapTileData();
                    @new.Attribute[x, y] = new MapAttribute();
                }

            // Adiciona na lista
            TreeNode node = new TreeNode(@new.Name);
            node.Tag = @new.ID;
            List.Nodes.Add(node);
            List.SelectedNode = node;
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
            Strip.Items[4].Text = "Position: {" + _mapMouse.X + ";" + _mapMouse.Y + "}";
        }
        #endregion

        #region Toolbox
        private void butSaveAll_Click(object sender, EventArgs e)
        {
            // Salva todos os dados
            foreach (Map map in Map.List.Values)
                ++map.Revision;
            Send.WriteMaps();
            MessageBox.Show("All maps has been saved");
        }

        private void butReload_Click(object sender, EventArgs e)
        {
            // Recarrega o mapa
            Send.RequestMap(Selected);
            Update_List_Layers();
            Selected.Update();
        }

        private void Copy()
        {
            _tilesCopy.Data = new MapLayer[Selected.Layer.Count];

            // Seleção
            _tilesCopy.Area = MapSelection;

            // Copia os dados das camadas
            for (byte c = 0; c < _tilesCopy.Data.Length; c++)
            {
                _tilesCopy.Data[c] = new MapLayer();
                _tilesCopy.Data[c].Name = Selected.Layer[c].Name;

                // Copia os dados dos azulejos
                for (byte x = 0; x < Map.Width; x++)
                    for (byte y = 0; y < Map.Height; y++)
                        _tilesCopy.Data[c].Tile[x, y] = Selected.Layer[c].Tile[x, y];
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
            for (int x = MapSelection.X; x < MapSelection.X + MapSelection.Width; x++)
                for (int y = MapSelection.Y; y < MapSelection.Y + MapSelection.Height; y++)
                    for (byte c = 0; c < Selected.Layer.Count; c++)
                        Selected.Layer[c].Tile[x, y] = new MapTileData();

            // Atualiza os azulejos Autos 
            Selected.Update();
        }

        private void butPaste_Click(object sender, EventArgs e)
        {
            // Cola os azulejos
            for (int x = _tilesCopy.Area.X; x < _tilesCopy.Area.X + _tilesCopy.Area.Width; x++)
                for (int y = _tilesCopy.Area.Y; y < _tilesCopy.Area.Y + _tilesCopy.Area.Height; y++)
                    for (byte c = 0; c < _tilesCopy.Data.Length; c++)
                    {
                        // Dados
                        int layer = Find_Layer(_tilesCopy.Data[c].Name);
                        int x2 = MapSelection.X + x - _tilesCopy.Area.X;
                        int y2 = y + MapSelection.Y - _tilesCopy.Area.Y;

                        // Previne erros
                        if (layer < 0) continue;
                        if (x2 >= Map.Width) continue;
                        if (y2 >= Map.Height) continue;

                        // Cola
                        Selected.Layer[layer].Tile[x2, y2] = _tilesCopy.Data[c].Tile[x, y];
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
            _defMapSelection.Size = new Size(1, 1);
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
            _defMapSelection.Size = new Size(1, 1);
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
            _defMapSelection.Size = new Size(1, 1);
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
            _defMapSelection.Size = new Size(1, 1);
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
            PreMapView = butVisualization.Checked;
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
            PreMapView = butVisualization.Checked;
            Write.Options();
        }

        private void butGrids_Click(object sender, EventArgs e)
        {
            // Salva a preferência
            PreMapGrid = butGrid.Checked;
            Write.Options();
        }

        private void butAudio_Click(object sender, EventArgs e)
        {
            // Salva a preferência
            PreMapAudio = butAudio.Checked;
            Write.Options();

            // Desativa os áudios
            if (!butAudio.Checked)
            {
                Music.Stop();
                Sound.StopAll();
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

        private void Modes_Visibility()
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
            Modes_Visibility();
            butMNormal.Checked = true;
        }

        private void butMAttributes_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibility();
            butMAttributes.Checked = true;
        }

        private void butMZones_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibility();
            butMZones.Checked = true;
        }

        private void butMLighting_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibility();
            butMLighting.Checked = true;
        }

        private void butMNPCs_Click(object sender, EventArgs e)
        {
            // Marca o botão
            Modes_Visibility();
            butMNPCs.Checked = true;

            // Adiciona os NPCs e reseta os valores
            foreach (var npc in Npc.List.Values) cmbNPC.Items.Add(npc);
            cmbNPC.SelectedIndex = 0;
            numNPC_Zone.Value = 0;
        }

        private void butMNormal_CheckedChanged(object sender, EventArgs e)
        {
            _defMapSelection.Size = new Size(1, 1);
        }

        private void butMLighting_CheckedChanged(object sender, EventArgs e)
        {
            _defMapSelection.Size = new Size(1, 1);
        }

        private void butMZones_CheckedChanged(object sender, EventArgs e)
        {
            _defMapSelection.Size = new Size(1, 1);
        }

        private void butEditors_Classes_Click(object sender, EventArgs e)
        {
            EditorClasses.Form = new EditorClasses();
        }

        private void butEditors_Data_Click(object sender, EventArgs e)
        {
            new EditorData();
        }

        private void butEditors_Interface_Click(object sender, EventArgs e)
        {
            EditorInterface.Form = new EditorInterface();
        }

        private void butEditors_Items_Click(object sender, EventArgs e)
        {
            EditorItems.Form = new EditorItems();
        }

        private void butEditors_NPCs_Click(object sender, EventArgs e)
        {
            EditorNpcs.Form = new EditorNpcs();
        }

        private void butEditors_Shops_Click(object sender, EventArgs e)
        {
            new EditorShops();
        }

        private void butEditors_Tiles_Click(object sender, EventArgs e)
        {
            EditorTiles.Form = new EditorTiles();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            if (Npc.List.Count == 0)
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
            if (_tilesCopy.Data == null) butPaste.Enabled = false;

            // Atualiza os dados da faixa
            Update_Strip();
        }
        #endregion

        #region Tile
        private void Update_Tile_Bounds()
        {
            Size tileSize = Graphicss.Size(Textures.Tiles[cmbTiles.SelectedIndex + 1]);
            int width = tileSize.Width - picTile.Width;
            int height = tileSize.Height - picTile.Height;

            // Verifica se nada passou do limite minímo
            if (width < 0) width = 0;
            if (height < 0) height = 0;
            if (width > 0) width += Grid;
            if (height > 0) height += Grid;

            // Define os limites
            scrlTileX.Maximum = width;
            scrlTileY.Maximum = height;
        }

        // Altera o tamanho do azulejo selecionado
        private void Update_Tile_Selected() => _defTilesSelection.Size = chkAuto.Checked ? new Size(2, 3) : new Size(1, 1);

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
                if (e.X + scrlTileX.Value > Graphicss.Size(Textures.Tiles[cmbTiles.SelectedIndex + 1]).Width) return;
                if (e.Y + scrlTileY.Value > Graphicss.Size(Textures.Tiles[cmbTiles.SelectedIndex + 1]).Height) return;

                // Seleciona o azulejo;
                _defTilesSelection.Location = new Point((e.X + scrlTileX.Value) / Grid, (e.Y + scrlTileY.Value) / Grid);
                Update_Tile_Selected();
            }
        }

        private void picTile_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (e.X + scrlTileX.Value) / Grid;
            int y = (e.Y + scrlTileY.Value) / Grid;
            Size textureSize = Graphicss.Size(Textures.Tiles[cmbTiles.SelectedIndex + 1]);

            // Define os valores
            TileMouse = new Point(x * Grid - scrlTileX.Value, y * Grid - scrlTileY.Value);

            // Somente se necessário
            if (e.Button != MouseButtons.Left) return;
            if (chkAuto.Checked) return;

            // Verifica se não passou do limite
            if (x < 0) x = 0;
            if (x > textureSize.Width / Grid - 1) x = textureSize.Width / Grid - 1;
            if (y < 0) y = 0;
            if (y > textureSize.Height / Grid - 1) y = textureSize.Height / Grid - 1;

            // Tamanho da grade
            _defTilesSelection.Width = x - _defTilesSelection.X + 1;
            _defTilesSelection.Height = y - _defTilesSelection.Y + 1;

            // Altera o tamanho da tela de azulejos
            if (picTile.Width < textureSize.Width - scrlTileX.Value) picTile.Width = textureSize.Width - scrlTileX.Value;
            if (picTile.Height < textureSize.Height - scrlTileY.Value) picTile.Height = textureSize.Height - scrlTileY.Value;
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
            TileMouse = new Point(0);
            _defTilesSelection = new Rectangle(0, 0, 1, 1);
            _defMapSelection.Size = new Size(1, 1);
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
            if (Graphicss.WinMap == null) return;
            Graphicss.WinMap.Dispose();
            Graphicss.WinMap = new RenderWindow(picMap.Handle);
            Graphicss.WinMapLighting.Dispose();
            Graphicss.WinMapLighting = new RenderTexture((uint)picMap.Width, (uint)picMap.Height);
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
            Point tileDif = new Point(e.X - e.X / Grid * Grid, e.Y - e.Y / Grid * Grid);

            // Previne erros
            if (MapSelection.X >= Map.Width || MapSelection.Y >= Map.Height) return;

            // Executa um evento de acordo com a ferramenta selecionada
            if (butMNormal.Checked)
            {
                Tile_Events(e.Button);

                // Ferramentas
                if (butArea.Checked) _defMapSelection = new Rectangle(_mapMouse, new Size(1, 1));
            }
            else if (butMAttributes.Checked && optA_DirBlock.Checked)
            {
                // Define o bloqueio direcional
                for (byte i = 0; i < (byte)Direction.Count; i++)
                    if (tileDif.X >= Block_Position(i).X && tileDif.X <= Block_Position(i).X + 8)
                        if (tileDif.Y >= Block_Position(i).Y && tileDif.Y <= Block_Position(i).Y + 8)
                            // Altera o valor de bloqueio
                            Selected.Attribute[MapSelection.X, MapSelection.Y].Block[i] = !Selected.Attribute[MapSelection.X, MapSelection.Y].Block[i];
            }
            else if (butMAttributes.Checked && !optA_DirBlock.Checked)
                Set_Attribute(e);
            else if (butMZones.Checked)
            {
                // Define as zonas
                if (e.Button == MouseButtons.Left)
                    Selected.Attribute[MapSelection.X, MapSelection.Y].Zone = (byte)scrlZone.Value;
                else if (e.Button == MouseButtons.Right)
                    Selected.Attribute[MapSelection.X, MapSelection.Y].Zone = 0;
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
                    AddNPC(true, (byte)MapSelection.X, (byte)MapSelection.Y);
        }

        private void Lighting_Remove()
        {
            // Encontra a luz que está nessa camada
            if (Selected.Light.Count > 0)
                for (byte i = 0; i < Selected.Light.Count; i++)
                    if (Selected.Light[i].X == MapSelection.X)
                        if (Selected.Light[i].Y == MapSelection.Y)
                            Selected.Light.RemoveAt(i);
        }

        private void picMap_MouseUp(object sender, MouseEventArgs e)
        {
            _mapPressed = false;

            // Somente se necessário
            if (e.Button != MouseButtons.Left) return;
            if (lstLayers.SelectedIndices.Count == 0) return;
            if (MapSelection.X >= Map.Width || MapSelection.Y >= Map.Height) return;

            // Camada selecionada
            byte layer = (byte)Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

            // Retângulo
            if (butRectangle.Checked)
            {
                if (MapSelection.Width > 1 || MapSelection.Height > 1)
                    // Define os azulejos
                    for (int x = MapSelection.X; x < MapSelection.X + MapSelection.Width; x++)
                        for (int y = MapSelection.Y; y < MapSelection.Y + MapSelection.Height; y++)
                        {
                            Selected.Layer[layer].Tile[x, y] = Set_Tile();
                            Selected.Layer[layer].Update(x, y);
                        }
            }
            // Iluminação
            else if (butMLighting.Checked)
                Selected.Light.Add(new MapLight(MapSelection));
            // Nenhum
            else
                return;

            // Reseta o tamanho da seleção
            _defMapSelection.Size = new Size(1, 1);
        }

        private void picMap_MouseMove(object sender, MouseEventArgs e)
        {
            // Mouse
            _mapMouse.X = e.X / GridZoom + scrlMapX.Value;
            _mapMouse.Y = e.Y / GridZoom + scrlMapY.Value;

            // Impede que saia do limite da tela
            if (_mapMouse.X < 0) _mapMouse.X = 0;
            if (_mapMouse.Y < 0) _mapMouse.Y = 0;
            if (_mapMouse.X >= Map.Width) _mapMouse.X = Map.Width - 1;
            if (_mapMouse.Y >= Map.Height) _mapMouse.Y = Map.Height - 1;

            // Cria um retângulo
            if (Map_Rectangle(e)) return;

            // Não mover e nem executar nada caso for a ferramenta de área 
            if (butArea.Checked && butArea.Enabled) return;

            // Define a posição da seleção
            _defMapSelection.Location = _mapMouse;

            // Executa um evento de acordo com a ferramenta selecionada
            if (butMNormal.Checked)
                Tile_Events(e.Button);
            else if (butMZones.Checked)
            {
                // Define as zonas
                if (e.Button == MouseButtons.Left)
                    Selected.Attribute[_mapMouse.X, _mapMouse.Y].Zone = (byte)scrlZone.Value;
                else if (e.Button == MouseButtons.Right)
                    Selected.Attribute[MapSelection.X, MapSelection.Y].Zone = 0;
            }
            else if (butMAttributes.Checked && !optA_DirBlock.Checked)
            {
                // Define as zonas
                if (e.Button == MouseButtons.Left)
                    Selected.Attribute[_mapMouse.X, _mapMouse.Y].Type = (byte)Attribute_Selected();
                else if (e.Button == MouseButtons.Right)
                    Selected.Attribute[MapSelection.X, MapSelection.Y].Type = 0;
            }
        }

        private void Tile_Events(MouseButtons e)
        {
            // Previne erros
            if (lstLayers.SelectedIndices.Count == 0) return;

            // Camada selecionada
            byte layer = (byte)Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

            // Executa um evento de acordo com a ferramenta selecionada
            if (e == MouseButtons.Left)
            {
                if (butPencil.Checked) Tile_Set(layer);
                if (butDiscover.Checked) Tile_Discover();
            }
            else if (e == MouseButtons.Right)
            {
                if (butPencil.Checked) Tile_Clear(layer);
            }
        }

        private bool Map_Rectangle(MouseEventArgs e)
        {
            int x = e.X / GridZoom + scrlMapX.Value, y = e.Y / GridZoom + scrlMapY.Value;

            // Somente se necessário
            if (e.Button != MouseButtons.Left) return false;
            if (butMLighting.Checked) goto Continuation;
            if (butRectangle.Checked && butRectangle.Enabled) goto Continuation;
            if (butArea.Checked && butArea.Enabled) goto Continuation;
            return false;
        Continuation:

            // Cria um retângulo
            if (!_mapPressed) _defMapSelection.Size = new Size(1, 1);

            // Verifica se não passou do limite
            if (x < 0) x = 0;
            if (x >= Map.Width) x = Map.Width - 1;
            if (y < 0) y = 0;
            if (y >= Map.Height) y = Map.Height - 1;

            // Define o tamanho
            _defMapSelection.Width = x - _defMapSelection.X + 1;
            _defMapSelection.Height = y - _defMapSelection.Y + 1;
            _mapPressed = true;
            return true;
        }

        private void Tile_Discover()
        {
            MapTileData data;

            for (int c = Selected.Layer.Count - 1; c >= 0; c--)
            {
                data = Selected.Layer[c].Tile[MapSelection.X, MapSelection.Y];

                // Somente se necessário
                if (!lstLayers.Items[c].Checked) continue;
                if (data.Texture == 0) continue;

                // Define o azulejo
                cmbTiles.SelectedIndex = data.Texture - 1;
                chkAuto.Checked = data.IsAutoTile;
                _defTilesSelection = new Rectangle(data.X, data.Y, 1, 1);
                return;
            }
        }

        private MapTileData Set_Tile(byte x = 0, byte y = 0)
        {
            MapTileData tempTile = new MapTileData();

            // Posição padrão
            if (x == 0) x = (byte)TilesSelection.X;
            if (y == 0) y = (byte)TilesSelection.Y;

            // Define os valores da camada
            tempTile.Mini = new Point[4];
            tempTile.X = x;
            tempTile.Y = y;
            tempTile.Texture = (byte)(cmbTiles.SelectedIndex + 1);
            tempTile.IsAutoTile = chkAuto.Checked;

            // Retorna o azulejo
            return tempTile;
        }

        private void Tile_Set(byte layerNum)
        {
            // Define múltiplos azulejos
            if (TilesSelection.Width > 1 || TilesSelection.Height > 1)
                Tile_Set_Multiples(layerNum);

            // Defini um único azulejo
            Selected.Layer[layerNum].Tile[MapSelection.X, MapSelection.Y] = Set_Tile();
            Selected.Layer[layerNum].Update(MapSelection.X, MapSelection.Y);
        }

        private void Tile_Set_Multiples(byte layerNum)
        {
            byte x2 = 0, y2;

            // Apenas se necessário
            if (chkAuto.Checked) return;

            // Define todos os azulejos selecionados
            for (int x = MapSelection.X; x < MapSelection.X + TilesSelection.Width; x++)
            {
                y2 = 0;
                for (int y = MapSelection.Y; y < MapSelection.Y + TilesSelection.Height; y++)
                {
                    // Define os azulejos
                    if (!Map.OutLimit((short)x, (short)y))
                    {
                        Selected.Layer[layerNum].Tile[x, y] = Set_Tile((byte)(TilesSelection.X + x2), (byte)(TilesSelection.Y + y2));
                        Selected.Layer[layerNum].Update(x, y);
                    }
                    y2++;
                }
                x2++;
            }
        }

        private void Tile_Clear(byte layerNum)
        {
            // Limpa a camada
            Selected.Layer[layerNum].Tile[MapSelection.X, MapSelection.Y] = new MapTileData();
            Selected.Layer[layerNum].Tile[MapSelection.X, MapSelection.Y].Mini = new Point[4];
            Selected.Layer[layerNum].Update(MapSelection.X, MapSelection.Y);
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
                lstLayers.Items[i].SubItems.Add(((Layer)Selected.Layer[i].Type).ToString());
            }

            // Seleciona o primeiro item
            if (Selected.Layer.Count > 0) lstLayers.Items[0].Selected = true;
        }

        private void butLayer_Add_Click(object sender, EventArgs e)
        {
            MapLayer layer = new MapLayer();

            // Verifica se o nome é válido
            if (txtLayer_Name.Text.Length < 1 || txtLayer_Name.Text.Length > 12) return;
            if (Find_Layer(txtLayer_Name.Text) >= 0)
            {
                MessageBox.Show("There is already a layer with that name.");
                return;
            }

            // Define os dados
            layer.Name = txtLayer_Name.Text;
            layer.Type = (byte)cmbLayers_Type.SelectedIndex;
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    layer.Tile[x, y] = new MapTileData();

            // Adiciona a camada
            Selected.Layer.Add(layer);

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
            List<MapLayer> temp = new List<MapLayer>();

            // Reordena as camadas
            for (byte n = 0; n < (byte)Layer.Count; n++)
                for (byte i = 0; i < Selected.Layer.Count; i++)
                    if (Selected.Layer[i].Type == n)
                        temp.Add(Selected.Layer[i]);

            // Atualiza os valores
            Selected.Layer = temp;
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
            int index = Find_Layer(lstLayers.SelectedItems[0].SubItems[2].Text);

            // Remove a camada
            if (index >= 0)
            {
                Selected.Layer.RemoveAt(index);
                Update_List_Layers();
            }
        }

        private int Find_Layer(string nome)
        {
            // Encontra a camada
            for (byte i = 0; i < Selected.Layer.Count; i++)
                if (Selected.Layer[i].Name == nome)
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
            List<MapLayer> temp = new List<MapLayer>(Selected.Layer);
            int layerNum = lstLayers.SelectedItems[0].Index;

            if (temp[layerNum - 1].Type == temp[layerNum].Type)
            {
                // Altera as posições
                temp[layerNum - 1] = Selected.Layer[layerNum];
                temp[layerNum] = Selected.Layer[layerNum - 1];
                Selected.Layer = temp;

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
            List<MapLayer> temp = new List<MapLayer>(Selected.Layer);
            int layerNum = lstLayers.SelectedItems[0].Index;

            if (temp[layerNum + 1].Type == temp[layerNum].Type)
            {
                // Altera as posições
                temp[layerNum + 1] = Selected.Layer[layerNum];
                temp[layerNum] = Selected.Layer[layerNum + 1];
                Selected.Layer = temp;

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
        private void AddNPC(bool fixedSpawn = false, byte x = 0, byte y = 0)
        {
            // Define os dados
            MapNpc data = new MapNpc
            {
                Npc = (Npc)cmbNPC.SelectedItem,
                Zone = (byte)numNPC_Zone.Value,
                Spawn = fixedSpawn,
                X = x,
                Y = y
            };

            // Adiciona o NPC
            Selected.Npc.Add(data);
            lstNPC.UpdateData();
        }

        private void butNPC_Remove_Click(object sender, EventArgs e)
        {
            // Remove o NPC
            if (lstNPC.SelectedIndex >= 0)
            {
                Selected.Npc.RemoveAt(lstNPC.SelectedIndex);
                lstNPC.UpdateData();
            }
        }

        private void butNPC_Clear_Click(object sender, EventArgs e)
        {
            // Limpa todos os NPCs do mapa
            Selected.Npc.Clear();
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
                Selected.Attribute[MapSelection.X, MapSelection.Y].Data1 = _aData1;
                Selected.Attribute[MapSelection.X, MapSelection.Y].Data2 = _aData2;
                Selected.Attribute[MapSelection.X, MapSelection.Y].Data3 = _aData3;
                Selected.Attribute[MapSelection.X, MapSelection.Y].Data4 = _aData4;
                Selected.Attribute[MapSelection.X, MapSelection.Y].Type = (byte)Attribute_Selected();
            }
            // Limpa os dados
            else if (e.Button == MouseButtons.Right)
                Selected.Attribute[MapSelection.X, MapSelection.Y] = new MapAttribute();
        }

        private void butAttributes_Clear_Click(object sender, EventArgs e)
        {
            // Reseta todas os atributos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    Selected.Attribute[x, y] = new MapAttribute();
        }

        private void butAttributes_Import_Click(object sender, EventArgs e)
        {
            // Importa os dados padrões dos azulejos
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    for (byte c = 0; c < Selected.Layer.Count; c++)
                    {
                        // Dados do azulejo
                        MapTileData data = Selected.Layer[c].Tile[x, y];

                        if (data.Texture > 0)
                        {
                            // Atributos
                            if (Tile.List[data.Texture].Data[data.X, data.Y].Attribute > 0)
                                Selected.Attribute[x, y].Type = Tile.List[data.Texture].Data[data.X, data.Y].Attribute;

                            // Bloqueio direcional
                            for (byte b = 0; b < (byte)Direction.Count; b++)
                                if (Tile.List[data.Texture].Data[data.X, data.Y].Block[b])
                                    Selected.Attribute[x, y].Block[b] = Tile.List[data.Texture].Data[data.X, data.Y].Block[b];
                        }
                    }
        }

        private TileAttribute Attribute_Selected()
        {
            // Retorna com o atributo selecionado
            if (optA_Block.Checked) return TileAttribute.Block;
            if (optA_Warp.Checked) return TileAttribute.Warp;
            if (optA_Item.Checked) return TileAttribute.Item;
            return TileAttribute.None;
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
                foreach (var item in Item.List.Values) cmbA_Item.Items.Add(item);
                cmbA_Item.SelectedIndex = 0;
                numA_Item_Amount.Value = _aData2 = 1;
            }
            grpA_Item.Visible = optA_Item.Checked;
        }

        private void cmbA_Warp_Map_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reseta os valores
            var warpMap = (Map)cmbA_Warp_Map.SelectedItem;
            _aData1 = warpMap.GetID().ToString();
        }

        private void numA_Warp_X_ValueChanged(object sender, EventArgs e)
        {
            _aData2 = (short)numA_Warp_X.Value;
        }

        private void numA_Warp_Y_ValueChanged(object sender, EventArgs e)
        {
            _aData3 = (short)numA_Warp_Y.Value;
        }

        private void cmbA_Warp_Direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            _aData4 = (short)cmbA_Warp_Direction.SelectedIndex;
        }

        private void cmbA_Item_SelectedIndexChanged(object sender, EventArgs e)
        {
            _aData1 = ((Item)cmbA_Item.SelectedItem).GetID().ToString();
        }

        private void numA_Item_Amount_ValueChanged(object sender, EventArgs e)
        {
            _aData2 = (short)numA_Item_Amount.Value;
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
            Selected.Light = new List<MapLight>();
        }
        #endregion

        #region Utils
        // Usado pra movimentar as seleções
        private Rectangle Selection_Rec(Rectangle temp)
        {
            // Largura
            if (temp.Width <= 0)
            {
                temp.X += temp.Width - 1;
                temp.Width = (temp.Width - 2) * -1;
            }
            // Altura
            if (temp.Height <= 0)
            {
                temp.Y += temp.Height - 1;
                temp.Height = (temp.Height - 2) * -1;
            }

            // Retorna o valor do retângulo
            return temp;
        }

        // Retângulo da seleção de azulejos
        private Rectangle TilesSelection => Selection_Rec(_defTilesSelection);

        // Retângulo do mapa
        public Rectangle MapSelection
        {
            get
            {
                if (chkAuto.Checked) return new Rectangle(_mapMouse, new Size(1, 1));
                if (butMNormal.Checked && butPencil.Checked) return new Rectangle(_mapMouse, TilesSelection.Size);
                return Selection_Rec(_defMapSelection);
            }
        }

        // Retorna com o retângulo do azulejo ajustado à grade
        public Rectangle TileSource => new(TilesSelection.X * Grid, TilesSelection.Y * Grid, TilesSelection.Width * Grid, TilesSelection.Height * Grid);

        // Retorna o valor com o zoom
        public byte Zoom()
        {
            // Retorna o valor do zoom
            if (butZoom_2x.Checked) return 2;
            if (butZoom_4x.Checked) return 4;
            return 1;
        }

        public byte GridZoom => (byte)(Grid / Zoom());
        public Rectangle Zoom(Rectangle value) => new(value.X / Zoom(), value.Y / Zoom(), value.Width / Zoom(), value.Height / Zoom());
        public Rectangle Zoom_Grid(Rectangle rectangle) => new(rectangle.X * GridZoom, rectangle.Y * GridZoom, rectangle.Width * GridZoom, rectangle.Height * GridZoom);
        public Point Zoom_Grid(int x, int y) => new(x * GridZoom, y * GridZoom);

        // Tamanho da grade com o zoom
        // public static byte Grid_Zoom => (byte)(Grid / Editor_Maps.Form.Zoom());
        //public static Point Zoom(int X, int Y) => new Point(X * Grid_Zoom, Y * Grid_Zoom);
        #endregion
    }
}