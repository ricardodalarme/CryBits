using System;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Logic;

namespace CryBits.Client.UI.Events;

internal static class TextBoxesEvents
{
    /// <summary>
    /// Blink signal for focused textboxes' caret.
    /// </summary>
    public static bool Signal;

    public static void Bind()
    {
        TextBoxes.Chat.OnMouseUp += Chat_MouseUp;
    }

    public static void Chat_MouseUp()
    {
        // Focus chat textbox and reset timer
        Loop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        Panels.Chat.Visible = true;
    }
}
