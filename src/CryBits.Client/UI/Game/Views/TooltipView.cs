using CryBits.Client.Framework.UI.Entities;
using CryBits.Client.Graphics.Renderers;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using Iguina.Entities;
using Attribute = CryBits.Definitions.Characters.Attribute;
using Color = Iguina.Defs.Color;
using DrawingPoint = System.Drawing.Point;

namespace CryBits.Client.UI.Game.Views;

internal class TooltipView(ItemRenderer itemRenderer) : ViewBase
{
    private static Panel Panel => IguinaContext.Instance.Get<Panel>("Information");
    private static Label TitleLabel => IguinaContext.Instance.Get<Label>("InfoTitle");
    private static Label DescriptionLabel => IguinaContext.Instance.Get<Label>("InfoDesc");
    private static Label AdditionalContextLabel => IguinaContext.Instance.Get<Label>("InfoContext");
    private static Picture ItemPicture => IguinaContext.Instance.Get<Picture>("InfoItem");

    private static Item? _currentItem;

    public override void Bind() => IguinaContext.Instance.PostDraw += OnPostDraw;

    public override void Unbind()
    {
        IguinaContext.Instance.PostDraw -= OnPostDraw;
        Hide();
    }

    public static void Show(Guid itemId, DrawingPoint position, string? contextLine = null)
    {
        var item = DefinitionCatalog.Instance.Items.Get(itemId);
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

    public static void Hide()
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
