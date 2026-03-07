using Avalonia.Controls;
using Avalonia.Interactivity;
using CryBits.Editors.ViewModels;

namespace CryBits.Editors.Forms;

internal partial class EditorDataWindow : Window
{
    private readonly EditorDataViewModel _vm;

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
        _vm = new EditorDataViewModel();
        DataContext = _vm;
        InitializeComponent();
        LoadValues();
    }

    private void LoadValues()
    {
        txtGame_Name.Text = _vm.GameName;
        txtWelcome.Text = _vm.WelcomeMessage;
        numPort.Value = _vm.Port;
        numMax_Players.Value = _vm.MaxPlayers;
        numMax_Characters.Value = _vm.MaxCharacters;
        numMax_Party_Members.Value = _vm.MaxPartyMembers;
        numMax_Map_Items.Value = _vm.MaxMapItems;
        numPoints.Value = _vm.NumPoints;
        numMin_Name.Value = _vm.MinNameLength;
        numMax_Name.Value = _vm.MaxNameLength;
        numMin_Password.Value = _vm.MinPasswordLength;
        numMax_Password.Value = _vm.MaxPasswordLength;
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
        _vm.GameName = txtGame_Name.Text ?? string.Empty;
        _vm.WelcomeMessage = txtWelcome.Text ?? string.Empty;
        _vm.Port = ToShort(numPort.Value);
        _vm.MaxPlayers = ToByte(numMax_Players.Value);
        _vm.MaxCharacters = ToByte(numMax_Characters.Value);
        _vm.MaxPartyMembers = ToByte(numMax_Party_Members.Value);
        _vm.MaxMapItems = ToByte(numMax_Map_Items.Value);
        _vm.NumPoints = ToByte(numPoints.Value);
        _vm.MinNameLength = ToByte(numMin_Name.Value);
        _vm.MaxNameLength = ToByte(numMax_Name.Value);
        _vm.MinPasswordLength = ToByte(numMin_Password.Value);
        _vm.MaxPasswordLength = ToByte(numMax_Password.Value);
        _vm.Save();

        Close();
    }

    private void butCancel_Click(object sender, RoutedEventArgs e)
    {
        _vm.Cancel();
        Close();
    }
}
