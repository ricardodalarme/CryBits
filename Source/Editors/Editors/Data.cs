using System.Windows.Forms;

public partial class Editor_Data : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Data Objects = new Editor_Data();

    public Editor_Data()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Globals.OpenEditor = Objects;
        Send.Request_Server_Data();
    }

    public static void Open()
    {
        // Define os valores
        Objects.txtGame_Name.Text = Lists.Server_Data.Game_Name;
        Objects.txtWelcome.Text = Lists.Server_Data.Welcome;
        Objects.numPort.Value = Lists.Server_Data.Port;
        Objects.numMax_Players.Value = Lists.Server_Data.Max_Players;
        Objects.numMax_Characters.Value = Lists.Server_Data.Max_Characters;
        Objects.numMax_Party_Members.Value = Lists.Server_Data.Max_Party_Members;
        Objects.numMax_Map_Items.Value = Lists.Server_Data.Max_Map_Items;
        
        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void butSave_Click(object sender, System.EventArgs e)
    {
        // Salva os dados
        Lists.Server_Data.Game_Name = Objects.txtGame_Name.Text;
        Lists.Server_Data.Welcome = Objects.txtWelcome.Text;
        Lists.Server_Data.Port = (short)Objects.numPort.Value;
        Lists.Server_Data.Max_Players = (byte)Objects.numMax_Players.Value;
        Lists.Server_Data.Max_Characters = (byte)Objects.numMax_Characters.Value;
        Lists.Server_Data.Max_Party_Members = (byte)Objects.numMax_Party_Members.Value;
        Lists.Server_Data.Max_Map_Items = (byte)Objects.numMax_Map_Items.Value;
        Send.Write_Server_Data();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butCancel_Click(object sender, System.EventArgs e)
    {
        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }
}