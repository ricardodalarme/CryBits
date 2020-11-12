using CryBits.Editors.Library;
using DarkUI.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;
using static CryBits.Editors.Logic.Utils;

namespace CryBits.Editors.Forms
{
    partial class Editor_Tiles : DarkForm
    {
        // Usado para acessar os dados da janela
        public static Editor_Tiles Form;

        // Atributo selecionado
        private Tile_Attributes Attributes;

        public Editor_Tiles()
        {
            InitializeComponent();

            // Abre janela
            Editor_Maps.Form.Hide();
            Show();

            // Inicializa a janela de renderização
            Graphics.Win_Tile = new SFML.Graphics.RenderWindow(picTile.Handle);

            // Define os limites
            scrlTile.Maximum = Graphics.Tex_Tile.GetUpperBound(0);
            Update_Bounds();
        }

        private void Editor_Tiles_FormClosed(object sender, FormClosedEventArgs e)
        {
            Graphics.Win_Tile = null;
            Editor_Maps.Form.Show();
        }

        private void Update_Bounds()
        {
            int x = Graphics.TSize(Graphics.Tex_Tile[scrlTile.Value]).Width / Grid - picTile.Width / Grid;
            int y = Graphics.TSize(Graphics.Tex_Tile[scrlTile.Value]).Height / Grid - picTile.Height / Grid;

            // Verifica se nada passou do limite minímo
            if (x < 0) x = 0;
            if (y < 0) y = 0;

            // Define os limites
            scrlTileX.Maximum = x;
            scrlTileY.Maximum = y;
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados e volta à janela principal
            Write.Tiles();
            Close();
        }

        private void butClear_Click(object sender, EventArgs e)
        {
            // Limpa os dados
            Lists.Tile[(byte)scrlTile.Value] = new Entities.Tile((byte)scrlTile.Value);
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Close();
        }

        private void scrlTile_ValueChanged(object sender, EventArgs e)
        {
            // Define os valores
            grpTile.Text = "Tile: " + scrlTile.Value;
            scrlTileX.Value = 0;
            scrlTileY.Value = 0;
            Update_Bounds();
        }

        private void picTile_MouseDown(object sender, MouseEventArgs e)
        {
            Point Position = new Point((e.X + scrlTileX.Value * Grid) / Grid, (e.Y + scrlTileY.Value * Grid) / Grid);
            Point Tile_Dif = new Point(e.X - e.X / Grid * Grid, e.Y - e.Y / Grid * Grid);

            // Previne erros
            if (Position.X > Lists.Tile[scrlTile.Value].Data.GetUpperBound(0)) return;
            if (Position.Y > Lists.Tile[scrlTile.Value].Data.GetUpperBound(1)) return;

            // Atributos
            if (optAttributes.Checked)
            {
                // Define
                if (e.Button == MouseButtons.Left)
                    Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Attribute = (byte)Attributes;
                // Remove
                else if (e.Button == MouseButtons.Right)
                    Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Attribute = 0;
            }
            // Bloqueio direcional
            else if (optDirBlock.Checked)
                for (byte i = 0; i < (byte)Directions.Count; i++)
                    if (Tile_Dif.X >= Block_Position(i).X && Tile_Dif.X <= Block_Position(i).X + 8)
                        if (Tile_Dif.Y >= Block_Position(i).Y && Tile_Dif.Y <= Block_Position(i).Y + 8)
                            if (Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Attribute != (byte)Tile_Attributes.Block)
                                // Altera o valor de bloqueio
                                Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Block[i] = !Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Block[i];
        }

        private void optBlock_CheckedChanged(object sender, EventArgs e)
        {
            // Define o atributo
            Attributes = Tile_Attributes.Block;
        }

        private void optAttributes_CheckedChanged(object sender, EventArgs e)
        {
            // Abre a janela de atributos
            grpAttributes.Visible = true;
        }

        private void optDirBlock_CheckedChanged(object sender, EventArgs e)
        {
            // Abre a janela de atributos
            grpAttributes.Visible = false;
        }
    }
}