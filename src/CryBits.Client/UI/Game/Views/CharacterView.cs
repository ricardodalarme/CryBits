using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Items;
using Iguina.Entities;
using System.Drawing;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(
    UiContext uiContext,
    EquipmentSlotRenderer equipmentRenderer,
    PortraitRenderer characterRenderer,
    TooltipView tooltip,
    CharacterViewModel viewModel) : ViewBase
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
        viewModel.Refresh();
        var equipVM = viewModel.Equipment[slot];
        if (equipVM == null || equipVM.ItemId == Guid.Empty) return;

        var item = equipVM.Definition;
        if (item is not { Bind: BindOn.Equip })
            viewModel.RemoveEquipment((short)slot);
    }

    private void OnSlotHoverEnter(int slot)
    {
        viewModel.Refresh();
        var equipVM = viewModel.Equipment[slot];
        if (equipVM == null || equipVM.ItemId == Guid.Empty) return;

        tooltip.Show(equipVM.ItemId, new Point(Panel.LastBoundingRect.X - 186, Panel.LastBoundingRect.Y + 5));
    }

    private void RenderFace()
    {
        viewModel.Refresh();
        var pos = FacePicture.LastBoundingRect;
        characterRenderer.DrawFace(viewModel.TextureNum, new Point(pos.X, pos.Y));
    }

    private void OnPostDraw()
    {
        if (!Panel.Visible) return;

        for (var i = 0; i < EquipmentGrid.TotalSlots; i++)
        {
            var rect = EquipmentGrid.GetSlotRect(i);
            equipmentRenderer.DrawSlot(i, new Point(rect.X, rect.Y));
        }
    }

    private void OnAddStrengthPressed(Entity _) => viewModel.SpendPoint(Attribute.Strength);
    private void OnAddResistancePressed(Entity _) => viewModel.SpendPoint(Attribute.Resistance);
    private void OnAddIntelligencePressed(Entity _) => viewModel.SpendPoint(Attribute.Intelligence);
    private void OnAddAgilityPressed(Entity _) => viewModel.SpendPoint(Attribute.Agility);
    private void OnAddVitalityPressed(Entity _) => viewModel.SpendPoint(Attribute.Vitality);

    public void Update()
    {
        viewModel.Refresh();

        CharNameLabel.Text = viewModel.Name;
        CharLevelLabel.Text = viewModel.Level.ToString();
        CharPointsLabel.Text = viewModel.Points.ToString();

        var hasPoints = viewModel.HasPoints;
        AddStrengthButton.Visible = hasPoints;
        AddResistanceButton.Visible = hasPoints;
        AddIntelligenceButton.Visible = hasPoints;
        AddAgilityButton.Visible = hasPoints;
        AddVitalityButton.Visible = hasPoints;

        CharStrengthLabel.Text = viewModel.Strength.ToString();
        CharResistanceLabel.Text = viewModel.Resistance.ToString();
        CharIntelligenceLabel.Text = viewModel.Intelligence.ToString();
        CharAgilityLabel.Text = viewModel.Agility.ToString();
        CharVitalityLabel.Text = viewModel.Vitality.ToString();
    }
}
