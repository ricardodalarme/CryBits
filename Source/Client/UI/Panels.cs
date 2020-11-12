using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Client.Entities;
using CryBits.Client.Network;
using static CryBits.Client.Logic.Utils;

namespace CryBits.Client.UI
{
    class Panels : Tools.Structure
    {
        // Armazenamento dos dados da ferramenta
        public static Dictionary<string, Panels> List = new Dictionary<string, Panels>();

        // Dados
        public byte Texture_Num;

        // Dados temporários
        public static byte CreateCharacter_Class = 0;
        public static byte CreateCharacter_Tex = 0;
        public static int SelectCharacter = 1;
        public static Guid Infomation_ID;
        public static byte Drop_Slot = 0;
        public static string Party_Invitation;
        public static string Trade_Invitation;
        public static byte Trade_Slot_Selected = 0;
        public static byte Trade_Inventory_Slot = 0;
        public static Shop Shop_Open;
        public static byte Shop_Inventory_Slot = 0;
        public static short Hotbar_Change;
        public static byte Inventory_Change;
        public static TempCharacter[] Characters;
        public struct TempCharacter
        {
            public string Name;
            public short Level;
            public short Texture_Num;
        }

        public static void Menu_Close()
        {
            // Fecha todos os paineis abertos
            List["Connect"].Visible = false;
            List["Register"].Visible = false;
            List["Options"].Visible = false;
            List["SelectCharacter"].Visible = false;
            List["CreateCharacter"].Visible = false;
        }

        // Retorna em qual slot o mouse está sobrepondo
        public static byte Inventory_Slot => Slot(List["Menu_Inventory"], 7, 29, 6, 5);
        public static short Hotbar_Slot => (short)(Slot(List["Hotbar"], 8, 6, 1, 10) - 1);
        public static byte Trade_Slot => Slot(List["Trade"], 7, 50, 6, 5);
        public static short Shop_Slot => (short)(Slot(List["Shop"], 7, 50, 4, 7) - 1);
        public static short Equipment_Slot => (short)(Slot(List["Menu_Character"], 7, 248, 1, 5) - 1);

        public static void Inventory_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            byte slot = Inventory_Slot;

            // Somente se necessário
            if (slot == 0) return;
            if (Player.Me.Inventory[slot].Item == null) return;

            // Solta item
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                if (Player.Me.Inventory[slot].Item.Bind != BindOn.Pickup)
                    // Vende o item
                    if (List["Shop"].Visible)
                    {
                        if (Player.Me.Inventory[slot].Amount != 1)
                        {
                            Shop_Inventory_Slot = slot;
                            TextBoxes.List["Shop_Sell_Amount"].Text = string.Empty;
                            List["Shop_Sell"].Visible = true;
                        }
                        else Send.Shop_Sell(slot, 1);
                    }
                    // Solta o item
                    else if (!List["Trade"].Visible)
                        if (Player.Me.Inventory[slot].Amount != 1)
                        {
                            Drop_Slot = slot;
                            TextBoxes.List["Drop_Amount"].Text = string.Empty;
                            List["Drop"].Visible = true;
                        }
                        else Send.DropItem(slot, 1);
            }
            // Seleciona o item
            else if (e.Button == SFML.Window.Mouse.Button.Left) Inventory_Change = slot;
        }

        public static void Equipment_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            Point panelPosition = List["Menu_Character"].Position;

            for (byte i = 0; i < (byte)Equipments.Count; i++)
                if (IsAbove(new Rectangle(panelPosition.X + 7 + i * 36, panelPosition.Y + 247, 32, 32)))
                    // Remove o equipamento
                    if (e.Button == SFML.Window.Mouse.Button.Right)
                        if (Player.Me.Equipment[i].Bind != BindOn.Equip)
                        {
                            Send.Equipment_Remove(i);
                            return;
                        }
        }

        public static void Hotbar_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            short slot = Hotbar_Slot;

            // Somente se necessário
            if (slot < 0) return;
            if (Player.Me.Hotbar[slot].Slot == 0) return;

            // Solta item
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                Send.Hotbar_Add(slot, 0, 0);
                return;
            }
            // Seleciona o item
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                Panels.Hotbar_Change = slot;
                return;
            }
        }

        public static void Trade_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            byte slot = Trade_Slot;

            // Somente se necessário
            if (!List["Trade"].Visible) return;
            if (slot == 0) return;
            if (Player.Me.Trade_Offer[slot].Item == null) return;

            // Solta item
            if (e.Button == SFML.Window.Mouse.Button.Right) Send.Trade_Offer(slot, 0);
        }

        public static void CheckInformations()
        {
            Point position = new Point();

            // Define as informações do painel com base no que o Window.Mouse está sobrepondo
            if (Hotbar_Slot >= 0)
            {
                position = List["Hotbar"].Position + new Size(0, 42);
                Infomation_ID = Player.Me.Inventory[Player.Me.Hotbar[Hotbar_Slot].Slot].Item.ID;
            }
            else if (Inventory_Slot > 0)
            {
                position = List["Menu_Inventory"].Position + new Size(-186, 3);
                Infomation_ID = Player.Me.Inventory[Inventory_Slot].Item.ID;
            }
            else if (Equipment_Slot >= 0)
            {
                position = List["Menu_Character"].Position + new Size(-186, 5);
                Infomation_ID = Player.Me.Equipment[Equipment_Slot].ID;
            }
            else if (Shop_Slot >= 0 && Shop_Slot < Shop_Open.Sold.Length)
            {
                position = new Point(List["Shop"].Position.X - 186, List["Shop"].Position.Y + 5);
                Infomation_ID = Shop_Open.Sold[Shop_Slot].Item.ID;
            }
            else Infomation_ID = Guid.Empty;

            // Define os dados do painel de informações
            List["Information"].Visible = !position.IsEmpty && Infomation_ID != Guid.Empty;
            List["Information"].Position = position;
        }
    }
}