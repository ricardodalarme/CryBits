using DarkUI.Forms;
using CryBits.Editors.Library;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using System.Windows.Forms;

namespace CryBits.Editors.Editors
{
    partial class Login : DarkForm
    {
        // Usado para acessar os dados da janela
        public static Login Form;

        public Login()
        {
            InitializeComponent();
            txtUsername.Text = Lists.Options.Username;
            chkUsername.Checked = Lists.Options.Username != string.Empty;
        }

        private void butConnect_Click(object sender, System.EventArgs e)
        {
            // Verifica se é possível se conectar ao servidor
            if (!Socket.TryConnect())
            {
                MessageBox.Show("The server is currently unavailable.");
                return;
            }
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Some field is empty.");
                return;
            }

            // Tenta fazer login
            Send.Connect();

            // Salva o nome do usuário
            if (chkUsername.Checked) Lists.Options.Username = txtUsername.Text;
            else Lists.Options.Username = string.Empty;
            Write.Options();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Fecha a aplicação
            Program.Working = false;
        }
    }
}