using System;
using System.Windows.Forms;

public partial class Editor_Quantity : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Quantity Objects = new Editor_Quantity();

    public Editor_Quantity()
    {
        InitializeComponent();
    }

    public static void Open(int Quantity)
    {
        // Abre a janela de alteração
        Objects.numQuantity.Value = Quantity;
        Objects.ShowDialog();
    }

    private void Editor_Quantity_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Volta ao editor sem salvar as alterações
        Cancel();
        e.Cancel = true;
    }

    private void butOk_Click(object sender, EventArgs e)
    {
        // Define o nova quantidade
        if (Editor_Maps.Objects.Visible) Editor_Maps.Change_Quantity();
        if (Editor_NPCs.Objects.Visible) Editor_NPCs.Change_Quantity();
        if (Editor_Items.Objects.Visible) Editor_Items.Change_Quantity();

        // Fecha a janela
        Visible = false;
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao editor sem salvar as alterações
        Cancel();
    }

    private void Cancel()
    {
        // Define o nova quantidade
        if (Editor_Classes.Objects.Visible) Editor_Classes.Objects.Enabled = true;
        if (Editor_Maps.Objects.Visible) Editor_Maps.Objects.Enabled = true;
        if (Editor_NPCs.Objects.Visible) Editor_NPCs.Objects.Enabled = true;
        if (Editor_Items.Objects.Visible) Editor_Items.Objects.Enabled = true;

        // Fecha a janela
        Visible = false;
    }
}