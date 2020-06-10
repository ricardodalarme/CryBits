using System.Collections.Generic;
using System.Drawing;
using Network;

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
    public static byte Inventory_Slot => Utils.Slot(Get("Menu_Inventory"), 7, 29, 6, 5);
    public static short Hotbar_Slot => (short)(Utils.Slot(Get("Hotbar"), 8, 6, 1, 10) -1);
    public static byte Trade_Slot => Utils.Slot(Get("Trade"), 7, 50, 6, 5);
    public static short Shop_Slot => (short)(Utils.Slot(Get("Shop"), 7, 50, 4, 7) - 1);
    public static short Equipment_Slot => (short)(Utils.Slot(Get("Menu_Character"), 7, 248, 1, 5) - 1);

    public static void Inventory_MouseDown(SFML.Window.MouseButtonEventArgs e)
    {
        byte Slot = Inventory_Slot;

        // Somente se necessário
        if (Slot == 0) return;
        if (Player.Me.Inventory[Slot].Item == null) return;

        // Solta item
        if (e.Button == SFML.Window.Mouse.Button.Right)
        {
            if (Player.Me.Inventory[Slot].Item.Bind != Game.BindOn.Pickup)
                // Vende o item
                if (Get("Shop").Visible)
                {
                    if (Player.Me.Inventory[Slot].Amount != 1)
                    {
                        Utils.Shop_Inventory_Slot = Slot;
                        TextBoxes.Get("Shop_Sell_Amount").Text = string.Empty;
                        Get("Shop_Sell").Visible = true;
                    }
                    else Send.Shop_Sell(Slot, 1);
                }
                // Solta o item
                else if (!Get("Trade").Visible)
                    if (Player.Me.Inventory[Slot].Amount != 1)
                    {
                        Utils.Drop_Slot = Slot;
                        TextBoxes.Get("Drop_Amount").Text = string.Empty;
                        Get("Drop").Visible = true;
                    }
                    else Send.DropItem(Slot, 1);
        }
        // Seleciona o item
        else if (e.Button == SFML.Window.Mouse.Button.Left) Utils.Inventory_Change = Slot;
    }

    public static void Equipment_MouseDown(SFML.Window.MouseButtonEventArgs e)
    {
        Point Panel_Position = Get("Menu_Character").Position;

        for (byte i = 0; i < (byte)Game.Equipments.Count; i++)
            if (Utils.IsAbove(new Rectangle(Panel_Position.X + 7 + i * 36, Panel_Position.Y + 247, 32, 32)))
                // Remove o equipamento
                if (e.Button == SFML.Window.Mouse.Button.Right)
                    if (Player.Me.Equipment[i].Bind != Game.BindOn.Equip)
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
            Utils.Hotbar_Change = Slot;
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
}