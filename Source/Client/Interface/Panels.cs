using System.Drawing;
using System.Collections.Generic;

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

    public static byte Inventory_Mouse()
    {
        byte NumColumn = 5;
        Point Panel_Position = Get("Menu_Inventory").Position;

        if (Get("Menu_Inventory").Visible)
            for (byte i = 1; i <= Game.Max_Inventory; i++)
            {
                // Posição do item
                byte Line = (byte)((i - 1) / NumColumn);
                int Column = i - (Line * 5) - 1;
                Point Position = new Point(Panel_Position.X + 7 + Column * 36, Panel_Position.Y + 30 + Line * 36);

                // Retorna o slot em que o mouse está por cima
                if (Tools.IsAbove(new Rectangle(Position.X, Position.Y, 32, 32))) return i;
            }

        return 0;
    }

    public static void Inventory_MouseDown(SFML.Window.MouseButtonEventArgs e)
    {
        byte Slot = Inventory_Mouse();

        // Somente se necessário
        if (Slot == 0) return;
        if (Player.Inventory[Slot].Item_Num == 0) return;

        // Solta item
        if (e.Button == SFML.Window.Mouse.Button.Right)
        {
            if (Lists.Item[Player.Inventory[Slot].Item_Num].Bind != Game.BindOn.Pickup)
                // Vende o item
                if (Get("Shop").Visible)
                {
                    if (Player.Inventory[Slot].Amount != 1)
                    {
                        Game.Shop_Inventory_Slot = Slot;
                        TextBoxes.Get("Shop_Sell_Amount").Text = string.Empty;
                        Get("Shop_Sell").Visible = true;
                    }
                    else Send.Shop_Sell(Slot, 1);
                }
                // Solta o item
                else if (!Get("Trade").Visible)
                    if (Player.Inventory[Slot].Amount != 1)
                    {
                        Game.Drop_Slot = Slot;
                        TextBoxes.Get("Drop_Amount").Text = string.Empty;
                        Get("Drop").Visible = true;
                    }
                    else Send.DropItem(Slot, 1);
        }
        // Seleciona o item
        else if (e.Button == SFML.Window.Mouse.Button.Left) Player.Inventory_Change = Slot;
    }

    public static void Equipment_MouseDown(SFML.Window.MouseButtonEventArgs e)
    {
        Point Panel_Position = Get("Menu_Character").Position;

        for (byte i = 0; i < (byte)Game.Equipments.Count; i++)
            if (Tools.IsAbove(new Rectangle(Panel_Position.X + 7 + i * 36, Panel_Position.Y + 247, 32, 32)))
                // Remove o equipamento
                if (e.Button == SFML.Window.Mouse.Button.Right)
                    if (Lists.Item[Player.Me.Equipment[i]].Bind != Game.BindOn.Equip)
                    {
                        Send.Equipment_Remove(i);
                        return;
                    }
    }

    public static byte Hotbar_Mouse()
    {
        Point Panel_Position = Get("Hotbar").Position;

        if (Get("Hotbar").Visible)
            for (byte i = 1; i <= Game.Max_Hotbar; i++)
            {
                // Posição do slot
                Point Position = new Point(Panel_Position.X + 8 + (i - 1) * 36, Panel_Position.Y + 6);

                // Retorna o slot em que o mouse está por cima
                if (Tools.IsAbove(new Rectangle(Position.X, Position.Y, 32, 32))) return i;
            }

        return 0;
    }

    public static void Hotbar_MouseDown(SFML.Window.MouseButtonEventArgs e)
    {
        byte Slot = Hotbar_Mouse();

        // Somente se necessário
        if (Slot == 0) return;
        if (Player.Hotbar[Slot].Slot == 0) return;

        // Solta item
        if (e.Button == SFML.Window.Mouse.Button.Right)
        {
            Send.Hotbar_Add(Slot, 0, 0);
            return;
        }
        // Seleciona o item
        if (e.Button == SFML.Window.Mouse.Button.Left)
        {
            Player.Hotbar_Change = Slot;
            return;
        }
    }

    public static byte Trade_Mouse()
    {
        byte NumColumn = 5;
        Point Panel_Position = Get("Trade").Position;

        if (Get("Trade").Visible)
            for (byte i = 1; i <= Game.Max_Inventory; i++)
            {
                // Posição do item
                byte Line = (byte)((i - 1) / NumColumn);
                int Column = i - (Line * 5) - 1;
                Point Position = new Point(Panel_Position.X + 7 + Column * 36, Panel_Position.Y + 50 + Line * 36);

                // Retorna o slot em que o mouse está por cima
                if (Tools.IsAbove(new Rectangle(Position.X, Position.Y, 32, 32))) return i;
            }

        return 0;
    }

    public static void Trade_MouseDown(SFML.Window.MouseButtonEventArgs e)
    {
        byte Slot = Trade_Mouse();

        // Somente se necessário
        if (!Get("Trade").Visible) return;
        if (Slot == 0) return;
        if (Player.Trade_Offer[Slot].Item_Num == 0) return;

        // Solta item
        if (e.Button == SFML.Window.Mouse.Button.Right) Send.Trade_Offer(Slot, 0);
    }

    public static byte Shop_Mouse()
    {
        Point Panel_Position = Get("Shop").Position;
        byte NumColumn = 7;

        if (Get("Shop").Visible)
            for (byte i = 1; i <= Lists.Shop[Game.Shop_Open].Sold.Length; i++)
            {
                // Posição do item
                byte Line = (byte)((i - 1) / NumColumn);
                int Column = i - (Line * 5) - 1;
                Point Position = new Point(Panel_Position.X + 7 + Column * 36, Panel_Position.Y + 50 + Line * 36);

                // Retorna o slot em que o mouse está por cima
                if (Tools.IsAbove(new Rectangle(Position.X, Position.Y, 32, 32))) return i;
            }

        return 0;
    }
}