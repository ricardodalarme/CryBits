using System.Windows.Forms;

public partial class Editor_Data : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Data Form;

    public Editor_Data()
    {
        // Inicializa os componentes
        InitializeComponent();

        // Define os valores
        txtGame_Name.Text = Lists.Server_Data.Game_Name;
        txtWelcome.Text = Lists.Server_Data.Welcome;
        numPort.Value = Lists.Server_Data.Port;
        numMax_Players.Value = Lists.Server_Data.Max_Players;
        numMax_Characters.Value = Lists.Server_Data.Max_Characters;
        numMax_Party_Members.Value = Lists.Server_Data.Max_Party_Members;
        numMax_Map_Items.Value = Lists.Server_Data.Max_Map_Items;
        numPoints.Value = Lists.Server_Data.Num_Points;

        // Abre a janela
        Editor_Maps.Form.Hide();
        Show();
    }

    private void butSave_Click(object sender, System.EventArgs e)
    {
        // Salva os dados
        Lists.Server_Data.Game_Name = txtGame_Name.Text;
        Lists.Server_Data.Welcome = txtWelcome.Text;
        Lists.Server_Data.Port = (short)numPort.Value;
        Lists.Server_Data.Max_Players = (byte)numMax_Players.Value;
        Lists.Server_Data.Max_Characters = (byte)numMax_Characters.Value;
        Lists.Server_Data.Max_Party_Members = (byte)numMax_Party_Members.Value;
        Lists.Server_Data.Max_Map_Items = (byte)numMax_Map_Items.Value;
        Lists.Server_Data.Num_Points = (byte)numPoints.Value;
        Send.Write_Server_Data();

        // Volta à janela principal
        Close();
        Editor_Maps.Form.Show();
    }

    private void butCancel_Click(object sender, System.EventArgs e)
    {
        // Volta à janela principal
        Close();
        Editor_Maps.Form.Show();
    }
}