using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Network;
using CryBits.Entities;
using SFML.Window;
using static CryBits.Client.Logic.Utils;

namespace CryBits.Client.UI
{
    internal class Panels : Tools.Structure
    {
        // Armazenamento dos dados da ferramenta
        public static Dictionary<string, Panels> List = new Dictionary<string, Panels>();

        // Dados
        public byte TextureNum;

        // Dados temporários
        public static byte CreateCharacterClass = 0;
        public static byte CreateCharacterTex = 0;
        public static int SelectCharacter = 1;
        public static Guid InformationID;
        public static byte DropSlot;
        public static string PartyInvitation;
        public static string TradeInvitation;
        public static byte TradeInventorySlot = 0;
        public static Shop ShopOpen;
        public static byte ShopInventorySlot;
        public static short HotbarChange;
        public static byte InventoryChange;
        public static TempCharacter[] Characters;
        public struct TempCharacter
        {
            public string Name;
            public short TextureNum;
        }

        public static void MenuClose()
        {
            // Fecha todos os paineis abertos
            List["Connect"].Visible = false;
            List["Register"].Visible = false;
            List["Options"].Visible = false;
            List["SelectCharacter"].Visible = false;
            List["CreateCharacter"].Visible = false;
        }

        // Retorna em qual slot o mouse está sobrepondo
        public static byte InventorySlot => Slot(List["Menu_Inventory"], 7, 29, 6, 5);
        public static short HotbarSlot => (short)(Slot(List["Hotbar"], 8, 6, 1, 10) - 1);
        public static byte TradeSlot => Slot(List["Trade"], 7, 50, 6, 5);
        public static short ShopSlot => (short)(Slot(List["Shop"], 7, 50, 4, 7) - 1);
        public static short EquipmentSlot => (short)(Slot(List["Menu_Character"], 7, 248, 1, 5) - 1);

        public static void Inventory_MouseDown(MouseButtonEventArgs e)
        {
            byte slot = InventorySlot;

            // Somente se necessário
            if (slot == 0) return;
            if (Player.Me.Inventory[slot].Item == null) return;

            // Solta item
            if (e.Button == Mouse.Button.Right)
            {
                if (Player.Me.Inventory[slot].Item.Bind != BindOn.Pickup)
                    // Vende o item
                    if (List["Shop"].Visible)
                    {
                        if (Player.Me.Inventory[slot].Amount != 1)
                        {
                            ShopInventorySlot = slot;
                            TextBoxes.List["Shop_Sell_Amount"].Text = string.Empty;
                            List["Shop_Sell"].Visible = true;
                        }
                        else Send.ShopSell(slot, 1);
                    }
                    // Solta o item
                    else if (!List["Trade"].Visible)
                        if (Player.Me.Inventory[slot].Amount != 1)
                        {
                            DropSlot = slot;
                            TextBoxes.List["Drop_Amount"].Text = string.Empty;
                            List["Drop"].Visible = true;
                        }
                        else Send.DropItem(slot, 1);
            }
            // Seleciona o item
            else if (e.Button == Mouse.Button.Left) InventoryChange = slot;
        }

        public static void Equipment_MouseDown(MouseButtonEventArgs e)
        {
            Point panelPosition = List["Menu_Character"].Position;

            for (byte i = 0; i < (byte)Equipments.Count; i++)
                if (IsAbove(new Rectangle(panelPosition.X + 7 + i * 36, panelPosition.Y + 247, 32, 32)))
                    // Remove o equipamento
                    if (e.Button == Mouse.Button.Right)
                        if (Player.Me.Equipment[i].Bind != BindOn.Equip)
                        {
                            Send.EquipmentRemove(i);
                            return;
                        }
        }

        public static void Hotbar_MouseDown(MouseButtonEventArgs e)
        {
            short slot = HotbarSlot;

            // Somente se necessário
            if (slot < 0) return;
            if (Player.Me.Hotbar[slot].Slot == 0) return;

            // Solta item
            if (e.Button == Mouse.Button.Right)
            {
                Send.HotbarAdd(slot, 0, 0);
                return;
            }
            // Seleciona o item
            if (e.Button == Mouse.Button.Left)
            {
                HotbarChange = slot;
            }
        }

        public static void Trade_MouseDown(MouseButtonEventArgs e)
        {
            byte slot = TradeSlot;

            // Somente se necessário
            if (!List["Trade"].Visible) return;
            if (slot == 0) return;
            if (Player.Me.TradeOffer[slot].Item == null) return;

            // Solta item
            if (e.Button == Mouse.Button.Right) Send.TradeOffer(slot, 0);
        }

        public static void CheckInformation()
        {
            Point position = new Point();

            // Define as informações do painel com base no que o Window.Mouse está sobrepondo
            if (HotbarSlot >= 0)
            {
                position = List["Hotbar"].Position + new Size(0, 42);
                InformationID = Player.Me.Inventory[Player.Me.Hotbar[HotbarSlot].Slot].Item.ID;
            }
            else if (InventorySlot > 0)
            {
                position = List["Menu_Inventory"].Position + new Size(-186, 3);
                InformationID = Player.Me.Inventory[InventorySlot].Item.ID;
            }
            else if (EquipmentSlot >= 0)
            {
                position = List["Menu_Character"].Position + new Size(-186, 5);
                InformationID = Player.Me.Equipment[EquipmentSlot].ID;
            }
            else if (ShopSlot >= 0 && ShopSlot < ShopOpen.Sold.Count)
            {
                position = new Point(List["Shop"].Position.X - 186, List["Shop"].Position.Y + 5);
                InformationID = ShopOpen.Sold[ShopSlot].Item.ID;
            }
            else InformationID = Guid.Empty;

            // Define os dados do painel de informações
            List["Information"].Visible = !position.IsEmpty && InformationID != Guid.Empty;
            List["Information"].Position = position;
        }
    }
}