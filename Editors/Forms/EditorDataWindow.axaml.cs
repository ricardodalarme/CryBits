using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Editors.Network;
using static CryBits.Globals;

namespace CryBits.Editors.Forms;

internal partial class EditorDataWindow : Window
{
    /// <summary>Opens the Data editor, hiding the owner window while open.</summary>
    public static void Open(Window owner)
    {
        owner.Hide();
        var window = new EditorDataWindow();
        window.Closed += (_, _) => owner.Show();
        window.Show();
    }

    public EditorDataWindow()
    {
        InitializeComponent();
        LoadValues();
        Closed += Editor_Data_Closed;
    }

    private void LoadValues()
    {
        txtGame_Name.Text = Config.GameName;
        txtWelcome.Text = Config.WelcomeMessage;
        numPort.Value = Config.Port;
        numMax_Players.Value = Config.MaxPlayers;
        numMax_Characters.Value = Config.MaxCharacters;
        numMax_Party_Members.Value = Config.MaxPartyMembers;
        numMax_Map_Items.Value = Config.MaxMapItems;
        numPoints.Value = Config.NumPoints;
        numMin_Name.Value = Config.MinNameLength;
        numMax_Name.Value = Config.MaxNameLength;
        numMin_Password.Value = Config.MinPasswordLength;
        numMax_Password.Value = Config.MaxPasswordLength;
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
        Config.GameName = txtGame_Name.Text ?? string.Empty;
        Config.WelcomeMessage = txtWelcome.Text ?? string.Empty;
        Config.Port = ToShort(numPort.Value);
        Config.MaxPlayers = ToByte(numMax_Players.Value);
        Config.MaxCharacters = ToByte(numMax_Characters.Value);
        Config.MaxPartyMembers = ToByte(numMax_Party_Members.Value);
        Config.MaxMapItems = ToByte(numMax_Map_Items.Value);
        Config.NumPoints = ToByte(numPoints.Value);
        Config.MinNameLength = ToByte(numMin_Name.Value);
        Config.MaxNameLength = ToByte(numMax_Name.Value);
        Config.MinPasswordLength = ToByte(numMin_Password.Value);
        Config.MaxPasswordLength = ToByte(numMax_Password.Value);
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
