using Network;
using System.Collections.Generic;
using System.Drawing;
using Objects;
using System;
using static Utils;

namespace Interface
{
    class Panels
    {
        // Armazenamento dos dados da ferramenta
        public static List<Structure> List = new List<Structure>();

        // Estrutura da ferramenta
        public class Structure : Tools.Structure
        {
            public byte Texture_Num;
        }

        // Retorna o painel procurado
        public static Structure Get(string Name) => List.Find(x => x.Name.Equals(Name));

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
            Get("Connect").Visible = false;
            Get("Register").Visible = false;
            Get("Options").Visible = false;
            Get("SelectCharacter").Visible = false;
            Get("CreateCharacter").Visible = false;
        }

        // Retorna em qual slot o mouse está sobrepondo
        public static byte Inventory_Slot =>  Slot(Get("Menu_Inventory"), 7, 29, 6, 5);
        public static short Hotbar_Slot => (short)( Slot(Get("Hotbar"), 8, 6, 1, 10) - 1);
        public static byte Trade_Slot =>  Slot(Get("Trade"), 7, 50, 6, 5);
        public static short Shop_Slot => (short)( Slot(Get("Shop"), 7, 50, 4, 7) - 1);
        public static short Equipment_Slot => (short)( Slot(Get("Menu_Character"), 7, 248, 1, 5) - 1);

        public static void Inventory_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            byte Slot = Inventory_Slot;

            // Somente se necessário
            if (Slot == 0) return;
            if (Player.Me.Inventory[Slot].Item == null) return;

            // Solta item
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                if (Player.Me.Inventory[Slot].Item.Bind !=  BindOn.Pickup)
                    // Vende o item
                    if (Get("Shop").Visible)
                    {
                        if (Player.Me.Inventory[Slot].Amount != 1)
                        {
                            Shop_Inventory_Slot = Slot;
                            TextBoxes.Get("Shop_Sell_Amount").Text = string.Empty;
                            Get("Shop_Sell").Visible = true;
                        }
                        else Send.Shop_Sell(Slot, 1);
                    }
                    // Solta o item
                    else if (!Get("Trade").Visible)
                        if (Player.Me.Inventory[Slot].Amount != 1)
                        {
                            Drop_Slot = Slot;
                            TextBoxes.Get("Drop_Amount").Text = string.Empty;
                            Get("Drop").Visible = true;
                        }
                        else Send.DropItem(Slot, 1);
            }
            // Seleciona o item
            else if (e.Button == SFML.Window.Mouse.Button.Left) Inventory_Change = Slot;
        }

        public static void Equipment_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            Point Panel_Position = Get("Menu_Character").Position;

            for (byte i = 0; i < (byte) Equipments.Count; i++)
                if ( IsAbove(new Rectangle(Panel_Position.X + 7 + i * 36, Panel_Position.Y + 247, 32, 32)))
                    // Remove o equipamento
                    if (e.Button == SFML.Window.Mouse.Button.Right)
                        if (Player.Me.Equipment[i].Bind !=  BindOn.Equip)
                        {
                            Send.Equipment_Remove(i);
                            return;
                        }
        }

        public static void Hotbar_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            short Slot = Hotbar_Slot;

            // Somente se necessário
            if (Slot < 0) return;
            if (Player.Me.Hotbar[Slot].Slot == 0) return;

            // Solta item
            if (e.Button == SFML.Window.Mouse.Button.Right)
            {
                Send.Hotbar_Add(Slot, 0, 0);
                return;
            }
            // Seleciona o item
            if (e.Button == SFML.Window.Mouse.Button.Left)
            {
                Panels.Hotbar_Change = Slot;
                return;
            }
        }

        public static void Trade_MouseDown(SFML.Window.MouseButtonEventArgs e)
        {
            byte Slot = Trade_Slot;

            // Somente se necessário
            if (!Get("Trade").Visible) return;
            if (Slot == 0) return;
            if (Player.Me.Trade_Offer[Slot].Item == null) return;

            // Solta item
            if (e.Button == SFML.Window.Mouse.Button.Right) Send.Trade_Offer(Slot, 0);
        }

        public static void CheckInformations()
        {
            Point Position = new Point();

            // Define as informações do painel com base no que o Window.Mouse está sobrepondo
            if ( Hotbar_Slot >= 0)
            {
                Position =  Get("Hotbar").Position + new Size(0, 42);
                Infomation_ID = Player.Me.Inventory[Player.Me.Hotbar[ Hotbar_Slot].Slot].Item.ID;
            }
            else if ( Inventory_Slot > 0)
            {
                Position =  Get("Menu_Inventory").Position + new Size(-186, 3);
                Infomation_ID = Player.Me.Inventory[ Inventory_Slot].Item.ID;
            }
            else if ( Equipment_Slot >= 0)
            {
                Position =  Get("Menu_Character").Position + new Size(-186, 5);
                Infomation_ID = Player.Me.Equipment[ Equipment_Slot].ID;
            }
            else if ( Shop_Slot >= 0 &&  Shop_Slot < Shop_Open.Sold.Length)
            {
                Position = new Point( Get("Shop").Position.X - 186,  Get("Shop").Position.Y + 5);
                Infomation_ID = Shop_Open.Sold[ Shop_Slot].Item.ID;
            }
            else Infomation_ID = Guid.Empty;

            // Define os dados do painel de informações
             Get("Information").Visible = !Position.IsEmpty && Infomation_ID != Guid.Empty;
             Get("Information").Position = Position;
        }
    }
}