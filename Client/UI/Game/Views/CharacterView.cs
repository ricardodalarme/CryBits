using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Window;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(PlayerSender playerSender) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Character"];
    private static SlotGrid Grid => Tools.SlotGrids["Equipment_Grid"];
    internal static Button AddStrengthButton => Tools.Buttons["Attributes_Strength"];
    internal static Button AddResistanceButton => Tools.Buttons["Attributes_Resistance"];
    internal static Button AddIntelligenceButton => Tools.Buttons["Attributes_Intelligence"];
    internal static Button AddAgilityButton => Tools.Buttons["Attributes_Agility"];
    internal static Button AddVitalityButton => Tools.Buttons["Attributes_Vitality"];
    private static Label NameLabel => Tools.Labels["Character_Name"];
    private static Label LevelLabel => Tools.Labels["Character_Level"];
    private static Label StrengthLabel => Tools.Labels["Character_Strength"];
    private static Label ResistanceLabel => Tools.Labels["Character_Resistance"];
    private static Label IntelligenceLabel => Tools.Labels["Character_Intelligence"];
    private static Label AgilityLabel => Tools.Labels["Character_Agility"];
    private static Label VitalityLabel => Tools.Labels["Character_Vitality"];
    private static Label PointsLabel => Tools.Labels["Character_Points"];

    public void Bind()
    {
        Grid.OnMouseDown += OnGridMouseDown;
        AddStrengthButton.OnMouseUp += OnAddStrengthPressed;
        AddResistanceButton.OnMouseUp += OnAddResistancePressed;
        AddIntelligenceButton.OnMouseUp += OnAddIntelligencePressed;
        AddAgilityButton.OnMouseUp += OnAddAgilityPressed;
        AddVitalityButton.OnMouseUp += OnAddVitalityPressed;
    }

    public void Unbind()
    {
        Grid.OnMouseDown -= OnGridMouseDown;
        AddStrengthButton.OnMouseUp -= OnAddStrengthPressed;
        AddResistanceButton.OnMouseUp -= OnAddResistancePressed;
        AddIntelligenceButton.OnMouseUp -= OnAddIntelligencePressed;
        AddAgilityButton.OnMouseUp -= OnAddAgilityPressed;
        AddVitalityButton.OnMouseUp -= OnAddVitalityPressed;
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        if (Player.Me.Equipment[slot] == null) return;

        if (e.Button == Mouse.Button.Right)
            if (Player.Me.Equipment[slot].Bind != BindOn.Equip)
                playerSender.EquipmentRemove((byte)slot);
    }

    private void OnAddStrengthPressed()
    {
        playerSender.AddPoint(Attribute.Strength);
    }

    private void OnAddResistancePressed()
    {
        playerSender.AddPoint(Attribute.Resistance);
    }

    private void OnAddIntelligencePressed()
    {
        playerSender.AddPoint(Attribute.Intelligence);
    }

    private void OnAddAgilityPressed()
    {
        playerSender.AddPoint(Attribute.Agility);
    }

    private void OnAddVitalityPressed()
    {
        playerSender.AddPoint(Attribute.Vitality);
    }

    public static void Update()
    {
        NameLabel.SetArguments(Player.Me.Name);
        LevelLabel.SetArguments(Player.Me.Level);
        StrengthLabel.SetArguments(Player.Me.Attribute[(byte)Attribute.Strength]);
        ResistanceLabel.SetArguments(Player.Me.Attribute[(byte)Attribute.Resistance]);
        IntelligenceLabel.SetArguments(Player.Me.Attribute[(byte)Attribute.Intelligence]);
        AgilityLabel.SetArguments(Player.Me.Attribute[(byte)Attribute.Agility]);
        VitalityLabel.SetArguments(Player.Me.Attribute[(byte)Attribute.Vitality]);
        PointsLabel.SetArguments(Player.Me.Points);
    }
}
