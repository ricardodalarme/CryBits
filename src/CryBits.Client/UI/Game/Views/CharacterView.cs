using CryBits.Client.Core;
using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Network.Senders;
using CryBits.Client.Rendering.UI;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Intents;
using Iguina.Entities;
using System.Drawing;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(UiContext uiContext, GameContext context, IntentSender intentSender, EquipmentSlotRenderer equipmentRenderer, PortraitRenderer characterRenderer, TooltipView tooltip, DefinitionCatalog catalog) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("CharacterPanel");
    private SlotGrid EquipmentGrid => uiContext.Get<SlotGrid>("CharEquipmentGrid");
    private Button AddStrengthButton => uiContext.Get<Button>("AttrStrength");
    private Button AddResistanceButton => uiContext.Get<Button>("AttrResistance");
    private Button AddIntelligenceButton => uiContext.Get<Button>("AttrIntelligence");
    private Button AddAgilityButton => uiContext.Get<Button>("AttrAgility");
    private Button AddVitalityButton => uiContext.Get<Button>("AttrVitality");
    private Label CharNameLabel => uiContext.Get<Label>("CharName");
    private Picture FacePicture => uiContext.Get<Picture>("CharFace");
    private Label CharLevelLabel => uiContext.Get<Label>("CharLevel");
    private Label CharPointsLabel => uiContext.Get<Label>("CharPoints");
    private Label CharStrengthLabel => uiContext.Get<Label>("CharStrength");
    private Label CharResistanceLabel => uiContext.Get<Label>("CharResistance");
    private Label CharIntelligenceLabel => uiContext.Get<Label>("CharIntelligence");
    private Label CharAgilityLabel => uiContext.Get<Label>("CharAgility");
    private Label CharVitalityLabel => uiContext.Get<Label>("CharVitality");

    public override void Bind()
    {
        EquipmentGrid.OnSlotRightClick += OnSlotRightClick;
        EquipmentGrid.OnSlotHoverEnter += OnSlotHoverEnter;
        EquipmentGrid.OnSlotHoverLeave += tooltip.Hide;
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
        EquipmentGrid.OnSlotHoverLeave -= tooltip.Hide;
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

        var item = catalog.Items.Get(equipSlot);
        if (item is not { Bind: BindOn.Equip })
            intentSender.Send(new EquipmentRemoveIntent(default, (byte)slot));
    }

    private void OnSlotHoverEnter(int slot)
    {
        var equipment = context.LocalPlayer.GetEquipment();
        if (equipment == null) return;

        var item = catalog.Items.Get(equipment.Slots[slot]);
        if (item == null) return;
        tooltip.Show(item.Id, new Point(Panel.LastBoundingRect.X - 186, Panel.LastBoundingRect.Y + 5));
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

    public void Update()
    {
        var local = context.LocalPlayer;

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
