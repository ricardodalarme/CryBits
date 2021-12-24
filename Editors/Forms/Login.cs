using System;
using System.Windows.Forms;
using CryBits.Client.Framework;
using CryBits.Editors.Network;
using DarkUI.Forms;

namespace CryBits.Editors.Forms;

internal partial class Login : DarkForm
{
    // Usado para acessar os dados da janela
    public static Login Form;

    public Login()
    {
        InitializeComponent();
        txtUsername.Text =  Options.Username;
        chkUsername.Checked = Options.Username != string.Empty;
    }

    private void butConnect_Click(object sender, EventArgs e)
    {
        // Verifica se é possível se conectar ao servidor
        if (!Socket.TryConnect())
        {
            MessageBox.Show(@"The server is currently unavailable.");
            return;
        }
        if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
        {
            MessageBox.Show(@"Some field is empty.");
            return;
        }

        // Tenta fazer login
        Send.Connect();

        // Salva o nome do usuário
        Options.Username = chkUsername.Checked ? txtUsername.Text : string.Empty;
        Client.Framework.Library.Write.Options();
    }

    private void Login_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Fecha a aplicação
        Program.Working = false;
    }
}