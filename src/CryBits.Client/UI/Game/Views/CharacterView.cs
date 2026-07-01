using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using System.Drawing;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(IguinaContext uiContext, GameContext context, IntentSender intentSender, EquipmentRenderer equipmentRenderer, CharacterRenderer characterRenderer) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("CharacterPanel");
    private SlotGrid EquipmentGrid => uiContext.Get<SlotGrid>("CharEquipmentGrid");
    internal static Button AddStrengthButton => IguinaContext.Instance.Get<Button>("AttrStrength");
    internal static Button AddResistanceButton => IguinaContext.Instance.Get<Button>("AttrResistance");
    internal static Button AddIntelligenceButton => IguinaContext.Instance.Get<Button>("AttrIntelligence");
    internal static Button AddAgilityButton => IguinaContext.Instance.Get<Button>("AttrAgility");
    internal static Button AddVitalityButton => IguinaContext.Instance.Get<Button>("AttrVitality");
    private static Label CharNameLabel => IguinaContext.Instance.Get<Label>("CharName");
    private Picture FacePicture => uiContext.Get<Picture>("CharFace");
    private static Label CharLevelLabel => IguinaContext.Instance.Get<Label>("CharLevel");
    private static Label CharPointsLabel => IguinaContext.Instance.Get<Label>("CharPoints");
    private static Label CharStrengthLabel => IguinaContext.Instance.Get<Label>("CharStrength");
    private static Label CharResistanceLabel => IguinaContext.Instance.Get<Label>("CharResistance");
    private static Label CharIntelligenceLabel => IguinaContext.Instance.Get<Label>("CharIntelligence");
    private static Label CharAgilityLabel => IguinaContext.Instance.Get<Label>("CharAgility");
    private static Label CharVitalityLabel => IguinaContext.Instance.Get<Label>("CharVitality");

    public override void Bind()
    {
        EquipmentGrid.OnSlotRightClick += OnSlotRightClick;
        EquipmentGrid.OnSlotHoverEnter += OnSlotHoverEnter;
        EquipmentGrid.OnSlotHoverLeave += TooltipView.Hide;
        AddStrengthButton.Events.OnClick += OnAddStrengthPressed;
        AddResistanceButton.Events.OnClick += OnAddResistancePressed;
        AddIntelligenceButton.Events.OnClick += OnAddIntelligencePressed;
        AddAgilityButton.Events.OnClick += OnAddAgilityPressed;
        AddVitalityButton.Events.OnClick += OnAddVitalityPressed;
        FacePicture.OnRenderPicture += RenderFace;

        uiContext.PostDraw += FacePicture.Render;
        uiContext.PostDraw += OnPostDraw;
    }

    public override void Unbind()
    {
        FacePicture.OnRenderPicture -= RenderFace;
        uiContext.PostDraw -= FacePicture.Render;
        EquipmentGrid.OnSlotRightClick -= OnSlotRightClick;
        EquipmentGrid.OnSlotHoverEnter -= OnSlotHoverEnter;
        EquipmentGrid.OnSlotHoverLeave -= TooltipView.Hide;
        AddStrengthButton.Events.OnClick -= OnAddStrengthPressed;
        AddResistanceButton.Events.OnClick -= OnAddResistancePressed;
        AddIntelligenceButton.Events.OnClick -= OnAddIntelligencePressed;
        AddAgilityButton.Events.OnClick -= OnAddAgilityPressed;
        AddVitalityButton.Events.OnClick -= OnAddVitalityPressed;
        uiContext.PostDraw -= OnPostDraw;
    }

    private void OnSlotRightClick(int slot)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var equipSlot = equipment.Slots[slot];
        if (equipSlot == Guid.Empty) return;

        var item = DefinitionCatalog.Instance.Items.Get(equipSlot);
        if (item == null || item.Bind != BindOn.Equip)
            intentSender.Send(new EquipmentRemoveIntent(default, (byte)slot));
    }

    private void OnSlotHoverEnter(int slot)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var item = DefinitionCatalog.Instance.Items.Get(equipment.Slots[slot]);
        if (item == null) return;
        TooltipView.Show(item.Id, new Point(Panel.LastBoundingRect.X - 186, Panel.LastBoundingRect.Y + 5));
    }

    private void RenderFace()
    {
        var appearance = context.LocalPlayer.GetAppearance();
        if (appearance == null) return;
        var pos = FacePicture.LastBoundingRect;
        characterRenderer.DrawFace(appearance.TextureNum, new Point(pos.X, pos.Y));
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible) return;

        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        for (var i = 0; i < EquipmentGrid.TotalSlots; i++)
        {
            var rect = EquipmentGrid.GetSlotRect(i);
            equipmentRenderer.DrawSlot(i, new Point(rect.X, rect.Y));
        }
    }

    private void OnAddStrengthPressed(Entity _) => intentSender.Send(new AddPointIntent(default, (byte)Attribute.Strength));
    private void OnAddResistancePressed(Entity _) => intentSender.Send(new AddPointIntent(default, (byte)Attribute.Resistance));
    private void OnAddIntelligencePressed(Entity _) => intentSender.Send(new AddPointIntent(default, (byte)Attribute.Intelligence));
    private void OnAddAgilityPressed(Entity _) => intentSender.Send(new AddPointIntent(default, (byte)Attribute.Agility));
    private void OnAddVitalityPressed(Entity _) => intentSender.Send(new AddPointIntent(default, (byte)Attribute.Vitality));

    public static void Update()
    {
        var local = GameContext.Instance.LocalPlayer;

        CharNameLabel.Text = local.GetName();

        var level = local.GetLevel();
        if (level != null)
        {
            CharLevelLabel.Text = level.Level.ToString();
            CharPointsLabel.Text = level.Points.ToString();

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
            CharStrengthLabel.Text = attrs.Values[(byte)Attribute.Strength].ToString();
            CharResistanceLabel.Text = attrs.Values[(byte)Attribute.Resistance].ToString();
            CharIntelligenceLabel.Text = attrs.Values[(byte)Attribute.Intelligence].ToString();
            CharAgilityLabel.Text = attrs.Values[(byte)Attribute.Agility].ToString();
            CharVitalityLabel.Text = attrs.Values[(byte)Attribute.Vitality].ToString();
        }
    }
}
