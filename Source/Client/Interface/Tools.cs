using System.Collections.Generic;
using System.Drawing;

class Tools
{
    // Posição do ponteiro do mouse
    public static Point Mouse;

    // Janela que está aberta
    public static Windows CurrentWindow;

    // Chat
    public static bool Chat_Text_Visible;
    public const byte Chat_Lines_Visible = 9;
    public static byte Chat_Line;
    private const byte Max_Chat_Lines = 50;

    // Ordem da renderização das ferramentas
    public static List<Order_Structure>[] All_Order = new List<Order_Structure>[(byte)Windows.Count];
    public static List<Chat_Structure> Chat = new List<Chat_Structure>();

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

    public class Chat_Structure
    {
        public string Text;
        public SFML.Graphics.Color Color;
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

    private static void Chat_AddLine(string Mensagem, SFML.Graphics.Color Cor)
    {
        Chat.Add(new Chat_Structure());
        int i = Chat.Count - 1;

        // Adiciona a mensagem em uma linha vazia
        Chat[i].Text = Mensagem;
        Chat[i].Color = Cor;

        // Remove uma linha se necessário
        if (Chat.Count > Max_Chat_Lines) Chat.Remove(Chat[0]);
        if (i + Chat_Line > Chat_Lines_Visible + Chat_Line)
            Chat_Line = (byte)(i - Chat_Lines_Visible);

        // Torna as linhas visíveis
        Chat_Text_Visible = true;
    }

    public static void Chat_Add(string Message, SFML.Graphics.Color Color)
    {
        int Message_Width, Box_Width = Graphics.TSize(Graphics.Tex_Panel[Panels.Get("Chat").Texture_Num]).Width - 16;
        string Temp_Message;

        // Remove os espaços
        Message = Message.Trim();
        Message_Width = MeasureString(Message);

        // Caso couber, adiciona a mensagem normalmente
        if (Message_Width < Box_Width)
            Chat_AddLine(Message, Color);
        else
            for (int i = 0; i <= Message.Length; i++)
            {
                Temp_Message = Message.Substring(0, i);

                // Adiciona o texto à caixa
                if (MeasureString(Temp_Message) > Box_Width)
                {
                    Chat_AddLine(Temp_Message, Color);
                    Chat_Add(Message.Substring(Temp_Message.Length), Color);
                    return;
                }
            }
    }
}