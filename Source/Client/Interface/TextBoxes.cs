using System;
using System.Drawing;
using System.Windows.Forms;

public class TextBoxes
{
    // Armazenamento de dados da ferramenta
    public static Structure[] List = new Structure[1];

    // Digitalizador focado
    public static byte TexBox_Focus;
    public static bool Signal;

    // Estrutura da ferramenta
    public class Structure
    {
        public string Text;
        public short Lenght;
        public short Width;
        public bool Password;
        public Tools.Structure General;
    }

    public static byte FindIndex(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].General.Name == Name)
                return i;

        return 0;
    }

    public static Structure Find(string Name)
    {
        // Lista os nomes das ferramentas
        for (byte i = 1; i < List.Length; i++)
            if (List[i].General.Name == Name)
                return List[i];

        return null;
    }

    public static void Focus()
    {
        // Se o digitalizador não estiver habilitado então isso não é necessário 
        if (List[TexBox_Focus] != null && List[TexBox_Focus].General.Able) return;

        // Altera o digitalizador focado para o mais próximo
        for (byte i = 1; i < Tools.Order.Length; i++)
        {
            if (Tools.Order[i].Type != Tools.Types.TextBox)
                continue;
            else if (!List[Tools.Order[i].Index].General.Able)
                continue;
            else if (i == FindIndex("Chat"))

                TexBox_Focus = Tools.Order[i].Index;
            return;
        }
    }

    public static void ChangeFocus()
    {
        // Altera o digitalizador focado para o próximo
        for (byte i = 1; i < Tools.Order.Length; i++)
        {
            if (Tools.Order[i].Type != Tools.Types.TextBox)
                continue;
            else if (!List[Tools.Order[i].Index].General.Able)
                continue;
            if (TexBox_Focus != Last() && i <= Tools.Encontrar(Tools.Types.TextBox, TexBox_Focus))
                continue;

            TexBox_Focus = Tools.Order[i].Index;
            return;
        }
    }

    public static byte Last()
    {
        byte Index = 0;

        // Retorna o último digitalizador habilitado
        for (byte i = 1; i < Tools.Order.Length; i++)
            if (Tools.Order[i].Type == Tools.Types.TextBox)
                if (List[Tools.Order[i].Index].General.Able)
                    Index = Tools.Order[i].Index;

        return Index;
    }

    public static void Chat_Type()
    {
        byte Index = FindIndex("Chat");

        // Somente se necessário
        if (!Player.IsPlaying(Player.MyIndex)) return;

        // Altera a visiblidade da caixa
        Panels.Find("Chat").General.Visible = !Panels.Find("Chat").General.Visible;

        // Altera o foco do digitalizador
        if (Panels.Find("Chat").General.Visible)
        {
            Tools.Chat_Text_Visible  = true;
            TexBox_Focus = Index;
            return;
        }
        else
            TexBox_Focus = 0;

        // Dados
        string Message = List[Index].Text;
        string Player_Name = Player.Me.Name;

        // Somente se necessário
        if (Message.Length < 3)
        {
            List[Index].Text = string.Empty;
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
        List[Index].Text = string.Empty;
    }

    public class Events
    {
        public static void MouseUp(MouseEventArgs e, byte Index)
        {
            // Somente se necessário
            if (!List[Index].General.Able) return;
            if (!Tools.IsAbove(new Rectangle(List[Index].General.Position, new Size(List[Index].Width, Graphics.TSize(Graphics.Tex_TextBox).Height)))) return;

            // Define o foco no Digitalizador
            TexBox_Focus = Index;
        }

        public static void KeyPress(KeyPressEventArgs e)
        {
            // Se não tiver nenhum focado então sair
            if (TexBox_Focus == 0) return;

            // Altera o foco do digitalizador para o próximo
            if (e.KeyChar == (char)Keys.Tab)
            {
                ChangeFocus();
                return;
            }

            // Texto
            string Text = List[TexBox_Focus].Text;

            // Apaga a última letra do texto
            if (!string.IsNullOrEmpty(Text))
            {
                if (e.KeyChar == '\b' && Text.Length > 0)
                {
                    List[TexBox_Focus].Text = Text.Remove(Text.Length - 1);
                    return;
                }

                // Não adicionar se já estiver no máximo de caracteres
                if (List[TexBox_Focus].Lenght > 0)
                    if (Text.Length >= List[TexBox_Focus].Lenght)
                        return;
            }

            // Adiciona apenas os caractres válidos ao digitalizador
            if (e.KeyChar >= 32 && e.KeyChar <= 126) List[TexBox_Focus].Text += e.KeyChar.ToString();
        }
    }
}