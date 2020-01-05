using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class Tools
{
    // Posição do ponteiro do mouse
    public static Point Mouse;

    // Janela que está focada
    public static Windows CurrentWindow;

    // Chat
    public static bool Chat_Text_Visible;
    public const byte Chat_Lines_Visible = 9;
    public static byte Chat_Line;
    public const byte Max_Chat_Lines = 50;

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
            get {  return Viewable(this); }
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
        if (!Order.Data.Visible) return false;
        return Viewable(Order.Parent);
    }

    public static Order_Structure Get(Structure Tool, List<Order_Structure> Node = null)
    {
        if (Node == null) return Get(Tool, Order);

        // Encontra a ferramenta na árvore de ordem
        for (byte i = 0; i < Node.Count; i++)
        {
            if (Node[i].Data == Tool) return Node[i];
            Get(Tool, Node[i].Nodes);
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
        if (string.IsNullOrEmpty(Text))
            return Text;

        // Usado para fazer alguns calculos
        int Text_Width = MeasureString(Text);

        // Diminui o tamanho do texto até que ele possa caber no digitalizador
        while (Text_Width - Width >= 0)
        {
            Text = Text.Substring(1);
            Text_Width = MeasureString(Text);
        }

        return Text;
    }

    public static void Chat_AddLine(string Mensagem, SFML.Graphics.Color Cor)
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
        string Temp_Message; int Split;

        // Remove os espaços
        Message = Message.Trim();
        Message_Width = MeasureString(Message);

        // Caso couber, adiciona a mensagem normalmente
        if (Message_Width < Box_Width)
            Chat_AddLine(Message, Color);
        else
        {
            for (int i = 0; i <= Message.Length; i++)
            {
                // Verifica se o próximo caráctere é um separável 
                switch (Message[i])
                {
                    case '-':
                    case '_':
                    case ' ': Split = i; break;
                }

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

    public static byte Inventory_Mouse()
    {
        byte NumColumn = 5;
        Point Panel_Position = Panels.Get("Menu_Inventário").Position;

        for (byte i = 1; i <= Game.Max_Inventory; i++)
        {
            // Posição do item
            byte Line = (byte)((i - 1) / NumColumn);
            int Column = i - (Line * 5) - 1;
            Point Position = new Point(Panel_Position.X + 7 + Column * 36, Panel_Position.Y + 30 + Line * 36);

            // Retorna o slot em que o mouse está por cima
            if (IsAbove(new Rectangle(Position.X, Position.Y, 32, 32)))
                return i;
        }

        return 0;
    }

    public static void Inventory_MouseDown(MouseEventArgs e)
    {
        byte Slot = Inventory_Mouse();

        // Somente se necessário
        if (Slot == 0) return;
        if (Player.Inventory[Slot].Item_Num == 0) return;

        // Solta item
        if (e.Button == MouseButtons.Right)
        {
            Send.DropItem(Slot);
            return;
        }
        // Seleciona o item
        else if (e.Button == MouseButtons.Left)
        {
            Player.Inventory_Change = Slot;
            return;
        }
    }

    public static void Equipment_MouseDown(MouseEventArgs e)
    {
        Point Panel_Position = Panels.Get("Menu_Personagem").Position;

        for (byte i = 0; i <= (byte)Game.Equipments.Amount - 1; i++)
            if (IsAbove(new Rectangle(Panel_Position.X + 7 + i * 36, Panel_Position.Y + 247, 32, 32)))
                // Remove o equipamento
                if (e.Button == MouseButtons.Right)
                {
                    Send.Equipment_Remove(i);
                    return;
                }
    }

    public static byte Hotbar_Mouse()
    {
        Point Panel_Position = Panels.Get("Hotbar").Position;

        for (byte i = 1; i <= Game.Max_Hotbar; i++)
        {
            // Posição do slot
            Point Position = new Point(Panel_Position.X + 8 + (i - 1) * 36, Panel_Position.Y + 6);

            // Retorna o slot em que o mouse está por cima
            if (IsAbove(new Rectangle(Position.X, Position.Y, 32, 32)))
                return i;
        }

        return 0;
    }

    public static void Hotbar_MouseDown(MouseEventArgs e)
    {
        byte Slot = Hotbar_Mouse();

        // Somente se necessário
        if (Slot == 0) return;
        if (Player.Hotbar[Slot].Slot == 0) return;

        // Solta item
        if (e.Button == MouseButtons.Right)
        {
            Send.Hotbar_Add(Slot, 0, 0);
            return;
        }
        // Seleciona o item
        else if (e.Button == MouseButtons.Left)
        {
            Player.Hotbar_Change = Slot;
            return;
        }
    }
}