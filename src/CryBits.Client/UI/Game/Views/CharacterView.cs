using CryBits.Client.Graphics.Renderers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Items;
using Attr = CryBits.Definitions.Characters.Attribute;
using DrawingPoint = System.Drawing.Point;
using IguinaRect = Iguina.Defs.Rectangle;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using Ent = global::Iguina.Entities.Entity;

namespace CryBits.Client.UI.Game.Views;

internal sealed class CharacterView
{
    private readonly UISystem _ui;
    private readonly GameContext _context;
    private readonly DefinitionCatalog _catalog;
    private readonly CharacterRenderer _characterRenderer;
    private readonly EquipmentRenderer _equipmentRenderer;
    private Panel? _panel;

    // Stat labels
    private Label? _nameLabel;
    private Label? _levelLabel;
    private Label? _strLabel;
    private Label? _resLabel;
    private Label? _intLabel;
    private Label? _agiLabel;
    private Label? _vitLabel;
    private Label? _pointsLabel;
    private IguinaSlotGrid? _equipmentGrid;
    private Panel? _facePanel;
    private readonly Button[] _attrButtons = new Button[5];

    public bool IsVisible => _panel?.Visible ?? false;
    public void SetVisible(bool visible) { if (_panel != null) _panel.Visible = visible; }

    public CharacterView(UISystem ui, GameContext context, DefinitionCatalog catalog,
        CharacterRenderer characterRenderer, EquipmentRenderer equipmentRenderer)
    {
        _ui = ui;
        _context = context;
        _catalog = catalog;
        _characterRenderer = characterRenderer;
        _equipmentRenderer = equipmentRenderer;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        // Character panel #11: 190x287 at (596, 270)
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/11.png",
            SourceRect = new IguinaRect { Width = 190, Height = 287 }
        };
        _panel.Size.SetPixels(190, 287);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(596, 270);
        _panel.Visible = false;
        root.AddChild(_panel);

        // Name label — absolute (614, 322) → relative (18, 52)
        _nameLabel = new Label(_ui);
        _nameLabel.Anchor = Anchor.TopLeft;
        _nameLabel.Offset.SetPixels(18, 52);
        _panel.AddChild(_nameLabel);

        // Level label — absolute (614, 349) → relative (18, 79)
        _levelLabel = new Label(_ui);
        _levelLabel.Anchor = Anchor.TopLeft;
        _levelLabel.Offset.SetPixels(18, 79);
        _panel.AddChild(_levelLabel);

        // Stat labels
        _strLabel = CreateStatLabel(628, 416, 32, 146);
        _resLabel = CreateStatLabel(628, 432, 32, 162);
        _intLabel = CreateStatLabel(628, 448, 32, 178);
        _agiLabel = CreateStatLabel(628, 464, 32, 194);
        _vitLabel = CreateStatLabel(628, 480, 32, 210);
        _pointsLabel = new Label(_ui);
        _pointsLabel.Anchor = Anchor.TopLeft;
        _pointsLabel.Offset.SetPixels(14, 228);
        _panel.AddChild(_pointsLabel);

        // Equipment grid — 5 columns x 1 row, 32x32, padding 3 at (604, 518) → relative (8, 248)
        _equipmentGrid = new IguinaSlotGrid(_ui, 5, 1, 32, 3, 8, 248, _panel);
        _equipmentGrid.SlotRender += OnEquipmentSlotRender;
        _equipmentGrid.SlotRightClick += OnEquipmentRightClick;
        _equipmentGrid.SlotHover += OnEquipmentHover;
        _equipmentGrid.SlotLeave += _ => UI.Game.Views.InformationView.Hide();

        // Face picture — absolute (678, 307) → relative (82, 37)
        _facePanel = new Panel(_ui);
        _facePanel.Size.SetPixels(96, 96);
        _facePanel.Anchor = Anchor.TopLeft;
        _facePanel.Offset.SetPixels(82, 37);
        _facePanel.Events.AfterDraw += OnRenderFace;
        _panel.AddChild(_facePanel);

        _attrButtons[0] = AddAttributeButton("Attributes_Strength", 16, 148);
        _attrButtons[1] = AddAttributeButton("Attributes_Resistance", 16, 164);
        _attrButtons[2] = AddAttributeButton("Attributes_Intelligence", 16, 180);
        _attrButtons[3] = AddAttributeButton("Attributes_Agility", 16, 196);
        _attrButtons[4] = AddAttributeButton("Attributes_Vitality", 16, 212);

        Update();
    }

    private Label CreateStatLabel(int absX, int absY, int offsetX, int offsetY)
    {
        var label = new Label(_ui);
        label.Anchor = Anchor.TopLeft;
        label.Offset.SetPixels(offsetX, offsetY);
        _panel!.AddChild(label);
        return label;
    }

    private Button AddAttributeButton(string name, int offsetX, int offsetY)
    {
        var btn = new Button(_ui);
        // Button texture #22: 15x15
        btn.OverrideStyles.Icon = new IconTexture
        {
            TextureId = "Textures/Buttons/22.png",
            SourceRect = new IguinaRect { Width = 15, Height = 15 },
            TextureScale = 1
        };
        btn.Size.SetPixels(15, 15);
        btn.Anchor = Anchor.TopLeft;
        btn.Offset.SetPixels(offsetX, offsetY);
        btn.Paragraph.Text = string.Empty;
        btn.Events.OnClick += _ => OnAddAttributePressed(name);
        _panel!.AddChild(btn);
        return btn;
    }

    private void OnAddAttributePressed(string attrName)
    {
        var attr = attrName switch
        {
            "Attributes_Strength" => Attr.Strength,
            "Attributes_Resistance" => Attr.Resistance,
            "Attributes_Intelligence" => Attr.Intelligence,
            "Attributes_Agility" => Attr.Agility,
            "Attributes_Vitality" => Attr.Vitality,
            _ => Attr.Strength
        };
        PlayerSender.Instance.AddPoint(attr);
    }

    public void Update()
    {
        if (_context.LocalPlayer.Entity == Arch.Core.Entity.Null) return;

        _nameLabel!.Text = $"Name: {_context.LocalPlayer.GetName()}";
        _levelLabel!.Text = $"Level: {_context.LocalPlayer.GetLevel().Level}";
        var attrs = _context.LocalPlayer.GetAttributes();
        _strLabel!.Text = $"Strength: {attrs.Values[(byte)Attr.Strength]}";
        _resLabel!.Text = $"Resistance: {attrs.Values[(byte)Attr.Resistance]}";
        _intLabel!.Text = $"Intelligence: {attrs.Values[(byte)Attr.Intelligence]}";
        _agiLabel!.Text = $"Agility: {attrs.Values[(byte)Attr.Agility]}";
        _vitLabel!.Text = $"Vitality: {attrs.Values[(byte)Attr.Vitality]}";
        _pointsLabel!.Text = $"Points: {_context.LocalPlayer.GetLevel().Points}";

        var hasPoints = _context.LocalPlayer.GetLevel().Points > 0;
        foreach (var btn in _attrButtons)
            if (btn != null) btn.Visible = hasPoints;
    }

    private void OnRenderFace(Ent _)
    {
        if (_context.LocalPlayer.Entity == Arch.Core.Entity.Null) return;
        var rect = _facePanel!.LastVisibleBoundingRect;
        var texNum = _context.LocalPlayer.GetFaceComponent().TextureNum;
        if (texNum > 0) _characterRenderer.DrawFace(texNum, new DrawingPoint(rect.X, rect.Y));
    }

    private void OnEquipmentSlotRender(int slot)
    {
        var equip = _context.LocalPlayer.GetEquipment();
        if (equip.Slots[slot] == null) return;
        var rect = _equipmentGrid!.GetSlotRect(slot);
        _equipmentRenderer.DrawSlot(slot, new DrawingPoint(rect.X, rect.Y));
    }

    private void OnEquipmentRightClick(int slot)
    {
        if (_context.LocalPlayer.GetEquipment().Slots[slot] == null) return;
        if (_context.LocalPlayer.GetEquipment().Slots[slot].Bind != BindOn.Equip)
            PlayerSender.Instance.EquipmentRemove((byte)slot);
    }

    private void OnEquipmentHover(int slot)
    {
        var item = _context.LocalPlayer.GetEquipment().Slots[slot];
        if (item == null) return;
        UI.Game.Views.InformationView.Show(item.Id, new DrawingPoint(-186, 5));
    }
}
