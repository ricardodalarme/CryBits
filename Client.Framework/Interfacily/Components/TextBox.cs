using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Interfaces;
using SFML.Window;
using static CryBits.Client.Framework.Interfacily.InterfaceUtils;

namespace CryBits.Client.Framework.Interfacily.Components;

/// <summary>Single-line text input control used by the UI system.</summary>
public class TextBox : Component, IMouseUp
{
    public string Text { get; set; } = string.Empty;
    public short MaxCharacters { get; set; }
    public short Width { get; set; }
    public bool Password { get; set; }

    public event Action? OnMouseUp;

    /// <summary>Currently focused textbox, or null if none.</summary>
    public static TextBox? Focused;

    public void MouseUp()
    {
        if (!IsAbove(new Rectangle(Position, new Size(Width, Textures.TextBox.ToSize().Height)))) return;

        // Set focus to this textbox
        Focused = this;

        OnMouseUp?.Invoke();
    }

    public void TextEntered(TextEventArgs e)
    {
        if (!Viewable(this)) return;
        if (!string.IsNullOrEmpty(Text))
        {
            // Handle backspace
            if (e.Unicode == "\b" && Text.Length > 0)
            {
                Text = Text.Remove(Text.Length - 1);
                return;
            }

            // Respect MaxCharacters
            if (MaxCharacters > 0)
                if (Text.Length >= MaxCharacters)
                    return;
        }

        // Append printable character
        var @char = Convert.ToChar(e.Unicode);
        if (@char > 31 && @char < 128) Text += e.Unicode;
    }

    /// <summary>Focus the first visible textbox in the current screen, if any.</summary>
    public static void Focus()
    {
        if ((Focused != null) & Viewable(Focused)) return;

        // Traverse component tree to find first focusable textbox
        var stack = new Stack<List<Component>>();
        stack.Push(Screen.Current.Body);
        while (stack.Count != 0)
        {
            var top = stack.Pop();

            for (byte i = 0; i < top.Count; i++)
                if (top[i].Visible)
                {
                    // Set focus to the first visible textbox (skip chat)
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

        // Advance focus to the next focusable TextBox.
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
