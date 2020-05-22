using System;
using System.Drawing;

class Utilities
{
    // Dados temporários
    public static byte CreateCharacter_Class = 0;
    public static byte CreateCharacter_Tex = 0;
    public static int SelectCharacter = 1;
    public static string Infomation_ID;
    public static byte Drop_Slot = 0;
    public static string Party_Invitation;
    public static string Trade_Invitation;
    public static byte Trade_Slot = 0;
    public static byte Trade_Inventory_Slot = 0;
    public static Lists.Structures.Shop Shop_Open;
    public static byte Shop_Inventory_Slot = 0;
    public static byte Hotbar_Change;
    public static byte Inventory_Change;

    public static bool IsAbove(Rectangle Rectangle)
    {
        // Verficia se o Window.Mouse está sobre o objeto
        if (Window.Mouse.X >= Rectangle.X && Window.Mouse.X <= Rectangle.X + Rectangle.Width)
            if (Window.Mouse.Y >= Rectangle.Y && Window.Mouse.Y <= Rectangle.Y + Rectangle.Height)
                return true;

        // Se não, retornar um valor nulo
        return false;
    }

    public static short MeasureString(string Text)
    {
        // Dados do texto
        SFML.Graphics.Text TempText = new SFML.Graphics.Text(Text, Graphics.Font_Default);
        TempText.CharacterSize = 10;
        return (short)TempText.GetLocalBounds().Width;
    }

    public static string TextBreak(string Text, int Width)
    {
        // Previne sobrecargas
        if (string.IsNullOrEmpty(Text)) return Text;

        // Usado para fazer alguns calculosk
        int Text_Width = MeasureString(Text);

        // Diminui o tamanho do texto até que ele caiba no digitalizador
        while (Text_Width - Width >= 0)
        {
            Text = Text.Substring(1);
            Text_Width = MeasureString(Text);
        }

        return Text;
    }

    public static byte Slot(Panels.Structure Panel, byte OffX, byte OffY, byte Lines, byte Columns, byte Grid = 32, byte Gap = 4)
    {
        int Size = Grid + Gap;
        Point Start = Panel.Position + new Size(OffX, OffY);
        Point Slot = new Point((Window.Mouse.X - Start.X) / Size, (Window.Mouse.Y - Start.Y) / Size);

        // Verifica se o Window.Mouse está sobre o slot
        if (Slot.Y < 0 || Slot.X < 0 || Slot.X >= Columns || Slot.Y >= Lines) return 0;
        if (!IsAbove(new Rectangle(Start.X + Slot.X * Size, Start.Y + Slot.Y * Size, Grid, Grid))) return 0;
        if (!Panel.Visible) return 0;

        // Retorna o slot
        return (byte)(Slot.Y * Columns + Slot.X + 1);
    }

    public static void CheckInformations()
    {
        Point Position = new Point();

        // Define as informações do painel com base no que o Window.Mouse está sobrepondo
        if (Panels.Hotbar_Slot > 0)
        {
            Position = Panels.Get("Hotbar").Position + new Size(0, 42);
            Infomation_ID = Lists.GetID(Player.Me.Inventory[Player.Me.Hotbar[Panels.Hotbar_Slot].Slot].Item);
        }
        else if (Panels.Inventory_Slot > 0)
        {
            Position = Panels.Get("Menu_Inventory").Position + new Size(-186, 3);
            Infomation_ID = Lists.GetID(Player.Me.Inventory[Panels.Inventory_Slot].Item);
        }
        else if (Panels.Equipment_Slot >= 0)
        {
            Position = Panels.Get("Menu_Character").Position + new Size(-186, 5);
            Infomation_ID = Lists.GetID(Player.Me.Equipment[Panels.Equipment_Slot]);
        }
        else if (Panels.Shop_Slot >= 0 && Panels.Shop_Slot < Shop_Open.Sold.Length)
        {
            Position = new Point(Panels.Get("Shop").Position.X - 186, Panels.Get("Shop").Position.Y + 5);
            Infomation_ID = Lists.GetID(Shop_Open.Sold[Panels.Shop_Slot].Item);
        }
        else Infomation_ID = Guid.Empty.ToString();

        // Define os dados do painel de informações
        Panels.Get("Information").Visible = !Position.IsEmpty && Infomation_ID != Guid.Empty.ToString();
        Panels.Get("Information").Position = Position;
    }
}