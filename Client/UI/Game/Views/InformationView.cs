using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Graphics.Renderers;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class InformationView(ItemRenderer itemRenderer) : IView
{
    private static Panel Panel => Tools.Panels["Information"];
    private static Label TitleLabel => Tools.Labels["Information_Title"];
    private static Label DescriptionLabel => Tools.Labels["Information_Description"];
    private static Label AdditionalContextLabel => Tools.Labels["Information_AdditionalContext"];
    private static Picture ItemPicture => Tools.Pictures["Information_Item"];

    private static Item? _currentItem;

    public void Bind() => ItemPicture.OnRender += OnRenderItem;

    public void Unbind()
    {
        ItemPicture.OnRender -= OnRenderItem;
        Hide();
    }

    /// <summary>
    /// Populates and shows the information panel anchored at <paramref name="position"/>.
    /// </summary>
    /// <param name="itemId">The item to display.</param>
    /// <param name="position">Top-left corner of the panel.</param>
    /// <param name="contextLine">Optional extra line (e.g. shop price) prepended to additional context.</param>
    public static void Show(Guid itemId, Point position, string? contextLine = null)
    {
        var item = Item.List.Get(itemId);
        if (item == null) { Hide(); return; }

        _currentItem = item;

        // Position panel and child components relative to the anchor
        Panel.Position = position;
        TitleLabel.Position = new Point(position.X + 41, position.Y + 6);
        DescriptionLabel.Position = new Point(position.X + 82, position.Y + 20);
        ItemPicture.Position = new Point(position.X + 9, position.Y + 21);
        AdditionalContextLabel.Position = new Point(position.X + 10, position.Y + 90);

        // Title colored by rarity
        TitleLabel.Text = item.Name;
        TitleLabel.Color = item.Rarity switch
        {
            Rarity.Uncommon => 0xCCFF99,
            Rarity.Rare => 0x6699FF,
            Rarity.Epic => 0x9900CC,
            Rarity.Legendary => 0xFFFF4D,
            _ => 0xFFFFFF
        };

        DescriptionLabel.Text = item.Description;

        // Build additional context lines
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

    private void OnRenderItem(Point pos)
    {
        if (_currentItem != null)
            itemRenderer.DrawItem(_currentItem, 1, pos);
    }
}
