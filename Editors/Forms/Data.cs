using System;
using System.Windows.Forms;
using CryBits.Editors.Library;
using CryBits.Editors.Network;
using DarkUI.Forms;

namespace CryBits.Editors.Forms
{
    internal partial class EditorData : DarkForm
    {
        public EditorData()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

            // Define os valores
            txtGame_Name.Text = Lists.ServerData.GameName;
            txtWelcome.Text = Lists.ServerData.Welcome;
            numPort.Value = Lists.ServerData.Port;
            numMax_Players.Value = Lists.ServerData.MaxPlayers;
            numMax_Characters.Value = Lists.ServerData.MaxCharacters;
            numMax_Party_Members.Value = Lists.ServerData.MaxPartyMembers;
            numMax_Map_Items.Value = Lists.ServerData.MaxMapItems;
            numPoints.Value = Lists.ServerData.NumPoints;
            numMin_Name.Value = Lists.ServerData.MinNameLength;
            numMax_Name.Value = Lists.ServerData.MaxNameLength;
            numMin_Password.Value = Lists.ServerData.MinPasswordLength;
            numMax_Password.Value = Lists.ServerData.MaxPasswordLength;
        }

        private void Editor_Data_FormClosed(object sender, FormClosedEventArgs e)
        {
            EditorMaps.Form.Show();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados
            Lists.ServerData.GameName = txtGame_Name.Text;
            Lists.ServerData.Welcome = txtWelcome.Text;
            Lists.ServerData.Port = (short)numPort.Value;
            Lists.ServerData.MaxPlayers = (byte)numMax_Players.Value;
            Lists.ServerData.MaxCharacters = (byte)numMax_Characters.Value;
            Lists.ServerData.MaxPartyMembers = (byte)numMax_Party_Members.Value;
            Lists.ServerData.MaxMapItems = (byte)numMax_Map_Items.Value;
            Lists.ServerData.NumPoints = (byte)numPoints.Value;
            Lists.ServerData.MinNameLength = (byte)numMin_Name.Value;
            Lists.ServerData.MaxNameLength = (byte)numMax_Name.Value;
            Lists.ServerData.MinPasswordLength = (byte)numMin_Password.Value;
            Lists.ServerData.MaxPasswordLength = (byte)numMax_Password.Value;
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