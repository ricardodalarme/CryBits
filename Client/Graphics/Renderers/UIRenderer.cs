using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Client.Logic;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Enums;
using CryBits.Extensions;
using static CryBits.Client.Utils.TextUtils;
using static CryBits.Globals;
using Attribute = CryBits.Enums.Attribute;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class UIRenderer
{
    private static GameContext Ctx => GameContext.Instance;

    // ─── Local-player component helpers ──────────────────────────────────────

    private static int LocalId => Ctx.GetLocalPlayer();

    private static T? Local<T>() where T : class, IComponent
    {
        var id = LocalId;
        return id > 0 && Ctx.World.TryGet<T>(id, out var c) ? c : null;
    }

    /// <summary>
    /// Recursively render a tree of UI components.
    /// </summary>
    /// <param name="node">Top-level component list to render.</param>
    public static void Interface(List<Component> node)
    {
        for (byte i = 0; i < node.Count; i++)
            if (node[i].Visible)
            {
                switch (node[i])
                {
                    case Panel panel: Panel(panel); break;
                    case TextBox textBox: TextBox(textBox); break;
                    case Button button: Button(button); break;
                    case CheckBox checkBox: CheckBox(checkBox); break;
                }

                InterfaceSpecific(node[i]);

                Interface(node[i].Children);
            }
    }

    private static void Button(Button tool)
    {
        byte alpha = tool.ButtonState switch
        {
            ButtonState.Above => 250,
            ButtonState.Click => 200,
            _ => 225
        };

        Renders.Render(Textures.Buttons[tool.TextureNum], tool.Position, new Color(255, 255, 225, alpha));
    }

    private static void Panel(Panel tool)
    {
        Renders.Render(Textures.Panels[tool.TextureNum], tool.Position);
    }

    private static void CheckBox(CheckBox tool)
    {
        var recSource = new Rectangle(new Point(),
            new Size(Textures.CheckBox.ToSize().Width / 2, Textures.CheckBox.ToSize().Height));
        var recDestiny = new Rectangle(tool.Position, recSource.Size);

        if (tool.Checked) recSource.Location = new Point(Textures.CheckBox.ToSize().Width / 2, 0);

        Renders.Render(Textures.CheckBox, recSource, recDestiny);
        Renders.DrawText(tool.Text,
            recDestiny.Location.X + Textures.CheckBox.ToSize().Width / 2 +
            Framework.Interfacily.Components.CheckBox.Margin, recDestiny.Location.Y + 1, Color.White);
    }

    private static void TextBox(TextBox tool)
    {
        var position = tool.Position;
        var text = tool.Text;

        Renders.Render_Box(Textures.TextBox, 3, tool.Position, new Size(tool.Width, Textures.TextBox.ToSize().Height));

        if (tool.Password && !string.IsNullOrEmpty(text)) text = new string('•', text.Length);

        text = TextBreak(text, tool.Width - 10);

        if (Framework.Interfacily.Components.TextBox.Focused != null &&
            Framework.Interfacily.Components.TextBox.Focused == tool && TextBoxesEvents.Signal) text += "|";
        Renders.DrawText(text, position.X + 4, position.Y + 2, Color.White);
    }

    private static void InterfaceSpecific(Component tool)
    {
        if (tool is Panel panel)
            switch (panel.Name)
            {
                case "SelectCharacter": SelectCharacterClass(); break;
                case "CreateCharacter": CreateCharacterClass(); break;
                case "Hotbar": Hotbar(panel); break;
                case "Menu_Character": MenuCharacter(panel); break;
                case "Menu_Inventory": MenuInventory(panel); break;
                case "Bars": Bars(panel); break;
                case "Information": Information(panel); break;
                case "Party_Invitation": PartyInvitation(panel); break;
                case "Trade_Invitation": Trade_Invitation(panel); break;
                case "Trade": Trade(panel); break;
                case "Shop": Shop(panel); break;
            }
    }

    private static void Bars(Panel tool)
    {
        var vitals = Local<VitalsComponent>();
        var xp = Local<ExperienceComponent>();
        if (vitals == null) return;

        var hp = vitals.Current[(byte)Vital.Hp];
        var mp = vitals.Current[(byte)Vital.Mp];
        var mhp = vitals.Max[(byte)Vital.Hp];
        var mmp = vitals.Max[(byte)Vital.Mp];

        var hpPercentage = mhp > 0 ? hp / (decimal)mhp : 0m;
        var mpPercentage = mmp > 0 ? mp / (decimal)mmp : 0m;
        var expPercentage = xp?.Needed > 0 ? xp.Current / (decimal)xp.Needed : 0m;

        Renders.Render(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 6, 0, 0, (int)(Textures.BarsPanel.Size.X * hpPercentage), 17);
        Renders.Render(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 24, 0, 18, (int)(Textures.BarsPanel.Size.X * mpPercentage), 17);
        Renders.Render(Textures.BarsPanel, tool.Position.X + 6, tool.Position.Y + 42, 0, 36, (int)(Textures.BarsPanel.Size.X * expPercentage), 17);

        Renders.DrawText("HP", tool.Position.X + 10, tool.Position.Y + 3, Color.White);
        Renders.DrawText("MP", tool.Position.X + 10, tool.Position.Y + 21, Color.White);
        Renders.DrawText("Exp", tool.Position.X + 10, tool.Position.Y + 39, Color.White);

        Renders.DrawText(hp + "/" + mhp, tool.Position.X + 76, tool.Position.Y + 7, Color.White, TextAlign.Center);
        Renders.DrawText(mp + "/" + mmp, tool.Position.X + 76, tool.Position.Y + 25, Color.White, TextAlign.Center);
        Renders.DrawText((xp?.Current ?? 0) + "/" + (xp?.Needed ?? 0), tool.Position.X + 76, tool.Position.Y + 43, Color.White, TextAlign.Center);
    }

    /// <summary>
    /// Render chat messages and prompt if chat is not focused.
    /// </summary>
    public static void Chat()
    {
        var tool = Panels.Chat;
        tool.Visible = Framework.Interfacily.Components.TextBox.Focused != null &&
                       Framework.Interfacily.Components.TextBox.Focused.Name.Equals("Chat");

        if (tool.Visible || Loop.ChatTimer >= Environment.TickCount && Options.Chat)
            for (var i = UI.Chat.LinesFirst; i <= UI.Chat.LinesVisible + UI.Chat.LinesFirst; i++)
                if (UI.Chat.Order.Count > i)
                    Renders.DrawText(UI.Chat.Order[i].Text, 16, 461 + 11 * (i - UI.Chat.LinesFirst),
                        UI.Chat.Order[i].Color);

        if (!tool.Visible)
            Renders.DrawText("Press [Enter] to open chat.", TextBoxes.Chat.Position.X + 5,
                TextBoxes.Chat.Position.Y + 3,
                Color.White);
    }

    private static void Information(Panel tool)
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

        Renders.DrawText(item.Name, tool.Position.X + 41, tool.Position.Y + 6, textColor, TextAlign.Center);
        Renders.DrawText(item.Description, tool.Position.X + 82, tool.Position.Y + 20, Color.White, 86);
        Renders.Render(Textures.Items[item.Texture], new Rectangle(tool.Position.X + 9, tool.Position.Y + 21, 64, 64));

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
        [
            new(tool.Position.X + 10, tool.Position.Y + 90), new(tool.Position.X + 10, tool.Position.Y + 102),
            new(tool.Position.X + 10, tool.Position.Y + 114), new(tool.Position.X + 96, tool.Position.Y + 90),
            new(tool.Position.X + 96, tool.Position.Y + 102), new(tool.Position.X + 96, tool.Position.Y + 114),
            new(tool.Position.X + 96, tool.Position.Y + 126)
        ];
        for (byte i = 0; i < data.Count; i++) Renders.DrawText(data[i], positions[i].X, positions[i].Y, Color.White);
    }

    private static void Hotbar(Panel tool)
    {
        var hotbar = Local<HotbarComponent>();
        var inv = Local<InventoryComponent>();
        if (hotbar == null || inv == null) return;

        var indicator = string.Empty;

        for (byte i = 0; i < MaxHotbar; i++)
        {
            var slot = hotbar.Slots[i].Slot;
            if (slot > 0)
                switch (hotbar.Slots[i].Type)
                {
                    case SlotType.Item:
                        ItemRenderer.Item(inv.Slots[slot]?.Item, 1, tool.Position + new Size(8, 6), (byte)(i + 1), 10);
                        break;
                }

            if (i < 9) indicator = (i + 1).ToString();
            else if (i == 9) indicator = "0";
            Renders.DrawText(indicator, tool.Position.X + 16 + 36 * i, tool.Position.Y + 22, Color.White);
        }

        if (PanelsEvents.HotbarChange >= 0 && hotbar.Slots[PanelsEvents.HotbarChange].Type == SlotType.Item)
        {
            var dragSlot = hotbar.Slots[PanelsEvents.HotbarChange].Slot;
            var dragItem = inv.Slots[dragSlot]?.Item;
            if (dragItem != null)
                Renders.Render(Textures.Items[dragItem.Texture], new Point(UI.Window.Mouse.X + 6, UI.Window.Mouse.Y + 6));
        }
    }

    private static void MenuCharacter(Panel tool)
    {
        var player = Local<PlayerDataComponent>();
        var sprite = Local<CharacterSpriteComponent>();
        var xp = Local<ExperienceComponent>();
        if (player == null) return;

        Renders.DrawText(player.Name, tool.Position.X + 18, tool.Position.Y + 52, Color.White);
        Renders.DrawText(player.Level.ToString(), tool.Position.X + 18, tool.Position.Y + 79, Color.White);
        if (sprite != null && sprite.TextureNum > 0)
            Renders.Render(Textures.Faces[sprite.TextureNum], new Point(tool.Position.X + 82, tool.Position.Y + 37));

        Renders.DrawText("Strength: " + player.Attributes[(byte)Attribute.Strength], tool.Position.X + 32, tool.Position.Y + 146, Color.White);
        Renders.DrawText("Resistance: " + player.Attributes[(byte)Attribute.Resistance], tool.Position.X + 32, tool.Position.Y + 162, Color.White);
        Renders.DrawText("Intelligence: " + player.Attributes[(byte)Attribute.Intelligence], tool.Position.X + 32, tool.Position.Y + 178, Color.White);
        Renders.DrawText("Agility: " + player.Attributes[(byte)Attribute.Agility], tool.Position.X + 32, tool.Position.Y + 194, Color.White);
        Renders.DrawText("Vitality: " + player.Attributes[(byte)Attribute.Vitality], tool.Position.X + 32, tool.Position.Y + 210, Color.White);
        Renders.DrawText("Points: " + (xp?.Points ?? 0), tool.Position.X + 14, tool.Position.Y + 228, Color.White);

        for (byte i = 0; i < (byte)Equipment.Count; i++)
            if (player.EquippedItems[i] == null)
                Renders.Render(Textures.Equipments, tool.Position.X + 7 + i * 34, tool.Position.Y + 247, i * 34, 0, 32, 32);
            else
                Renders.Render(Textures.Items[player.EquippedItems[i]!.Texture], tool.Position.X + 8 + i * 35, tool.Position.Y + 247, 0, 0, 34, 34);
    }

    private static void MenuInventory(Panel tool)
    {
        var inv = Local<InventoryComponent>();
        if (inv == null) return;

        const byte numColumns = 5;

        for (byte i = 0; i < MaxInventory; i++)
            ItemRenderer.Item(inv.Slots[i]?.Item, inv.Slots[i]?.Amount ?? 0,
                tool.Position + new Size(7, 30), i, numColumns);

        if (PanelsEvents.InventoryChange > 0)
        {
            var dragItem = inv.Slots[PanelsEvents.InventoryChange]?.Item;
            if (dragItem != null)
                Renders.Render(Textures.Items[dragItem.Texture], new Point(UI.Window.Mouse.X + 6, UI.Window.Mouse.Y + 6));
        }
    }

    private static void PartyInvitation(Panel tool)
    {
        Renders.DrawText(PanelsEvents.PartyInvitation + " has invite you to a party. Would you like to join?",
            tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
    }

    /// <summary>
    /// Render the party member bars and names.
    /// </summary>
    public static void Party()
    {
        var party = Local<PartyComponent>();
        if (party == null) return;

        for (byte i = 0; i < party.MemberEntityIds.Length; i++)
        {
            var memberId = party.MemberEntityIds[i];
            Ctx.World.TryGet<VitalsComponent>(memberId, out var mv);
            Ctx.World.TryGet<PlayerDataComponent>(memberId, out var mp);

            Renders.Render(Textures.PartyBars, 10, 92 + 27 * i, 0, 0, 82, 8);
            Renders.Render(Textures.PartyBars, 10, 99 + 27 * i, 0, 0, 82, 8);

            if (mv != null)
            {
                var hp = mv.Current[(byte)Vital.Hp];
                var mhp = mv.Max[(byte)Vital.Hp];
                var mp2 = mv.Current[(byte)Vital.Mp];
                var mmp = mv.Max[(byte)Vital.Mp];

                if (hp > 0 && mhp > 0)
                    Renders.Render(Textures.PartyBars, 10, 92 + 27 * i, 0, 8, hp * 82 / mhp, 8);
                if (mp2 > 0 && mmp > 0)
                    Renders.Render(Textures.PartyBars, 10, 99 + 27 * i, 0, 16, mp2 * 82 / mmp, 8);
            }

            if (mp != null)
                Renders.DrawText(mp.Name, 10, 79 + 27 * i, Color.White);
        }
    }

    private static void Trade_Invitation(Panel tool)
    {
        Renders.DrawText(PanelsEvents.TradeInvitation + " has invite you to a trade. Would you like to join?",
            tool.Position.X + 14, tool.Position.Y + 33, Color.White, 160);
    }

    private static void Trade(Panel tool)
    {
        var trade = Local<TradeComponent>();
        if (trade?.Offer == null || trade.TheirOffer == null) return;

        for (byte i = 0; i < MaxInventory; i++)
        {
            ItemRenderer.Item(trade.Offer[i]?.Item, trade.Offer[i]?.Amount ?? 0, tool.Position + new Size(7, 50), i, 5);
            ItemRenderer.Item(trade.TheirOffer[i]?.Item, trade.TheirOffer[i]?.Amount ?? 0, tool.Position + new Size(192, 50), i, 5);
        }
    }

    private static void Shop(Panel tool)
    {
        var name = PanelsEvents.ShopOpen.Name;
        Renders.DrawText(name, tool.Position.X + 131, tool.Position.Y + 28, Color.White, TextAlign.Center);
        Renders.DrawText("Currency: " + PanelsEvents.ShopOpen.Currency.Name, tool.Position.X + 10,
            tool.Position.Y + 195,
            Color.White);

        for (byte i = 0; i < PanelsEvents.ShopOpen.Sold.Count; i++)
            ItemRenderer.Item(PanelsEvents.ShopOpen.Sold[i].Item, PanelsEvents.ShopOpen.Sold[i].Amount,
                tool.Position + new Size(7, 50), (byte)(i + 1), 7);
    }


    private static void SelectCharacterClass()
    {
        var textPosition = new Point(399, 425);
        var text = "(" + (PanelsEvents.SelectCharacter + 1) + ") None";

        if (!ButtonsEvents.Characters_Change_Buttons())
        {
            Renders.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        if (PanelsEvents.SelectCharacter >= PanelsEvents.Characters.Length)
        {
            Renders.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
            return;
        }

        var textureNum = PanelsEvents.Characters[PanelsEvents.SelectCharacter].TextureNum;
        if (textureNum > 0)
        {
            Renders.Render(Textures.Faces[textureNum], new Point(353, 442));
            CharacterRenderer.Character(textureNum,
                new Point(356, 534 - Textures.Characters[textureNum].ToSize().Height / 4),
                Direction.Down, AnimationStopped);
        }

        text = "(" + (PanelsEvents.SelectCharacter + 1) + ") " +
               PanelsEvents.Characters[PanelsEvents.SelectCharacter].Name;
        Renders.DrawText(text, textPosition.X, textPosition.Y, Color.White, TextAlign.Center);
    }

    private static void CreateCharacterClass()
    {
        short textureNum = 0;
        var @class = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value;

        if (CheckBoxes.GenderMale.Checked && @class.TextureMale.Count > 0)
            textureNum = @class.TextureMale[PanelsEvents.CreateCharacterTex];
        else if (@class.TextureFemale.Count > 0)
            textureNum = @class.TextureFemale[PanelsEvents.CreateCharacterTex];

        if (textureNum > 0)
        {
            Renders.Render(Textures.Faces[textureNum], new Point(425, 440));
            CharacterRenderer.Character(textureNum, new Point(433, 501), Direction.Down, AnimationStopped);
        }

        var text = @class.Name;
        Renders.DrawText(text, 347, 509, Color.White, TextAlign.Center);

        Renders.DrawText(@class.Description, 282, 526, Color.White, 123);
    }
}
