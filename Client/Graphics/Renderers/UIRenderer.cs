using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class UIRenderer(
    Renderer renderer,
    CharacterRenderer characterRenderer,
    ItemRenderer itemRenderer
)
{
    public static UIRenderer Instance { get; } = new(Renderer.Instance, CharacterRenderer.Instance, ItemRenderer.Instance);

    /// <summary>
    /// Recursively render a tree of UI components.
    /// </summary>
    /// <param name="node">Top-level component list to render.</param>
    public void DrawInterface(List<Component> node)
    {
        for (byte i = 0; i < node.Count; i++)
            if (node[i].Visible)
            {
                switch (node[i])
                {
                    case Panel panel: DrawPanel(panel); break;
                    case TextBox textBox: DrawTextBox(textBox); break;
                    case Button button: DrawButton(button); break;
                    case CheckBox checkBox: DrawCheckBox(checkBox); break;
                }

                DrawInterfaceSpecific(node[i]);

                DrawInterface(node[i].Children);
            }
    }

    private void DrawButton(Button tool)
    {
        byte alpha = tool.ButtonState switch
        {
            ButtonState.Above => 250,
            ButtonState.Click => 200,
            _ => 225
        };

        renderer.Draw(Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, alpha));
    }

    private void DrawPanel(Panel tool)
    {
        renderer.Draw(Textures.Panels[tool.TextureNum], tool.Position);
    }

    private void DrawCheckBox(CheckBox tool)
    {
        var recSource = new Rectangle(new Point(),
            new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        if (tool.Checked) recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        renderer.Draw(Textures.CheckBox, recSource, recDestiny);
        renderer.DrawText(tool.Text,
            recDestiny.Location.X + Textures.CheckBox.ToSize().Width / 2 +
            CheckBox.Margin, recDestiny.Location.Y + 1, Color.White);
    }

    private void DrawTextBox(TextBox tool)
    {
        var position = tool.Position;
        var text = tool.Text;

        renderer.DrawBox(Textures.TextBox, 3, tool.Position,
            new Size(tool.Width, Textures.TextBox.ToSize().Height));

        if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

        text = TextBreak(text, tool.Width - 10);

        if (TextBox.Focused != null &&
            TextBox.Focused == tool && TextBoxesEvents.Signal) text += "|";
        renderer.DrawText(text, position.X + 4, position.Y + 2, Color.White);
    }

    private void DrawInterfaceSpecific(Component tool)
    {
        if (tool is Panel panel)
            switch (panel.Name)
            {
                case "SelectCharacter": DrawSelectCharacterClass(); break;
                case "CreateCharacter": DrawCreateCharacterClass(); break;
                case "Hotbar": DrawHotbar(panel); break;
                case "Menu_Character": DrawMenuCharacter(panel); break;
                case "Menu_Inventory": MenuInventory(panel); break;
                case "Bars": DrawBars(panel); break;
                case "Information": DrawInformation(panel); break;
                case "Party_Invitation": DrawPartyInvitation(panel); break;
                case "Trade_Invitation": DrawTradeInvitation(panel); break;
                case "Trade": DrawTrade(panel); break;
                case "Shop": DrawShop(panel); break;
            }
    }

    private void DrawBars(Panel tool)
    {
        var hpPercentage = Player.Me.Vital[(byte)Vital.Hp] / (decimal)Player.Me.MaxVital[(byte)Vital.Hp];
        var mpPercentage = Player.Me.Vital[(byte)Vital.Mp] / (decimal)Player.Me.MaxVital[(byte)Vital.Mp];
        var expPercentage = Player.Me.Experience / (decimal)Player.Me.ExpNeeded;

        renderer.Draw(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 6, 0, 0,
            (int)(Textures.BarsPanel.Size.X * hpPercentage), 17);
        renderer.Draw(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 24, 0, 18,
            (int)(Textures.BarsPanel.Size.X * mpPercentage), 17);
        renderer.Draw(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 42, 0, 36,
            (int)(Textures.BarsPanel.Size.X * expPercentage), 17);

        renderer.DrawText("HP", tool.Position.X + 10, tool.Position.Y + 3, Color.White);
        renderer.DrawText("MP", tool.Position.X + 10, tool.Position.Y + 21, Color.White);
        renderer.DrawText("Exp", tool.Position.X + 10, tool.Position.Y + 39, Color.White);

        renderer.DrawText(Player.Me.Vital[(byte)Vital.Hp] + "/" + Player.Me.MaxVital[(byte)Vital.Hp],
            tool.Position.X + 76,
            tool.Position.Y + 7, Color.White, TextAlign.Center);
        renderer.DrawText(Player.Me.Vital[(byte)Vital.Mp] + "/" + Player.Me.MaxVital[(byte)Vital.Mp],
            tool.Position.X + 76,
            tool.Position.Y + 25, Color.White, TextAlign.Center);
        renderer.DrawText(Player.Me.Experience + "/" + Player.Me.ExpNeeded, tool.Position.X + 76,
            tool.Position.Y + 43,
            Color.White, TextAlign.Center);
    }

    /// <summary>
    /// Render chat messages and prompt if chat is not focused.
    /// </summary>
    public void DrawChat()
    {
        var tool = Panels.Chat;
        tool.Visible = TextBox.Focused != null &&
                       TextBox.Focused.Name.Equals("Chat");

        if (tool.Visible || GameLoop.ChatTimer >= Environment.TickCount && Options.Chat)
            for (var i = UI.Chat.LinesFirst; i <= UI.Chat.LinesVisible + UI.Chat.LinesFirst; i++)
                if (UI.Chat.Order.Count > i)
                    renderer.DrawText(UI.Chat.Order[i].Text, 16, 461 + 11 * (i - UI.Chat.LinesFirst),
                        UI.Chat.Order[i].Color);

        if (!tool.Visible)
            renderer.DrawText("Press [Enter] to open chat.", TextBoxes.Chat.Position.X + 5,
                TextBoxes.Chat.Position.Y + 3,
                Color.White);
    }

    private void DrawInformation(Panel tool)
    {
        var item = Item.List.Get(PanelsEvents.InformationId);
        var data = new List<string>();

        if (item == null) return;

        var textColor = item.Rarity switch
        {
            Rarity.Uncommon => new Color(204, 255, 153),
            Rarity.Rare => new Color(102, 153, 255),
            Rarity.Epic => new Color(153, 0, 204),
            Rarity.Legendary => new Color(255, 255, 77),
            _ => new Color()
        };

        renderer.DrawText(item.Name, tool.Position.X + 41, tool.Position.Y + 6, textColor, TextAlign.Center);
        renderer.DrawText(item.Description, tool.Position.X + 82, tool.Position.Y + 20, Color.White, 86);
        renderer.Draw(Textures.Items[item.Texture],
            new Rectangle(tool.Position.X + 9, tool.Position.Y + 21, 64, 64));

        if (Panels.Shop.Visible)
            if (PanelsEvents.ShopSlot >= 0)
                data.Add("Price: " + PanelsEvents.ShopOpen.Sold[PanelsEvents.ShopSlot].Price);
            else if (PanelsEvents.InventorySlot > 0)
                if (PanelsEvents.ShopOpen.FindBought(item) != null)
                    data.Add("Sale price: " + PanelsEvents.ShopOpen.FindBought(item).Price);

        switch (item.Type)
        {
            case ItemType.Potion:
                for (byte n = 0; n < (byte)Vital.Count; n++)
                    if (item.PotionVital[n] != 0)
                        data.Add((Vital)n + ": " + item.PotionVital[n]);

                if (item.PotionExperience != 0) data.Add("Experience: " + item.PotionExperience);
                break;

            case ItemType.Equipment:
                if (item.EquipType == (byte)Equipment.Weapon)
                    if (item.WeaponDamage != 0)
                        data.Add("Damage: " + item.WeaponDamage);

                for (byte n = 0; n < (byte)Attribute.Count; n++)
                    if (item.EquipAttribute[n] != 0)
                        data.Add((Attribute)n + ": " + item.EquipAttribute[n]);
                break;
        }

        Point[] positions =
        {
            new(tool.Position.X + 10, tool.Position.Y + 90), new(tool.Position.X + 10, tool.Position.Y + 102),
            new(tool.Position.X + 10, tool.Position.Y + 114), new(tool.Position.X + 96, tool.Position.Y + 90),
            new(tool.Position.X + 96, tool.Position.Y + 102), new(tool.Position.X + 96, tool.Position.Y + 114),
            new(tool.Position.X + 96, tool.Position.Y + 126)
        };
        for (byte i = 0; i < data.Count; i++)
            renderer.DrawText(data[i], positions[i].X, positions[i].Y, Color.White);
    }

    private void DrawHotbar(Panel tool)
    {
        var indicator = string.Empty;

        for (byte i = 0; i < MaxHotbar; i++)
        {
            var slot = Player.Me.Hotbar[i].Slot;
            if (slot > 0)
                switch (Player.Me.Hotbar[i].Type)
                {
                    case SlotType.Item:
                        itemRenderer.DrawItem(Player.Me.Inventory[slot].Item, 1, tool.Position + new Size(8, 6),
                            (byte)(i + 1),
                            10); break;
                }

            if (i < 9) indicator = (i + 1).ToString();
            else if (i == 9) indicator = "0";
            renderer.DrawText(indicator, tool.Position.X + 16 + 36 * i, tool.Position.Y + 22, Color.White);
        }

        if (PanelsEvents.HotbarChange >= 0)
            if (Player.Me.Hotbar[PanelsEvents.HotbarChange].Type == SlotType.Item)
                renderer.Draw(
                    Textures.Items[Player.Me.Inventory[Player.Me.Hotbar[PanelsEvents.HotbarChange].Slot].Item.Texture],
                    new Point(InputManager.Instance.MousePosition.X + 6,
                        InputManager.Instance.MousePosition.Y + 6));
    }

    private void DrawMenuCharacter(Panel tool)
    {
        renderer.DrawText(Player.Me.Name, tool.Position.X + 18, tool.Position.Y + 52, Color.White);
        renderer.DrawText(Player.Me.Level.ToString(), tool.Position.X + 18, tool.Position.Y + 79, Color.White);
        renderer.Draw(Textures.Faces[Player.Me.TextureNum],
            new Point(tool.Position.X + 82, tool.Position.Y + 37));

        renderer.DrawText("Strength: " + Player.Me.Attribute[(byte)Attribute.Strength], tool.Position.X + 32,
            tool.Position.Y + 146, Color.White);
        renderer.DrawText("Resistance: " + Player.Me.Attribute[(byte)Attribute.Resistance],
            tool.Position.X + 32,
            tool.Position.Y + 162, Color.White);
        renderer.DrawText("Intelligence: " + Player.Me.Attribute[(byte)Attribute.Intelligence],
            tool.Position.X + 32,
            tool.Position.Y + 178, Color.White);
        renderer.DrawText("Agility: " + Player.Me.Attribute[(byte)Attribute.Agility], tool.Position.X + 32,
            tool.Position.Y + 194, Color.White);
        renderer.DrawText("Vitality: " + Player.Me.Attribute[(byte)Attribute.Vitality], tool.Position.X + 32,
            tool.Position.Y + 210, Color.White);
        renderer.DrawText("Points: " + Player.Me.Points, tool.Position.X + 14, tool.Position.Y + 228,
            Color.White);

        for (byte i = 0; i < (byte)Equipment.Count; i++)
            if (Player.Me.Equipment[i] == null)
                renderer.Draw(Textures.Equipments, tool.Position.X + 7 + i * 34, tool.Position.Y + 247, i * 34,
                    0, 32,
                    32);
            else
                renderer.Draw(Textures.Items[Player.Me.Equipment[i].Texture], tool.Position.X + 8 + i * 35,
                    tool.Position.Y + 247, 0, 0, 34, 34);
    }

    private void MenuInventory(Panel tool)
    {
        byte numColumns = 5;

        for (byte i = 0; i < MaxInventory; i++)
            itemRenderer.DrawItem(Player.Me.Inventory[i].Item, Player.Me.Inventory[i].Amount,
                tool.Position + new Size(7, 30), i,
                numColumns);

        if (PanelsEvents.InventoryChange > 0)
            renderer.Draw(Textures.Items[Player.Me.Inventory[PanelsEvents.InventoryChange].Item.Texture],
                new Point(InputManager.Instance.MousePosition.X + 6,
                    InputManager.Instance.MousePosition.Y + 6));
    }

    private void DrawPartyInvitation(Panel tool)
    {
        renderer.DrawText(PanelsEvents.PartyInvitation + " has invite you to a party. Would you like to join?",
            tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
    }

    /// <summary>
    /// Render the party member bars and names.
    /// </summary>
    public void DrawParty()
    {
        for (byte i = 0; i < Player.Me.Party.Length; i++)
        {
            renderer.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 0, 82, 8);
            renderer.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 0, 82, 8);
            if (Player.Me.Party[i].Vital[(byte)Vital.Hp] > 0)
                renderer.Draw(Textures.PartyBars, 10, 92 + 27 * i, 0, 8,
                    Player.Me.Party[i].Vital[(byte)Vital.Hp] * 82 / Player.Me.Party[i].MaxVital[(byte)Vital.Hp],
                    8);
            if (Player.Me.Party[i].Vital[(byte)Vital.Mp] > 0)
                renderer.Draw(Textures.PartyBars, 10, 99 + 27 * i, 0, 16,
                    Player.Me.Party[i].Vital[(byte)Vital.Mp] * 82 / Player.Me.Party[i].MaxVital[(byte)Vital.Mp],
                    8);

            renderer.DrawText(Player.Me.Party[i].Name, 10, 79 + 27 * i, Color.White);
        }
    }

    private void DrawTradeInvitation(Panel tool)
    {
        renderer.DrawText(PanelsEvents.TradeInvitation + " has invite you to a trade. Would you like to join?",
            tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
    }

    private void DrawTrade(Panel tool)
    {
        for (byte i = 0; i < MaxInventory; i++)
        {
            itemRenderer.DrawItem(Player.Me.TradeOffer[i].Item, Player.Me.TradeOffer[i].Amount,
                tool.Position + new Size(7, 50), i, 5);
            itemRenderer.DrawItem(Player.Me.TradeTheirOffer[i].Item, Player.Me.TradeTheirOffer[i].Amount,
                tool.Position + new Size(192, 50), i, 5);
        }
    }

    private void DrawShop(Panel tool)
    {
        var name = PanelsEvents.ShopOpen.Name;
        renderer.DrawText(name, tool.Position.X + 131, tool.Position.Y + 28, Color.White, TextAlign.Center);
        renderer.DrawText("Currency: " + PanelsEvents.ShopOpen.Currency.Name, tool.Position.X + 10,
            tool.Position.Y + 195,
            Color.White);

        for (byte i = 0; i < PanelsEvents.ShopOpen.Sold.Count; i++)
            itemRenderer.DrawItem(PanelsEvents.ShopOpen.Sold[i].Item, PanelsEvents.ShopOpen.Sold[i].Amount,
                tool.Position + new Size(7, 50), (byte)(i + 1), 7);
    }


    private void DrawSelectCharacterClass()
    {
        var textPosition = new Point(399, 425);
        var text = "(" + (PanelsEvents.SelectCharacter + 1) + ") None";

        if (!ButtonsEvents.Characters_Change_Buttons())
        {
            renderer.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        if (PanelsEvents.SelectCharacter >= PanelsEvents.Characters.Length)
        {
            renderer.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        var textureNum = PanelsEvents.Characters[PanelsEvents.SelectCharacter].TextureNum;
        if (textureNum > 0)
        {
            renderer.Draw(Textures.Faces[textureNum], new Point(353, 442));
            characterRenderer.DrawCharacter(textureNum,
                new Point(356, 534 - Textures.Characters[textureNum].ToSize().Height / 4),
                Direction.Down, AnimationStopped);
        }

        text = "(" + (PanelsEvents.SelectCharacter + 1) + ") " +
               PanelsEvents.Characters[PanelsEvents.SelectCharacter].Name;
        renderer.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
    }

    private void DrawCreateCharacterClass()
    {
        short textureNum = 0;
        var @class = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value;

        if (CheckBoxes.GenderMale.Checked && @class.TextureMale.Count > 0)
            textureNum = @class.TextureMale[PanelsEvents.CreateCharacterTex];
        else if (@class.TextureFemale.Count > 0)
            textureNum = @class.TextureFemale[PanelsEvents.CreateCharacterTex];

        if (textureNum > 0)
        {
            renderer.Draw(Textures.Faces[textureNum], new Point(425, 440));
            characterRenderer.DrawCharacter(textureNum, new Point(433, 501), Direction.Down, AnimationStopped);
        }

        var text = @class.Name;
        renderer.DrawText(text, 347, 509, Color.White, TextAlign.Center);

        renderer.DrawText(@class.Description, 282, 526, Color.White, 123);
    }
}
