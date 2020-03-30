using System.Collections.Generic;
using System.Drawing;

class Tools
{
    // Posição do ponteiro do mouse
    public static Point Mouse;

    // Janela que está aberta
    public static Windows CurrentWindow;

    // Ordem da renderização das ferramentas
    public static List<Order_Structure>[] All_Order = new List<Order_Structure>[(byte)Windows.Count];

    public class Order_Structure
    {
        public Structure Data;
        public Order_Structure Parent;
        public List<Order_Structure> Nodes;
        public bool Viewable => Viewable(this);
    }

    public class Structure
    {
        public string Name;
        public bool Visible;
        public Point Position;
        public Windows Window;
    }

    // Ordem que as ferramentas são renderizadas
    public static List<Order_Structure> Order => All_Order[(byte)CurrentWindow];

    // Identificação das janelas do jogo
    public enum Windows
    {
        Menu,
        Game,
        Global,
        Count
    }

    // Tipos de ferramentas
    public enum Types
    {
        Button,
        Panel,
        CheckBox,
        TextBox,
    }

    // Tipos de informações do painel
    public enum Informations
    {
        Hotbar,
        Inventory,
        Shop
    }

    public static bool IsAbove(Rectangle Rectangle)
    {
        // Verficia se o mouse está sobre o objeto
        if (Mouse.X >= Rectangle.X && Mouse.X <= Rectangle.X + Rectangle.Width)
            if (Mouse.Y >= Rectangle.Y && Mouse.Y <= Rectangle.Y + Rectangle.Height)
                return true;

        // Se não, retornar um valor nulo
        return false;
    }

    public static bool Viewable(Order_Structure Order)
    {
        // Verifica se a ferramenta está visível
        if (Order == null) return true;
        if (Order.Data.Window != CurrentWindow) return false;
        if (!Order.Data.Visible) return false;
        return Viewable(Order.Parent);
    }

    public static Order_Structure Get(Structure Tool)
    {
        // Percorre toda a árvore de ordem para encontrar a ferramenta
        Stack<List<Order_Structure>> Stack = new Stack<List<Order_Structure>>();
        for (byte i = 0; i < All_Order.Length; i++) Stack.Push(All_Order[i]);
        while (Stack.Count != 0)
        {
            List<Order_Structure> Top = Stack.Pop();

            for (byte i = 0; i < Top.Count; i++)
            {
                if (Top[i].Data == Tool) return Top[i];
                Stack.Push(Top[i].Nodes);
            }
        }
        return null;
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
        Point Slot = new Point((Mouse.X - Start.X) / Size, (Mouse.Y - Start.Y) / Size);

        // Verifica se o mouse está sobre o slot
        if (Slot.Y < 0 || Slot.X < 0 || Slot.X >= Columns || Slot.Y >= Lines) return 0;
        if (!IsAbove(new Rectangle(Start.X + Slot.X * Size, Start.Y + Slot.Y * Size, Grid, Grid))) return 0;
        if (!Panel.Visible) return 0;

        // Retorna o slot
        return (byte)(Slot.Y * Columns + Slot.X + 1);
    }

    public static Point Slot_Position(Point Start, byte Slot, byte Columns, byte Grid = 32, byte Gap = 4)
    {
        int Size = Grid + Gap;
        byte Line = (byte)(Slot / Columns);
        byte Column = (byte)(Slot - (Line * Columns));
        return new Point(Start.X + Column * Size, Start.Y + Line * Size);
    }

    public static void CheckInformations()
    {
        Point Position = new Point();
        short Data_Index = 0;

        // Define as informações do painel com base no que o mouse está sobrepondo
        if (Panels.Hotbar_Slot > 0)
        {
            Position = Panels.Get("Hotbar").Position + new Size(0, 42);
            Data_Index = Player.Inventory[Player.Hotbar[Panels.Hotbar_Slot].Slot].Item_Num;
        }
        else if (Panels.Inventory_Slot > 0)
        {
            Position = Panels.Get("Menu_Inventory").Position + new Size(-186, 3);
            Data_Index = Player.Inventory[Panels.Inventory_Slot].Item_Num;
        }
        else if (Panels.Equipment_Slot >= 0)
        {
            Position = Panels.Get("Menu_Character").Position + new Size(-186, 5);
            Data_Index = Player.Me.Equipment[Panels.Equipment_Slot];
        }
        else if (Panels.Shop_Slot >= 0)
        {
            Position = new Point(Panels.Get("Shop").Position.X - 186, Panels.Get("Shop").Position.Y + 5);
            Data_Index = Lists.Shop[Game.Shop_Open].Sold[Panels.Shop_Slot].Item_Num;
        }

        // Define os dados do painel de informações
        Panels.Get("Information").Visible = !Position.IsEmpty && Data_Index > 0;
        Panels.Get("Information").Position = Position;
        Game.Infomation_Index = Data_Index;
    }
}