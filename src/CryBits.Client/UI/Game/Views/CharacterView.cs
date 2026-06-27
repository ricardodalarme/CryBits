using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Intents;
using SFML.Window;
using System.Drawing;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(GameContext context, IntentSender intentSender, EquipmentRenderer equipmentRenderer, CharacterRenderer characterRenderer) : IView
{
    internal static Panel Panel => Tools.Panels["Menu_Character"];
    private static SlotGrid Grid => Tools.SlotGrids["Equipment_Grid"];
    private static Picture FacePicture => Tools.Pictures["Character_Face"];
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
        FacePicture.OnRender += OnRenderFace;
        Grid.OnRenderSlot += equipmentRenderer.DrawSlot;
        Grid.OnMouseDown += OnGridMouseDown;
        Grid.OnSlotHover += OnGridSlotHover;
        Grid.OnSlotLeave += OnGridSlotLeave;
        AddStrengthButton.OnMouseUp += OnAddStrengthPressed;
        AddResistanceButton.OnMouseUp += OnAddResistancePressed;
        AddIntelligenceButton.OnMouseUp += OnAddIntelligencePressed;
        AddAgilityButton.OnMouseUp += OnAddAgilityPressed;
        AddVitalityButton.OnMouseUp += OnAddVitalityPressed;
    }

    public void Unbind()
    {
        FacePicture.OnRender -= OnRenderFace;
        Grid.OnRenderSlot -= equipmentRenderer.DrawSlot;
        Grid.OnMouseDown -= OnGridMouseDown;
        Grid.OnSlotHover -= OnGridSlotHover;
        Grid.OnSlotLeave -= OnGridSlotLeave;
        AddStrengthButton.OnMouseUp -= OnAddStrengthPressed;
        AddResistanceButton.OnMouseUp -= OnAddResistancePressed;
        AddIntelligenceButton.OnMouseUp -= OnAddIntelligencePressed;
        AddAgilityButton.OnMouseUp -= OnAddAgilityPressed;
        AddVitalityButton.OnMouseUp -= OnAddVitalityPressed;
    }

    private void OnRenderFace(Point pos)
    {
        var appearance = context.LocalPlayer.GetAppearance();
        if (appearance != null)
            characterRenderer.DrawFace(appearance.TextureNum, pos);
    }

    private void OnGridMouseDown(MouseButtonEventArgs e, short slot)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var equipSlot = equipment.Slots[slot];
        if (equipSlot == Guid.Empty) return;

        if (e.Button == Mouse.Button.Right)
        {
            var item = DefinitionCatalog.Instance.Items.Get(equipSlot);
            if (item == null || item.Bind != BindOn.Equip)
                intentSender.Send(new EquipmentRemoveIntent(default, (byte)slot));
        }
    }

    private void OnGridSlotHover(short slot)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var item = DefinitionCatalog.Instance.Items.Get(equipment.Slots[slot]);
        if (item == null) return;
        InformationView.Show(item.Id, Panel.Position + new Size(-186, 5));
    }

    private void OnGridSlotLeave(short slot) => InformationView.Hide();

    private void OnAddStrengthPressed()
    {
        intentSender.Send(new AddPointIntent(default, (byte)Attribute.Strength));
    }

    private void OnAddResistancePressed()
    {
        intentSender.Send(new AddPointIntent(default, (byte)Attribute.Resistance));
    }

    private void OnAddIntelligencePressed()
    {
        intentSender.Send(new AddPointIntent(default, (byte)Attribute.Intelligence));
    }

    private void OnAddAgilityPressed()
    {
        intentSender.Send(new AddPointIntent(default, (byte)Attribute.Agility));
    }

    private void OnAddVitalityPressed()
    {
        intentSender.Send(new AddPointIntent(default, (byte)Attribute.Vitality));
    }
    public static void Update()
    {
        var local = GameContext.Instance.LocalPlayer;
        NameLabel.SetArguments(local.GetName());
        var level = local.GetLevel();
        if (level != null)
        {
            LevelLabel.SetArguments(level.Level);
            PointsLabel.SetArguments(level.Points);

            var hasPoints = level.Points > 0;
            AddStrengthButton.Visible = hasPoints;
            AddResistanceButton.Visible = hasPoints;
            AddIntelligenceButton.Visible = hasPoints;
            AddAgilityButton.Visible = hasPoints;
            AddVitalityButton.Visible = hasPoints;
        }
        var attrs = local.GetAttributes();
        if (attrs != null)
        {
            StrengthLabel.SetArguments(attrs.Values[(byte)Attribute.Strength]);
            ResistanceLabel.SetArguments(attrs.Values[(byte)Attribute.Resistance]);
            IntelligenceLabel.SetArguments(attrs.Values[(byte)Attribute.Intelligence]);
            AgilityLabel.SetArguments(attrs.Values[(byte)Attribute.Agility]);
            VitalityLabel.SetArguments(attrs.Values[(byte)Attribute.Vitality]);
        }
    }
}
