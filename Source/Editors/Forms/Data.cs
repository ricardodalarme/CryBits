using System;
using CryBits.Editors.Library;
using CryBits.Editors.Network;
using DarkUI.Forms;

namespace CryBits.Editors.Forms
{
    partial class EditorData : DarkForm
    {
        public EditorData()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

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
        }

        private void Editor_Data_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            EditorMaps.Form.Show();
        }

        private void butSave_Click(object sender, EventArgs e)
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
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Send.Request_Server_Data();
            Close();
        }
    }
}