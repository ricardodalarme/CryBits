using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

public class Tools
{
    // Habilitação das ferramentas
    public static bool Able;

    // Posição do ponteiro do mouse
    public static Point Mouse;

    // Janela que está focada
    public static Windows CurrentWindow;

    // Chat
    public static bool Chat_Text_Visible ;
    public const byte Chat_Lines_Visible = 9;
    public static byte Chat_Line;
    public const byte Max_Chat_Lines = 50;

    // Ordem da renderização das ferramentas
    public static Identification[] Order = new Identification[0];
    public static List<Chat_Structure> Chat = new List<Chat_Structure>();

    public struct Identification
    {
        public byte Index;
        public Types Type;
    }

    public class Structure
    {
        public string Name;
        public bool Visible;
        public Point Position;
        public bool Able;

        public bool CheckEnable()
        {
            // Define a habilitação da ferramenta
            if (!Visible || !Tools.Able)
                return Able = false;
            else
                return Able = true;
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
        None,
        Menu,
        Game
    }

    // Tipos de ferramentas
    public enum Types
    {
        Button,
        Painel,
        CheckBox,
        TextBox
    }

    public static void SetEnable(string Panel, Windows Window)
    {
        // Define a habilitação
        if (CurrentWindow != Window || Panel != string.Empty && !Panels.Find(Panel).General.Visible)
            Able = false;
        else
            Able = true;
    }

    public static void List(Types Type, byte Index)
    {
        short Amount =(short)( Order.GetUpperBound(0) + 1);

        // Se já estiver listado não é necessário listar de novo
        if (IsListed(Type, Index))
            return;

        // Altera o tamanho da caixa
        Array.Resize(ref Order, Amount + 1);

        // Adiciona à lista
        Order[Amount].Type = Type;
        Order[Amount].Index = Index;
    }

    private static bool IsListed(Types Type, byte Index)
    {
        // Verifica se a ferramenta já está listada
        for (short i = 1; i < Order.Length; i++)
            if (Order[i].Type == Type && Order[i].Index == Index)
                return true;

        return false;
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

    public static int Encontrar(Types Type, byte Index)
    {
        // Lista os nomes dos botões
        for (byte i = 1; i < Order.Length; i++)
            if (Order[i].Type == Type && Order[i].Index == Index)
                return i;

        return 0;
    }

    public static int MeasureString(string Text)
    {
        // Dados do texto
        SFML.Graphics.Text TempText = new SFML.Graphics.Text(Text, Graphics.Font_Default);
        TempText.CharacterSize = 10;
        return (int)TempText.GetLocalBounds().Width;
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

    public static byte Chat_EmptyLine()
    {
        // Encontra uma linha vazia
        for (byte i = 0; i <= Max_Chat_Lines; i++)
            if (Chat[i].Text == string.Empty)
                return i;

        return 0;
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
        Chat_Text_Visible  = true;
    }

    public static void Chat_Add(string Message, SFML.Graphics.Color Color)
    {
        int Message_Width, Box_Width = Graphics.TSize(Graphics.Tex_Panel[Panels.Find("Chat").Texture]).Width - 16;
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
        Point Panel_Position = Panels.Find("Menu_Inventário").General.Position;

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
        Point Panel_Position = Panels.Find("Menu_Personagem").General.Position;

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
        Point Panel_Position = Panels.Find("Hotbar").General.Position;

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