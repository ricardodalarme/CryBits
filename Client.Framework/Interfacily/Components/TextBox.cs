using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

public class TextBox : Component, IMouseUp
{
    // Propriedades
    public string Text { get; set; } = string.Empty;
    public short MaxCharacters { get; set; }
    public short Width { get; set; }
    public bool Password { get; set; }

    // Ações
    public event Action? OnMouseUp;

    public static TextBox? Focused;

    public void MouseUp()
    {
        // Somente se necessário
        if (!IsAbove(new Rectangle(Position, new Size(Width, Textures.TextBox.ToSize().Height)))) return;

        // Define o foco no digitalizador
        Focused = this;

        OnMouseUp?.Invoke();
    }

    public void TextEntered(TextEventArgs e)
    {
        if (!Viewable(this)) return;
        if (!string.IsNullOrEmpty(Text))
        {
            // Apaga a última letra do texto
            if (e.Unicode == "\b" && Text.Length > 0)
            {
                Text = Text.Remove(Text.Length - 1);
                return;
            }

            // Não adicionar se já estiver no máximo de caracteres
            if (MaxCharacters > 0)
                if (Text.Length >= MaxCharacters)
                    return;
        }

        // Adiciona o caracter à caixa de texto
        var @char = Convert.ToChar(e.Unicode);
        if (@char > 31 && @char < 128) Text += e.Unicode;
    }

    public static void Focus()
    {
        // Se o digitalizador não estiver habilitado então isso não é necessário 
        if ((Focused != null) & Viewable(Focused)) return;

        // Percorre toda a árvore de ordem para executar o comando
        var stack = new Stack<List<Component>>();
        stack.Push(Screen.Current.Body);
        while (stack.Count != 0)
        {
            var top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Visible)
                {
                    // Altera o digitalizador focado para o primeiro visível
                    if (top[i] is TextBox && !Screen.Current.Body[i].Name.Equals("Chat"))
                    {
                        Focused = (TextBox)top[i];
                        return;
                    }
                    stack.Push(top[i].Children);
                }
        }
        Focused = null;
    }

    public static void ChangeFocus()
    {
        var parent = Focused.Parent != null ? Focused?.Parent.Children : Screen.Current.Body;
        int index = parent.IndexOf(Focused), temp = index + 1;

        // Altera o digitalizador focado para o próximo
        while (temp != index)
        {
            if (temp == parent.Count) temp = 0;
            if (Viewable(parent[temp]) && parent[temp] is TextBox textBox)
            {
                Focused = textBox;
                return;
            }
            temp++;
        }
    }
    public override string ToString() => "[TextBox] " + Name;
}