using CryBits.Client.Graphics.Renderers;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using Attribute = CryBits.Definitions.Characters.Attribute;
using DrawingPoint = System.Drawing.Point;
using Ent = global::Iguina.Entities.Entity;
using IguinaRect = Iguina.Defs.Rectangle;

namespace CryBits.Client.UI.Game.Views;

internal sealed class InformationView
{
    private static InformationView? _instance;
    private readonly ItemRenderer _itemRenderer;
    private readonly DefinitionCatalog _catalog;
    private readonly UISystem _ui;

    private Panel? _panel;
    private Label? _titleLabel;
    private Label? _descriptionLabel;
    private Label? _additionalContextLabel;
    private Panel? _itemPicturePanel;
    private static Item? _currentItem;

    public InformationView(UISystem ui, ItemRenderer itemRenderer, DefinitionCatalog catalog)
    {
        _instance = this;
        _ui = ui;
        _itemRenderer = itemRenderer;
        _catalog = catalog;
    }

    public void Wire(System.Collections.Generic.Dictionary<string, global::Iguina.Entities.Entity> reg) { }
    public void Build(Panel root)
    {
        _panel = new Panel(_ui);
        _panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = "Textures/Panels/13.png",
            SourceRect = new IguinaRect { Width = 182, Height = 132 }
        };
        _panel.Size.SetPixels(182, 132);
        _panel.Anchor = Anchor.TopLeft;
        _panel.Offset.SetPixels(0, 0);
        _panel.Visible = false;
        root.AddChild(_panel);

        _titleLabel = new Label(_ui);
        _titleLabel.Anchor = Anchor.TopLeft;
        _titleLabel.Offset.SetPixels(41, 6);
        _panel.AddChild(_titleLabel);

        _descriptionLabel = new Label(_ui);
        _descriptionLabel.Anchor = Anchor.TopLeft;
        _descriptionLabel.Offset.SetPixels(82, 20);
        _panel.AddChild(_descriptionLabel);

        _itemPicturePanel = new Panel(_ui);
        _itemPicturePanel.Size.SetPixels(64, 64);
        _itemPicturePanel.Anchor = Anchor.TopLeft;
        _itemPicturePanel.Offset.SetPixels(9, 21);
        _itemPicturePanel.Events.AfterDraw += OnRenderItem;
        _panel.AddChild(_itemPicturePanel);

        _additionalContextLabel = new Label(_ui);
        _additionalContextLabel.Anchor = Anchor.TopLeft;
        _additionalContextLabel.Offset.SetPixels(10, 90);
        _panel.AddChild(_additionalContextLabel);
    }

    public static void Show(Guid itemId, DrawingPoint position, string? contextLine = null)
    {
        if (_instance == null) return;

        var item = DefinitionCatalog.Instance.Items.Get(itemId);
        if (item == null) { Hide(); return; }

        _currentItem = item;

        _instance._panel!.Offset.SetPixels(position.X, position.Y);
        _instance._titleLabel!.EnableStyleCommands = true;
        var colorHex = item.Rarity switch
        {
            Rarity.Uncommon => "CCFF99",
            Rarity.Rare => "6699FF",
            Rarity.Epic => "9900CC",
            Rarity.Legendary => "FFFF4D",
            _ => "FFFFFF"
        };
        _instance._titleLabel!.Text = "${FC:" + colorHex + "}" + item.Name + "${RESET}";
        _instance._descriptionLabel!.Text = item.Description;

        var lines = new List<string>();
        if (contextLine != null) lines.Add(contextLine);

        switch (item.Type)
        {
            case ItemType.Potion:
                for (byte n = 0; n < (byte)Vital.Count; n++)
                    if (item.PotionVital[n] != 0)
                        lines.Add($"{(Vital)n}: {item.PotionVital[n]}");
                if (item.PotionExperience != 0) lines.Add($"Experience: {item.PotionExperience}");
                break;

            case ItemType.Equipment:
                if (item.EquipType == (byte)Equipment.Weapon && item.WeaponDamage != 0)
                    lines.Add($"Damage: {item.WeaponDamage}");
                for (byte n = 0; n < (byte)Attribute.Count; n++)
                    if (item.EquipAttribute[n] != 0)
                        lines.Add($"{(Attribute)n}: {item.EquipAttribute[n]}");
                break;
        }

        _instance._additionalContextLabel!.Text = string.Join("\n", lines);
        _instance._additionalContextLabel.Visible = lines.Count > 0;
        _instance._panel!.Visible = true;
    }

    public static void Hide()
    {
        if (_instance == null) return;
        _instance._panel!.Visible = false;
        _currentItem = null;
    }

    private void OnRenderItem(Ent _)
    {
        if (_currentItem != null)
        {
            var rect = _itemPicturePanel!.LastVisibleBoundingRect;
            _itemRenderer.DrawItem(_currentItem, 1, new DrawingPoint(rect.X, rect.Y));
        }
    }
}
