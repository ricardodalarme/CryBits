using System.Windows.Forms;


public partial class Login : Form
{
    // Usado para acessar os dados da janela
    public static Login Form = new Login();

    public Login()
    {
        InitializeComponent();
    }

    private void butConnect_Click(object sender, System.EventArgs e)
    {
        // Verifica se é possível se conectar ao servidor
        if (!Socket.TryConnect())
        {
            MessageBox.Show("The server is currently unavailable.");
            return;
        }
        if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtPassword.Text))
        {
            MessageBox.Show("Some field is empty.");
            return;
        }

        // Tenta fazer login
        Send.Connect();
    }

    private void Login_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Fecha a aplicação
        Program.Working = false;
    }
}