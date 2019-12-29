using System;
using System.Windows.Forms;

public partial class Selection : Form
{
    // Usado para acessar os dados da janela
    public static Selection Objects = new Selection();

    public Selection()
    {
        InitializeComponent();
    }

    private void Selection_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Fecha a aplicação
        Program.Working = false;
    }

    private void butDirectory_Client_Click(object sender, EventArgs e)
    {
        // Seleciona o diretório atual
        Directory_Client.SelectedPath = Lists.Options.Directory_Client;

        // Apenas se já estiver selecionado um diretório
        if (Directory_Client.ShowDialog() != DialogResult.OK) return;

        // Salva os dados
        Lists.Options.Directory_Client = Directory_Client.SelectedPath;
        Write.Options();

        // Define e cria os diretórios
        Directories.SetClient();
    }

    private void butData_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_Data.Request();
    }

    private void butTools_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_Tools.Open();
    }

    private void butClasses_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_Classes.Request();
    }

    private void butMaps_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_Maps.Request();
    }

    private void butTiles_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_Tiles.Request();
    }

    private void butNPCs_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_NPCs.Request();
    }

    private void butItems_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (string.IsNullOrEmpty(Lists.Options.Directory_Client))
            MessageBox.Show("Select the client directory.");
        else
            Editor_Items.Request();
    }
}