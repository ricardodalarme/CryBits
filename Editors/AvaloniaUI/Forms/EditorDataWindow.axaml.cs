using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Editors.Network;
using static CryBits.Globals;

namespace CryBits.Editors.AvaloniaUI.Forms;

internal partial class EditorDataWindow : Window
{
    public EditorDataWindow()
    {
        InitializeComponent();
        LoadValues();
        Closed += Editor_Data_Closed;
    }

    private void LoadValues()
    {
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

    private static byte ToByte(decimal? value)
    {
        return (byte)(value ?? 0m);
    }

    private static short ToShort(decimal? value)
    {
        return (short)(value ?? 0m);
    }

    private void butSave_Click(object sender, RoutedEventArgs e)
    {
        GameName = txtGame_Name.Text ?? string.Empty;
        WelcomeMessage = txtWelcome.Text ?? string.Empty;
        Port = ToShort(numPort.Value);
        MaxPlayers = ToByte(numMax_Players.Value);
        MaxCharacters = ToByte(numMax_Characters.Value);
        MaxPartyMembers = ToByte(numMax_Party_Members.Value);
        MaxMapItems = ToByte(numMax_Map_Items.Value);
        NumPoints = ToByte(numPoints.Value);
        MinNameLength = ToByte(numMin_Name.Value);
        MaxNameLength = ToByte(numMax_Name.Value);
        MinPasswordLength = ToByte(numMin_Password.Value);
        MaxPasswordLength = ToByte(numMax_Password.Value);
        Send.WriteServerData();

        Close();
    }

    private void butCancel_Click(object sender, RoutedEventArgs e)
    {
        Send.RequestServerData();
        Close();
    }

    private void Editor_Data_Closed(object sender, System.EventArgs e)
    {
    }
}
