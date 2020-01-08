using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class TextBoxes
{
    // Armazenamento de dados da ferramenta
    public static List<Structure> List = new List<Structure>();

    // Digitalizador focado
    public static Tools.Order_Structure Focused;
    public static bool Signal;

    // Estrutura da ferramenta
    public class Structure : Tools.Structure
    {
        // Dados
        public string Text;
        public short Lenght;
        public short Width;
        public bool Password;

        // Eventos
        public void MouseUp(Tools.Order_Structure Order)
        {
            // Somente se necessário
            if (!Tools.IsAbove(new Rectangle(Position, new Size(Width, Graphics.TSize(Graphics.Tex_TextBox).Height)))) return;

            // Define o foco no digitalizador
            Focused = Order;

            // Altera o foco do digitalizador
            if (((Structure)Order.Data).Name.Equals("Chat"))
            {
                Tools.Chat_Text_Visible = true;
                Panels.Get("Chat").Visible = true;
            }
        }

        public void KeyPress(KeyPressEventArgs e)
        {
            // Apaga a última letra do texto
            if (!string.IsNullOrEmpty(Text))
            {
                if (e.KeyChar == '\b' && Text.Length > 0)
                {
                    Text = Text.Remove(Text.Length - 1);
                    return;
                }

                // Não adicionar se já estiver no máximo de caracteres
                if (Lenght > 0)
                    if (Text.Length >= Lenght)
                        return;
            }

            // Adiciona apenas os caractres válidos ao digitalizador
            if (e.KeyChar >= 32 && e.KeyChar <= 126) Text += e.KeyChar.ToString();
        }
    }

    public static Structure Get(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 0; i < List.Count; i++)
            if (List[i].Name.Equals(Name))
                return List[i];

        return null;
    }

    public static void Focus()
    {
        // Se o digitalizador não estiver habilitado então isso não é necessário 
        if (Focused != null && Focused.Viewable) return;

        // Percorre toda a árvore de ordem para executar o comando
        Stack<List<Tools.Order_Structure>> Stack = new Stack<List<Tools.Order_Structure>>();
        Stack.Push(Tools.Order);
        while (Stack.Count != 0)
        {
            List<Tools.Order_Structure> Top = Stack.Pop();

            for (byte i = 0; i < Top.Count; i++)
                if (Top[i].Data.Visible)
                {
                    // Altera o digitalizador focado para o primeiro visível
                    if (Top[i].Data is Structure && !Tools.Order[i].Data.Name.Equals("Chat"))
                    {
                        Focused = Top[i];
                        return;
                    }
                    Stack.Push(Top[i].Nodes);
                }
        }
    }

    public static void ChangeFocus()
    {
        List<Tools.Order_Structure> Parent;
        if (Focused.Parent != null) Parent = Focused.Parent.Nodes;
        else Parent = Tools.Order;
        int Index = Parent.IndexOf(Focused), Temp = Index + 1;

        // Altera o digitalizador focado para o próximo
        while (Temp != Index)
        {
            if (Temp == Parent.Count) Temp = 0;
            if (Parent[Temp].Viewable && Parent[Temp].Data is Structure)
            {
                Focused = Parent[Temp];
                return;
            }
            Temp++;
        }
    }

    public static void Chat_Type()
    {
        Structure Tool = Get("Chat");
        Panels.Structure Panel = Panels.Get("Chat");

        // Somente se necessário
        if (!Player.IsPlaying(Player.MyIndex)) return;

        // Altera a visiblidade da caixa
        Panel.Visible = !Panel.Visible;

        // Altera o foco do digitalizador
        if (Panel.Visible)
        {
            Tools.Chat_Text_Visible = true;
            Focused = Tools.Get(Tool);
            return;
        }
        else
            Focused = null;

        // Dados
        string Message = Tool.Text;

        // Somente se necessário
        if (Message.Length < 3)
        {
            Tool.Text = string.Empty;
            return;
        }

        // Separa as mensagens em partes
        string[] Parts = Message.Split(' ');

        // Global
        if (Message.Substring(0, 1) == "'")
            Send.Message(Message.Substring(1), Game.Messages.Global);
        // Particular
        else if (Message.Substring(0, 1) == "!")
        {
            // Previne erros 
            if (Parts.GetUpperBound(0) < 1)
                Tools.Chat_Add("Use: '!' + Addressee + 'Message'", SFML.Graphics.Color.White);
            else
            {
                // Dados
                string Destinatário = Message.Substring(1, Parts[0].Length - 1);
                Message = Message.Substring(Parts[0].Length + 1);

                // Envia a mensagem
                Send.Message(Message, Game.Messages.Private, Destinatário);
            }
        }
        // Mapa
        else
            Send.Message(Message, Game.Messages.Map);

        // Limpa a caixa de texto
        Tool.Text = string.Empty;
    }
}