using CryBits.Client.Rendering.UI;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Items;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.Views;

internal class TooltipView(UiContext uiContext, ItemIconRenderer itemRenderer) : ViewBase
{
    private Panel Panel => uiContext.Get<Panel>("Information");
    private Label TitleLabel => uiContext.Get<Label>("InfoTitle");
    private Label DescriptionLabel => uiContext.Get<Label>("InfoDesc");
    private Label AdditionalContextLabel => uiContext.Get<Label>("InfoContext");
    private Image ItemImage => uiContext.Get<Image>("InfoItem");

    private Item? _currentItem;

    public override void Bind() { }

    public override void Unbind()
    {
        Hide();
    }

    public void Show(Item item, Vector2 position, string? contextLine = null)
    {
        _currentItem = item;

        Panel.Left = (int)position.X;
        Panel.Top = (int)position.Y;
        TitleLabel.Left = 41;
        TitleLabel.Top = 6;
        DescriptionLabel.Left = 82;
        DescriptionLabel.Top = 20;
        ItemImage.Left = 9;
        ItemImage.Top = 21;
        AdditionalContextLabel.Left = 10;
        AdditionalContextLabel.Top = 90;

        TitleLabel.Text = item.Name;
        TitleLabel.TextColor = item.Rarity switch
        {
            Rarity.Uncommon => new Color(0xCC, 0xFF, 0x99),
            Rarity.Rare => new Color(0x66, 0x99, 0xFF),
            Rarity.Epic => new Color(0x99, 0x00, 0xCC),
            Rarity.Legendary => new Color(0xFF, 0xFF, 0x4D),
            _ => Color.White
        };

        DescriptionLabel.Text = item.Description;

        var tex = itemRenderer.GetTexture(item);
        ItemImage.Renderable = tex != null ? new TextureRegion(tex) : null;

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
}
