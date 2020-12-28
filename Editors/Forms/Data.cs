using System;
using System.Windows.Forms;
using CryBits.Editors.Network;
using DarkUI.Forms;
using static CryBits.Defaults;

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
            txtGame_Name.Text = GameName;
            txtWelcome.Text = WelcomeMessage;
            numPort.Value = Port;
            numMax_Players.Value = MaxPlayers;
            numMax_Characters.Value = MaxCharacters;
            numMax_Party_Members.Value = MaxPartyMembers;
            numMax_Map_Items.Value = MaxMapItems;
            numPoints.Value = NumPoints;
            numMin_Name.Value = MinNameLength;
            numMax_Name.Value = MaxNameLength;
            numMin_Password.Value = MinPasswordLength;
            numMax_Password.Value = MaxPasswordLength;
        }

        private void Editor_Data_FormClosed(object sender, FormClosedEventArgs e)
        {
            EditorMaps.Form.Show();
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados
            GameName = txtGame_Name.Text;
            WelcomeMessage = txtWelcome.Text;
            Port = (short)numPort.Value;
            MaxPlayers = (byte)numMax_Players.Value;
            MaxCharacters = (byte)numMax_Characters.Value;
            MaxPartyMembers = (byte)numMax_Party_Members.Value;
            MaxMapItems = (byte)numMax_Map_Items.Value;
            NumPoints = (byte)numPoints.Value;
            MinNameLength = (byte)numMin_Name.Value;
            MaxNameLength = (byte)numMax_Name.Value;
            MinPasswordLength = (byte)numMin_Password.Value;
            MaxPasswordLength = (byte)numMax_Password.Value;
            Send.WriteServerData();

            // Volta à janela principal
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Send.RequestServerData();
            Close();
        }
    }
}