using System.Drawing;
using System.Windows.Forms;

public partial class Preview : Form
{
    // Usado para acessar os dados da janela
    public static Preview Objects = new Preview();

    // Imagens
    public static SFML.Graphics.Texture[] Texture;
    public static short Pattern;

    public Preview()
    {
        InitializeComponent();
    }

    public static short Select(SFML.Graphics.Texture[] Texture, short Selected)
    {
        // Previne erros
        if (Texture == null) return Selected;

        // Lista os itens
        Objects.List.Items.Clear();
        Objects.List.Items.Add("None");
        for (byte i = 1; i <= Texture.GetUpperBound(0); i++)
            Objects.List.Items.Add(i.ToString());

        // Define os dados
        Preview.Texture = Texture;
        Pattern = Selected;
        Objects.List.SelectedIndex = Selected;
        Graphics.Win_Preview = new SFML.Graphics.RenderWindow(Objects.picImage.Handle);

        // Abre a janela
        Objects.ShowDialog();

        // Retorna o valor selecionado
        return (short)Objects.List.SelectedIndex;
    }

    private void butSelect_Click(object sender, System.EventArgs e)
    {
        // Fecha a janela
        Pattern = (short)Objects.List.SelectedIndex;
        this.Visible = false;
    }

    private void Update_Bounds()
    {
        // Previne erros
        if (List.SelectedIndex > 0)
        {
            Objects.scrlImageX.Maximum = 0;
            Objects.scrlImageY.Maximum = 0;
        }

        // Dados
        Size Size = Graphics.TSize(Texture[List.SelectedIndex]);
        int Width = Size.Width - Objects.picImage.Width;
        int Height = Size.Height - Objects.picImage.Height;

        // Verifica se nada passou do limite minímo
        if (Width < 0) Width = 0;
        if (Height < 0) Height = 0;

        // Define os limites
        Objects.scrlImageX.Maximum = Width;
        Objects.scrlImageY.Maximum = Height;
    }

    private void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Update_Bounds();
    }

    private void tmpRender_Tick(object sender, System.EventArgs e)
    {
        // Renderiza as imagens
        Graphics.Preview_Image();
    }

    private void PreView_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Define o padrão
        Objects.List.SelectedIndex = Pattern;
    }
}
