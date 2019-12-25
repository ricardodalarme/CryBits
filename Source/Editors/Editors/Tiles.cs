using System;
using System.Drawing;
using System.Windows.Forms;

public partial class Editor_Tiles : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Tiles Objects = new Editor_Tiles();

    // Atributo selecionado
    private Globals.Tile_Attributes Attributes;

    public Editor_Tiles()
    {
        InitializeComponent();
    }

    public static void Open()
    {
        // Lê os dados e lista os itens
        Send.Request_Tiles();

        // Reseta os valores
        Objects.scrlTile.Value = 1;

        // Define os limites
        Objects.scrlTile.Maximum = Graphics.Tex_Tile.GetUpperBound(0);
        Update_Bounds();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void Update_Bounds()
    {
        int x = Graphics.TSize(Graphics.Tex_Tile[Objects.scrlTile.Value]).Width / Globals.Grid - Objects.picTile.Width / Globals.Grid;
        int y = Graphics.TSize(Graphics.Tex_Tile[Objects.scrlTile.Value]).Height / Globals.Grid - Objects.picTile.Height / Globals.Grid;

        // Verifica se nada passou do limite minímo
        if (x < 0) x = 0;
        if (y < 0) y = 0;

        // Define os limites
        Objects.scrlTileX.Maximum = x;
        Objects.scrlTileY.Maximum = y;
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados
        Write.Tiles();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butClear_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Clear.Tile((byte)scrlTile.Value);
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao menu
        Visible = false;
        Selection.Objects.Visible = true;
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
        Point Position = new Point((e.X + scrlTileX.Value * Globals.Grid) / Globals.Grid, (e.Y + scrlTileY.Value * Globals.Grid) / Globals.Grid);
        Point Tile_Dif = new Point(e.X - e.X / Globals.Grid * Globals.Grid, e.Y - e.Y / Globals.Grid * Globals.Grid);

        // Previne erros
        if (Position.X > Lists.Tile[Objects.scrlTile.Value].Data.GetUpperBound(0)) return;
        if (Position.Y > Lists.Tile[Objects.scrlTile.Value].Data.GetUpperBound(1)) return;

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
        {
            for (byte i = 0; i < (byte)Globals.Directions.Amount; i++)
                if (Tile_Dif.X >= Globals.Block_Position(i).X && Tile_Dif.X <= Globals.Block_Position(i).X + 8)
                    if (Tile_Dif.Y >= Globals.Block_Position(i).Y && Tile_Dif.Y <= Globals.Block_Position(i).Y + 8)
                        // Altera o valor de bloqueio
                        Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Block[i] = !Lists.Tile[scrlTile.Value].Data[Position.X, Position.Y].Block[i];
        }

    }

    private void optBlock_CheckedChanged(object sender, EventArgs e)
    {
        // Define o atributo
        Attributes = Globals.Tile_Attributes.Block;
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