using System;
using System.Drawing;
using System.Windows.Forms;
using CryBits.Editors.Entities;
using CryBits.Editors.Library;
using CryBits.Enums;
using DarkUI.Forms;
using SFML.Graphics;
using static CryBits.Defaults;
using static CryBits.Editors.Logic.Utils;
using Graphics = CryBits.Editors.Media.Graphics;

namespace CryBits.Editors.Forms
{
    internal partial class EditorTiles : DarkForm
    {
        // Usado para acessar os dados da janela
        public static EditorTiles Form;

        // Atributo selecionado
        private TileAttribute _attributes;

        public EditorTiles()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

            // Inicializa a janela de renderização
            Graphics.WinTile = new RenderWindow(picTile.Handle);

            // Define os limites
            scrlTile.Maximum = Graphics.TexTile.GetUpperBound(0);
            Update_Bounds();
        }

        private void Editor_Tiles_FormClosed(object sender, FormClosedEventArgs e)
        {
            Graphics.WinTile = null;
            EditorMaps.Form.Show();
        }

        private void Update_Bounds()
        {
            int x = Graphics.Size(Graphics.TexTile[scrlTile.Value]).Width / Grid - picTile.Width / Grid;
            int y = Graphics.Size(Graphics.TexTile[scrlTile.Value]).Height / Grid - picTile.Height / Grid;

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
            Tile.List[(byte)scrlTile.Value] = new Tile((byte)scrlTile.Value);
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
            Point position = new Point((e.X + scrlTileX.Value * Grid) / Grid, (e.Y + scrlTileY.Value * Grid) / Grid);
            Point tileDif = new Point(e.X - e.X / Grid * Grid, e.Y - e.Y / Grid * Grid);

            // Previne erros
            if (position.X > Tile.List[scrlTile.Value].Data.GetUpperBound(0)) return;
            if (position.Y > Tile.List[scrlTile.Value].Data.GetUpperBound(1)) return;

            // Atributos
            if (optAttributes.Checked)
            {
                // Define
                if (e.Button == MouseButtons.Left)
                    Tile.List[scrlTile.Value].Data[position.X, position.Y].Attribute = (byte)_attributes;
                // Remove
                else if (e.Button == MouseButtons.Right)
                    Tile.List[scrlTile.Value].Data[position.X, position.Y].Attribute = 0;
            }
            // Bloqueio direcional
            else if (optDirBlock.Checked)
                for (byte i = 0; i < (byte)Direction.Count; i++)
                    if (tileDif.X >= Block_Position(i).X && tileDif.X <= Block_Position(i).X + 8)
                        if (tileDif.Y >= Block_Position(i).Y && tileDif.Y <= Block_Position(i).Y + 8)
                            if (Tile.List[scrlTile.Value].Data[position.X, position.Y].Attribute != (byte)TileAttribute.Block)
                                // Altera o valor de bloqueio
                                Tile.List[scrlTile.Value].Data[position.X, position.Y].Block[i] = !Tile.List[scrlTile.Value].Data[position.X, position.Y].Block[i];
        }

        private void optBlock_CheckedChanged(object sender, EventArgs e)
        {
            // Define o atributo
            _attributes = TileAttribute.Block;
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