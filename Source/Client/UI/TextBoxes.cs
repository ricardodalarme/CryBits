using Logic;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Drawing;
using static Logic.Utils;

namespace Interface
{
    class TextBoxes : Tools.Structure
    {
        // Armazenamento de dados da ferramenta
        public static Dictionary<string, TextBoxes> List = new Dictionary<string, TextBoxes>();

        // Digitalizador focado
        public static Tools.Order_Structure Focused;
        public static bool Signal;

        // Dados
        public string Text;
        public short Lenght;
        public short Width;
        public bool Password;

        // Eventos
        public void MouseUp(Tools.Order_Structure Order)
        {
            // Somente se necessário
            if (!IsAbove(new Rectangle(Position, new Size(Width, Graphics.TSize(Graphics.Tex_TextBox).Height)))) return;

            // Define o foco no digitalizador
            Focused = Order;

            // Altera o foco do digitalizador
            if (((TextBoxes)Order.Data).Name.Equals("Chat"))
            {
                Loop.Chat_Timer = Environment.TickCount + Chat.Sleep_Timer;
                Panels.List["Chat"].Visible = true;
            }
        }

        public void TextEntered(TextEventArgs e)
        {
            // Apaga a última letra do texto
            if (Tools.Viewable(Tools.Get(this)))
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    if (e.Unicode == "\b" && Text.Length > 0)
                    {
                        Text = Text.Remove(Text.Length - 1);
                        return;
                    }

                    // Não adicionar se já estiver no máximo de caracteres
                    if (Lenght > 0)
                        if (Text.Length >= Lenght)
                            return;
                }

                // Adiciona o caracter à caixa de texto
                char Char = Convert.ToChar(e.Unicode);
                if (Char > 31 && Char < 128) Text += e.Unicode;
            }
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
                        if (Top[i].Data is TextBoxes && !Tools.Order[i].Data.Name.Equals("Chat"))
                        {
                            Focused = Top[i];
                            return;
                        }
                        Stack.Push(Top[i].Nodes);
                    }
            }
            Focused = null;
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
                if (Parent[Temp].Viewable && Parent[Temp].Data is TextBoxes)
                {
                    Focused = Parent[Temp];
                    return;
                }
                Temp++;
            }
        }
    }
}