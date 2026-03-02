using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Network.Senders;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Client.Utils.UIUtils;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(PlayerSender playerSender) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Character"];
    internal static Button AddStrengthButton => Tools.Buttons["Attributes_Strength"];
    internal static Button AddResistanceButton => Tools.Buttons["Attributes_Resistance"];
    internal static Button AddIntelligenceButton => Tools.Buttons["Attributes_Intelligence"];
    internal static Button AddAgilityButton => Tools.Buttons["Attributes_Agility"];
    internal static Button AddVitalityButton => Tools.Buttons["Attributes_Vitality"];

    public static short CurrentSlot => GetSlotAtMousePosition(Panel, 7, 248, 1, 5);

    public void Bind()
    {
        Panel.OnMouseDown += OnPanelMouseDown;
        AddStrengthButton.OnMouseUp += OnAddStrengthPressed;
        AddResistanceButton.OnMouseUp += OnAddResistancePressed;
        AddIntelligenceButton.OnMouseUp += OnAddIntelligencePressed;
        AddAgilityButton.OnMouseUp += OnAddAgilityPressed;
        AddVitalityButton.OnMouseUp += OnAddVitalityPressed;
    }

    public void Unbind()
    {
        Panel.OnMouseDown -= OnPanelMouseDown;
        AddStrengthButton.OnMouseUp -= OnAddStrengthPressed;
        AddResistanceButton.OnMouseUp -= OnAddResistancePressed;
        AddIntelligenceButton.OnMouseUp -= OnAddIntelligencePressed;
        AddAgilityButton.OnMouseUp -= OnAddAgilityPressed;
        AddVitalityButton.OnMouseUp -= OnAddVitalityPressed;
    }

    private void OnPanelMouseDown(MouseButtonEventArgs e)
    {
        var slot = CurrentSlot;

        if (slot < 0) return;
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
}
