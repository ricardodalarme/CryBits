using System.Drawing;
using System.Windows.Forms;

public class TextBoxes
{
    // Armazenamento de dados da ferramenta
    public static Structure[] List;

    // Digitalizador focado
    public static Structure TexBox_Focus;
    public static bool Signal;

    // Estrutura da ferramenta
    public class Structure : Tools.Structure
    {
        public string Text;
        public short Lenght;
        public short Width;
        public bool Password;
    }

    public static Structure Get(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].Name.Equals(Name))
                return List[i];

        return null;
    }

    public static void Focus()
    {
        // Se o digitalizador não estiver habilitado então isso não é necessário 
        if (TexBox_Focus != null && TexBox_Focus.IsAble) return;
        
        // Altera o digitalizador focado para o mais próximo
        for (byte i = 1; i < Tools.Order.Length; i++)
        {
            if (!(Tools.Order[i] is Structure))
                continue;
            else if (!Tools.Order[i].IsAble)
                continue;
            else if ((Structure)Tools.Order[i] == Get("Chat"))
                TexBox_Focus = (Structure)Tools.Order[i];
            return;
        }
    }

    public static void ChangeFocus()
    {
        // Altera o digitalizador focado para o próximo
        for (byte i = 1; i < Tools.Order.Length; i++)
        {
            if (!(Tools.Order[i] is Structure))
                continue;
            else if (!Tools.Order[i].IsAble)
                continue;
            if (TexBox_Focus != Last() && i <= Tools.Get(TexBox_Focus))
                continue;

            TexBox_Focus = (Structure)Tools.Order[i];
            return;
        }
    }

    public static Structure Last()
    {
        Structure Tool = null;

        // Retorna o último digitalizador habilitado
        for (byte i = 1; i < Tools.Order.Length; i++)
            if (Tools.Order[i] is Structure)
                if (Tools.Order[i].IsAble)
                    Tool = (Structure)Tools.Order[i];

        return Tool;
    }

    public static void Chat_Type()
    {
        Structure Tool = Get("Chat");

        // Somente se necessário
        if (!Player.IsPlaying(Player.MyIndex)) return;

        // Altera a visiblidade da caixa
        Panels.Get("Chat").Visible = !Panels.Get("Chat").Visible;

        // Altera o foco do digitalizador
        if (Panels.Get("Chat").Visible)
        {
            Tools.Chat_Text_Visible = true;
            TexBox_Focus = Tool;
            return;
        }
        else
            TexBox_Focus = null;

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

    public class Events
    {
        public static void MouseUp(MouseEventArgs e, Structure Tool)
        {
            // Somente se necessário
            if (!Tool.IsAble) return;
            if (!Tools.IsAbove(new Rectangle(Tool.Position, new Size(Tool.Width, Graphics.TSize(Graphics.Tex_TextBox).Height)))) return;

            // Define o foco no Digitalizador
            TexBox_Focus = Tool;
        }

        public static void KeyPress(KeyPressEventArgs e)
        {
            // Se não tiver nenhum focado então sair
            if (TexBox_Focus == null) return;

            // Altera o foco do digitalizador para o próximo
            if (e.KeyChar == (char)Keys.Tab)
            {
                ChangeFocus();
                return;
            }

            // Texto
            string Text = TexBox_Focus.Text;

            // Apaga a última letra do texto
            if (!string.IsNullOrEmpty(Text))
            {
                if (e.KeyChar == '\b' && Text.Length > 0)
                {
                    TexBox_Focus.Text = Text.Remove(Text.Length - 1);
                    return;
                }

                // Não adicionar se já estiver no máximo de caracteres
                if (TexBox_Focus.Lenght > 0)
                    if (Text.Length >= TexBox_Focus.Lenght)
                        return;
            }

            // Adiciona apenas os caractres válidos ao digitalizador
            if (e.KeyChar >= 32 && e.KeyChar <= 126) TexBox_Focus.Text += e.KeyChar.ToString();
        }
    }
}