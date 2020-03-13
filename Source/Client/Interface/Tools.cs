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
        public bool Viewable
        {
            get { return Viewable(this); }
        }
    }

    public class Structure
    {
        public string Name;
        public bool Visible;
        public Point Position;
        public Windows Window;
    }

    public static List<Order_Structure> Order
    {
        get
        {
            return All_Order[(byte)CurrentWindow];
        }
    }

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
        Count
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
}