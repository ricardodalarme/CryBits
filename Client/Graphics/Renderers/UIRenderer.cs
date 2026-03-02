using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.UI.Game;
using CryBits.Client.UI.Game.Views;
using CryBits.Client.UI.Menu.Views;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class UIRenderer(
    Renderer renderer,
    ToolsRenderer toolsRenderer,
    CharacterRenderer characterRenderer,
    ItemRenderer itemRenderer
)
{
    public static UIRenderer Instance { get; } = new(Renderer.Instance, ToolsRenderer.Instance, CharacterRenderer.Instance, ItemRenderer.Instance);

    /// <summary>
    /// Recursively render a tree of UI components.
    /// </summary>
    /// <param name="node">Top-level component list to render.</param>
    public void DrawInterface(List<Component> node)
    {
        foreach (Component tool in node)
            if (tool.Visible)
            {
                switch (tool)
                {
                    case Label label: toolsRenderer.DrawLabel(label); break;
                    case Panel panel: toolsRenderer.DrawPanel(panel); break;
                    case TextBox textBox: toolsRenderer.DrawTextBox(textBox); break;
                    case Button button: toolsRenderer.DrawButton(button); break;
                    case CheckBox checkBox: toolsRenderer.DrawCheckBox(checkBox); break;
                    case ProgressBar progressBar: toolsRenderer.DrawProgressBar(progressBar); break;
                }

                DrawInterfaceSpecific(tool);
                DrawInterface(tool.Children);
            }
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
                case "Information": DrawInformation(panel); break;
                case "Trade": DrawTrade(panel); break;
                case "Shop": DrawShop(panel); break;
            }
    }

    /// <summary>
    /// Render chat messages and prompt if chat is not focused.
    /// </summary>
    public void DrawChat()
    {
        var tool = ChatView.Panel;
        tool.Visible = TextBox.Focused != null && TextBox.Focused.Name.Equals("Chat");

        if (tool.Visible || GameLoop.ChatTimer >= Environment.TickCount && Options.Chat)
            for (var i = Chat.LinesFirst; i <= Chat.LinesVisible + Chat.LinesFirst; i++)
                if (Chat.Order.Count > i)
                    renderer.DrawText(Chat.Order[i].Text, 16, 461 + 11 * (i - Chat.LinesFirst),
                        Chat.Order[i].Color);

        if (!tool.Visible)
            renderer.DrawText("Press [Enter] to open chat.", ChatView.MessageTextBox.Position.X + 5,
                ChatView.MessageTextBox.Position.Y + 3,
                Color.White);
    }

    private void DrawInformation(Panel tool)
    {
        var item = Item.List.Get(InformationView.CurrentId);
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

        if (ShopView.Panel.Visible)
        {
            var shopSlot = Tools.SlotGrids["Shop_Grid"].GetSlotIndex();
            var inventorySlot = Tools.SlotGrids["Inventory_Grid"].GetSlotIndex();
            if (shopSlot >= 0)
                data.Add("Price: " + ShopView.OpenedShop.Sold[shopSlot].Price);
            else if (inventorySlot > 0)
                if (ShopView.OpenedShop.FindBought(item) != null)
                    data.Add("Sale price: " + ShopView.OpenedShop.FindBought(item).Price);
        }

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
        var grid = Tools.SlotGrids["Hotbar_Grid"];

        for (byte i = 0; i < grid.SlotCount; i++)
        {
            var slot = Player.Me.Hotbar[i].Slot;
            if (slot > 0 && Player.Me.Hotbar[i].Type == SlotType.Item)
                itemRenderer.DrawItem(Player.Me.Inventory[slot].Item, 1, grid.GetSlotPosition(i));

            var indicator = i < 9 ? (i + 1).ToString() : "0";
            renderer.DrawText(indicator, tool.Position.X + 16 + 36 * i, tool.Position.Y + 22, Color.White);
        }

        if (GameScreen.HotbarChange >= 0)
            if (Player.Me.Hotbar[GameScreen.HotbarChange].Type == SlotType.Item)
                renderer.Draw(
                    Textures.Items[Player.Me.Inventory[Player.Me.Hotbar[GameScreen.HotbarChange].Slot].Item.Texture],
                    new Point(InputManager.Instance.MousePosition.X + 6,
                        InputManager.Instance.MousePosition.Y + 6));
    }

    private void DrawMenuCharacter(Panel tool)
    {
        renderer.Draw(Textures.Faces[Player.Me.TextureNum],
            new Point(tool.Position.X + 82, tool.Position.Y + 37));

        var grid = Tools.SlotGrids["Equipment_Grid"];
        for (byte i = 0; i < grid.SlotCount; i++)
        {
            var pos = grid.GetSlotPosition(i);
            if (Player.Me.Equipment[i] == null)
                renderer.Draw(Textures.Equipments, pos.X, pos.Y, i * 34, 0, 32, 32);
            else
                renderer.Draw(Textures.Items[Player.Me.Equipment[i].Texture], pos);
        }
    }

    private void MenuInventory(Panel tool)
    {
        var grid = Tools.SlotGrids["Inventory_Grid"];
        for (byte i = 0; i < grid.SlotCount; i++)
            itemRenderer.DrawItem(Player.Me.Inventory[i].Item, Player.Me.Inventory[i].Amount, grid.GetSlotPosition(i));

        if (GameScreen.InventoryChange > 0)
            renderer.Draw(Textures.Items[Player.Me.Inventory[GameScreen.InventoryChange].Item.Texture],
                new Point(InputManager.Instance.MousePosition.X + 6,
                    InputManager.Instance.MousePosition.Y + 6));
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

    private void DrawTrade(Panel tool)
    {
        var own = Tools.SlotGrids["Trade_Grid_Own"];
        var their = Tools.SlotGrids["Trade_Grid_Their"];
        for (byte i = 0; i < own.SlotCount; i++)
        {
            itemRenderer.DrawItem(Player.Me.TradeOffer[i].Item, Player.Me.TradeOffer[i].Amount, own.GetSlotPosition(i));
            itemRenderer.DrawItem(Player.Me.TradeTheirOffer[i].Item, Player.Me.TradeTheirOffer[i].Amount, their.GetSlotPosition(i));
        }
    }

    private void DrawShop(Panel tool)
    {
        var grid = Tools.SlotGrids["Shop_Grid"];
        for (var i = 0; i < Math.Min(grid.SlotCount, ShopView.OpenedShop.Sold.Count); i++)
            itemRenderer.DrawItem(ShopView.OpenedShop.Sold[i].Item, ShopView.OpenedShop.Sold[i].Amount, grid.GetSlotPosition(i));
    }

    private void DrawSelectCharacterClass()
    {
        var textPosition = new Point(399, 425);
        var index = SelectCharacterView.CurrentCharacter + 1;
        var text = $"({index}) None";

        if (!SelectCharacterView.UpdateButtonVisibility())
        {
            renderer.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        if (SelectCharacterView.CurrentCharacter >= SelectCharacterView.Characters.Length)
        {
            renderer.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        var textureNum = SelectCharacterView.Characters[SelectCharacterView.CurrentCharacter].TextureNum;
        if (textureNum > 0)
        {
            renderer.Draw(Textures.Faces[textureNum], new Point(353, 442));
            characterRenderer.DrawCharacter(textureNum,
                new Point(356, 534 - Textures.Characters[textureNum].ToSize().Height / 4),
                Direction.Down, AnimationStopped);
        }

        text = $"({index}) {SelectCharacterView.Characters[SelectCharacterView.CurrentCharacter].Name}";
        renderer.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
    }

    private void DrawCreateCharacterClass()
    {
        short textureNum = 0;
        var @class = Class.List.ElementAt(CreateCharacterView.CurrentClass).Value;

        if (CreateCharacterView.GenderMaleCheckBox.Checked && @class.TextureMale.Count > 0)
            textureNum = @class.TextureMale[CreateCharacterView.CurrentTexture];
        else if (@class.TextureFemale.Count > 0)
            textureNum = @class.TextureFemale[CreateCharacterView.CurrentTexture];

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
