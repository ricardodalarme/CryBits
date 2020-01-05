using System.Drawing;
using System.Windows.Forms;

class TextBoxes
{
    // Armazenamento de dados da ferramenta
    public static Structure[] List;

    // Digitalizador focado
    public static Structure Focused;
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
        public void MouseUp()
        {
            // Somente se necessário
            if (!Viewable) return;
            if (!Tools.IsAbove(new Rectangle(Position, new Size(Width, Graphics.TSize(Graphics.Tex_TextBox).Height)))) return;

            // Define o foco no Digitalizador
            Focused = this;
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
        for (byte i = 1; i < List.Length; i++)
            if (List[i].Name.Equals(Name))
                return List[i];

        return null;
    }

    public static void Focus()
    {
        // todo percorrer da forma certa
        // Se o digitalizador não estiver habilitado então isso não é necessário 
        if (Focused != null && Focused.Viewable) return;

        // Altera o digitalizador focado para o mais próximo
        for (byte i = 0; i < Tools.Order.Count; i++)
        {
            if (!(Tools.Order[i].Data is Structure))
                continue;
            else if (!Tools.Order[i].Data.Viewable)
                continue;
            else if ((Structure)Tools.Order[i].Data != Get("Chat"))
                Focused = (Structure)Tools.Order[i].Data;
            return;
        }
    }

    public static void ChangeFocus()
    {
        // todo percorrer da forma certa
        // Altera o digitalizador focado para o próximo
        for (byte i = 0; i < Tools.Order.Count; i++)
        {
            if (!(Tools.Order[i].Data is Structure))
                continue;
            else if (!Tools.Order[i].Data.Viewable)
                continue;
            if (Focused != Last() && i <= Tools.Get(Focused))
                continue;

            Focused = (Structure)Tools.Order[i].Data;
            return;
        }
    }

    public static Structure Last()
    {
        Structure Tool = null;
        // todo percorrer da forma certa
        // Retorna o último digitalizador habilitado
        for (byte i = 0; i < Tools.Order.Count; i++)
            if (Tools.Order[i].Data is Structure)
                if (Tools.Order[i].Data.Viewable)
                    Tool = (Structure)Tools.Order[i].Data;

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
            Focused = Tool;
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