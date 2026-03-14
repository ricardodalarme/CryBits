using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Logic;
using System;

namespace CryBits.Client.UI.Game.Views;

internal class ChatView : IView
{
    internal static Panel Panel => Tools.Panels["Chat"];
    internal static TextBox MessageTextBox => Tools.TextBoxes["Chat"];
    private static Button ScrollUpButton => Tools.Buttons["Chat_Up"];
    private static Button ScrollDownButton => Tools.Buttons["Chat_Down"];

    public void Bind()
    {
        MessageTextBox.OnMouseUp += OnMessagePressed;
        ScrollUpButton.OnMouseUp += OnScrollUpPressed;
        ScrollDownButton.OnMouseUp += OnScrollDownPressed;
    }

    public void Unbind()
    {
        MessageTextBox.OnMouseUp -= OnMessagePressed;
        ScrollUpButton.OnMouseUp -= OnScrollUpPressed;
        ScrollDownButton.OnMouseUp -= OnScrollDownPressed;
    }

    private void OnMessagePressed()
    {
        // Focus chat textbox and reset timer
        GameLoop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        Panel.Visible = true;
    }

    private void OnScrollUpPressed()
    {
        if (Chat.LinesFirst > 0)
            Chat.LinesFirst--;
    }

    private void OnScrollDownPressed()
    {
        if (Chat.Order.Count - 1 - Chat.LinesFirst - Chat.LinesVisible > 0)
            Chat.LinesFirst++;
    }
}
