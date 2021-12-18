using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Client.Logic;
using CryBits.Client.Media.Graphics;
using SFML.Window;
using static CryBits.Client.Logic.Utils;

namespace CryBits.Client.UI;

internal class TextBoxes : Tools.Structure
{
    // Armazenamento de dados da ferramenta
    public static Dictionary<string, TextBoxes> List = new();

    // Digitalizador focado
    public static Tools.OrderStructure Focused;
    public static bool Signal;

    // Dados
    public string Text;
    public short Length;
    public short Width;
    public bool Password;

    // Eventos
    public void MouseUp(Tools.OrderStructure order)
    {
        // Somente se necessário
        if (!IsAbove(new Rectangle(Position, new Size(Width, Textures.TextBox.ToSize().Height)))) return;

        // Define o foco no digitalizador
        Focused = order;

        // Altera o foco do digitalizador
        if (((TextBoxes)order.Data).Name.Equals("Chat"))
        {
            Loop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
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
                if (Length > 0)
                    if (Text.Length >= Length)
                        return;
            }

            // Adiciona o caracter à caixa de texto
            char @char = Convert.ToChar(e.Unicode);
            if (@char > 31 && @char < 128) Text += e.Unicode;
        }
    }

    public static void Focus()
    {
        // Se o digitalizador não estiver habilitado então isso não é necessário 
        if (Focused?.Viewable == true) return;

        // Percorre toda a árvore de ordem para executar o comando
        Stack<List<Tools.OrderStructure>> stack = new Stack<List<Tools.OrderStructure>>();
        stack.Push(Tools.Order);
        while (stack.Count != 0)
        {
            List<Tools.OrderStructure> top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Data.Visible)
                {
                    // Altera o digitalizador focado para o primeiro visível
                    if (top[i].Data is TextBoxes && !Tools.Order[i].Data.Name.Equals("Chat"))
                    {
                        Focused = top[i];
                        return;
                    }
                    stack.Push(top[i].Nodes);
                }
        }
        Focused = null;
    }

    public static void ChangeFocus()
    {
        List<Tools.OrderStructure>  parent = Focused.Parent != null ? Focused.Parent.Nodes : Tools.Order;
        int index = parent.IndexOf(Focused), temp = index + 1;

        // Altera o digitalizador focado para o próximo
        while (temp != index)
        {
            if (temp == parent.Count) temp = 0;
            if (parent[temp].Viewable && parent[temp].Data is TextBoxes)
            {
                Focused = parent[temp];
                return;
            }
            temp++;
        }
    }
}