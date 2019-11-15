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

    private void butDirectory_Server_Click(object sender, EventArgs e)
    {
        // Seleciona o diretório atual
        Directory_Server.SelectedPath = Lists.Options.Directory_Server;

        // Apenas se já estiver selecionado um diretório
        if (Directory_Server.ShowDialog() != DialogResult.OK) return;

        // Salva os dados
        Lists.Options.Directory_Server = Directory_Server.SelectedPath;
        Write.Options();

        // Define e cria os diretórios
        Directories.SetServer();
    }

    private void butData_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_Data.Open();
    }

    private void butTools_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_Tools.Open();
    }

    private void butClasses_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_Classes.Open();
    }

    private void butMaps_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_Maps.Open();
    }

    private void butTiles_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_Tiles.Open();
    }

    private void butNPCs_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_NPCs.Open();
    }

    private void butItems_Click(object sender, EventArgs e)
    {
        // Verifica se os diretórios foram selecionados
        if (Lists.Options.Directory_Server == String.Empty || Lists.Options.Directory_Client == String.Empty)
            MessageBox.Show("Select the directories.");
        else
            Editor_Items.Open();
    }
}