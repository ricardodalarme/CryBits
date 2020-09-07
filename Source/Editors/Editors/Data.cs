using DarkUI.Forms;
using Network;
using System.Windows.Forms;

namespace Editors
{
    partial class Editor_Data : DarkForm
    {
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
            numMin_Name.Value = Lists.Server_Data.Min_Name_Length;
            numMax_Name.Value = Lists.Server_Data.Max_Name_Length;
            numMin_Password.Value = Lists.Server_Data.Min_Password_Length;
            numMax_Password.Value = Lists.Server_Data.Max_Password_Length;

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
            Lists.Server_Data.Min_Name_Length = (byte)numMin_Name.Value;
            Lists.Server_Data.Max_Name_Length = (byte)numMax_Name.Value;
            Lists.Server_Data.Min_Password_Length = (byte)numMin_Password.Value;
            Lists.Server_Data.Max_Password_Length = (byte)numMax_Password.Value;
            Send.Write_Server_Data();

            // Volta à janela principal
            Close();
            Editor_Maps.Form.Show();
        }

        private void butCancel_Click(object sender, System.EventArgs e)
        {
            // Volta à janela principal
            Send.Request_Server_Data();
            Close();
            Editor_Maps.Form.Show();
        }
    }
}