using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Rendering.UI;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Iguina.Entities;
using Attribute = CryBits.Definitions.Characters.Attribute;
using Color = Iguina.Defs.Color;
using DrawingPoint = System.Drawing.Point;

namespace CryBits.Client.UI.Game.Views;

internal class TooltipView(UiContext uiContext, ItemIconRenderer itemRenderer, DefinitionCatalog catalog) : ViewBase
{
    private Panel Panel => uiContext.Get<Panel>("Information");
    private Label TitleLabel => uiContext.Get<Label>("InfoTitle");
    private Label DescriptionLabel => uiContext.Get<Label>("InfoDesc");
    private Label AdditionalContextLabel => uiContext.Get<Label>("InfoContext");
    private Picture ItemPicture => uiContext.Get<Picture>("InfoItem");

    private Item? _currentItem;

    public override void Bind() => uiContext.PostDraw += OnPostDraw;

    public override void Unbind()
    {
        uiContext.PostDraw -= OnPostDraw;
        Hide();
    }

    public void Show(Guid itemId, DrawingPoint position, string? contextLine = null)
    {
        var item = catalog.Items.Get(itemId);
        if (item == null) { Hide(); return; }

        _currentItem = item;

        Panel.Offset.SetPixels(position.X, position.Y);
        TitleLabel.Offset.SetPixels(41, 6);
        DescriptionLabel.Offset.SetPixels(82, 20);
        ItemPicture.Offset.SetPixels(9, 21);
        AdditionalContextLabel.Offset.SetPixels(10, 90);

        TitleLabel.Text = item.Name;
        var hex = item.Rarity switch
        {
            Rarity.Uncommon => 0xCCFF99,
            Rarity.Rare => 0x6699FF,
            Rarity.Epic => 0x9900CC,
            Rarity.Legendary => 0xFFFF4D,
            _ => 0xFFFFFF
        };
        TitleLabel.OverrideStyles.TextFillColor = new Color(
            (byte)(hex >> 16), (byte)(hex >> 8), (byte)hex, 255);

        DescriptionLabel.Text = item.Description;

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

        AdditionalContextLabel.Text = string.Join("\n", lines);
        AdditionalContextLabel.Visible = lines.Count > 0;

        Panel.Visible = true;
    }

    public void Hide()
    {
        Panel.Visible = false;
        _currentItem = null;
    }

    private void OnPostDraw()
    {
        if (_currentItem == null) return;
        var rect = ItemPicture.LastBoundingRect;
        itemRenderer.DrawItem(_currentItem, 1, new DrawingPoint(rect.X, rect.Y));
    }
}
