using CryBits.Client.Framework.Assets;
using CryBits.Client.Rendering.UI;
using CryBits.Client.UI.Game.ViewModels;
using CryBits.Definitions.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Events;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class CharacterView(
    UiContext uiContext,
    ItemIconRenderer itemRenderer,
    TooltipView tooltip,
    CharacterViewModel viewModel) : ViewBase
{
    internal Panel Panel => uiContext.Get<Panel>("CharacterPanel");
    private Grid EquipmentGrid => uiContext.Get<Grid>("CharEquipmentGrid");
    private Button AddStrengthButton => uiContext.Get<Button>("AttrStrength");
    private Button AddResistanceButton => uiContext.Get<Button>("AttrResistance");
    private Button AddIntelligenceButton => uiContext.Get<Button>("AttrIntelligence");
    private Button AddAgilityButton => uiContext.Get<Button>("AttrAgility");
    private Button AddVitalityButton => uiContext.Get<Button>("AttrVitality");
    private Label CharNameLabel => uiContext.Get<Label>("CharName");
    private Image FaceImage => uiContext.Get<Image>("CharFace");
    private Label CharLevelLabel => uiContext.Get<Label>("CharLevel");
    private Label CharPointsLabel => uiContext.Get<Label>("CharPoints");
    private Label CharStrengthLabel => uiContext.Get<Label>("CharStrength");
    private Label CharResistanceLabel => uiContext.Get<Label>("CharResistance");
    private Label CharIntelligenceLabel => uiContext.Get<Label>("CharIntelligence");
    private Label CharAgilityLabel => uiContext.Get<Label>("CharAgility");
    private Label CharVitalityLabel => uiContext.Get<Label>("CharVitality");

    private readonly List<Image> _equipmentSlotWidgets = new();

    private void EnsureSlotWidgets()
    {
        if (_equipmentSlotWidgets.Count > 0) return;

        int cols = 4;
        int rows = 2;
        int slotSize = 32;
        int spacing = 4;

        EquipmentGrid.ColumnsProportions.Clear();
        for (int c = 0; c < cols; c++)
            EquipmentGrid.ColumnsProportions.Add(new Proportion(ProportionType.Pixels, slotSize));

        EquipmentGrid.RowsProportions.Clear();
        for (int r = 0; r < rows; r++)
            EquipmentGrid.RowsProportions.Add(new Proportion(ProportionType.Pixels, slotSize));

        EquipmentGrid.ColumnSpacing = spacing;
        EquipmentGrid.RowSpacing = spacing;
        EquipmentGrid.Widgets.Clear();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int slotIndex = r * cols + c;
                var img = new Image
                {
                    Width = slotSize,
                    Height = slotSize,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(img, c);
                Grid.SetRow(img, r);

                img.TouchDown += (sender, e) => OnSlotTouchDown(slotIndex);
                img.MouseEntered += (sender, e) => OnSlotHoverEnter(slotIndex);
                img.MouseLeft += (sender, e) => tooltip.Hide();

                EquipmentGrid.Widgets.Add(img);
                _equipmentSlotWidgets.Add(img);
            }
        }
    }

    public override void Bind()
    {
        EnsureSlotWidgets();
        AddStrengthButton.Click += OnAddStrengthPressed;
        AddResistanceButton.Click += OnAddResistancePressed;
        AddIntelligenceButton.Click += OnAddIntelligencePressed;
        AddAgilityButton.Click += OnAddAgilityPressed;
        AddVitalityButton.Click += OnAddVitalityPressed;

        UpdateView();
    }

    public override void Unbind()
    {
        tooltip.Hide();
        AddStrengthButton.Click -= OnAddStrengthPressed;
        AddResistanceButton.Click -= OnAddResistancePressed;
        AddIntelligenceButton.Click -= OnAddIntelligencePressed;
        AddAgilityButton.Click -= OnAddAgilityPressed;
        AddVitalityButton.Click -= OnAddVitalityPressed;
    }

    public void UpdateView()
    {
        EnsureSlotWidgets();
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

        if (viewModel.TextureNum > 0 && Textures.Faces[viewModel.TextureNum] is { } faceTex)
            FaceImage.Renderable = new TextureRegion(faceTex);

        for (int i = 0; i < _equipmentSlotWidgets.Count && i < viewModel.Equipment.Length; i++)
        {
            var item = viewModel.Equipment[i]?.Definition;
            if (item != null)
            {
                var tex = itemRenderer.GetTexture(item);
                _equipmentSlotWidgets[i].Renderable = tex != null ? new TextureRegion(tex) : null;
            }
            else
            {
                _equipmentSlotWidgets[i].Renderable = null;
            }
        }
    }

    private void OnSlotTouchDown(int slot)
    {
        var mouse = Mouse.GetState();
        if (mouse.RightButton == ButtonState.Pressed)
        {
            OnSlotRightClick(slot);
        }
    }

    private void OnSlotRightClick(int slot)
    {
        var equipVM = viewModel.Equipment[slot];
        if (equipVM == null || equipVM.ItemId == Guid.Empty) return;

        var item = equipVM.Definition;
        if (item is not { Bind: BindOn.Equip })
            viewModel.RemoveEquipment((short)slot);
    }

    private void OnSlotHoverEnter(int slot)
    {
        var equipVM = viewModel.Equipment[slot];
        if (equipVM == null || equipVM.Definition == null) return;

        tooltip.Show(equipVM.Definition, new Vector2(Panel.Left, Panel.Top + 5));
    }

    private void OnAddStrengthPressed(object? sender, MyraEventArgs e) => viewModel.SpendPoint(Attribute.Strength);
    private void OnAddResistancePressed(object? sender, MyraEventArgs e) => viewModel.SpendPoint(Attribute.Resistance);
    private void OnAddIntelligencePressed(object? sender, MyraEventArgs e) => viewModel.SpendPoint(Attribute.Intelligence);
    private void OnAddAgilityPressed(object? sender, MyraEventArgs e) => viewModel.SpendPoint(Attribute.Agility);
    private void OnAddVitalityPressed(object? sender, MyraEventArgs e) => viewModel.SpendPoint(Attribute.Vitality);
}
