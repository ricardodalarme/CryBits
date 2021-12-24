using System;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Logic;

namespace CryBits.Client.UI.Events;

internal static class TextBoxesEvents
{
    // Digitalizador focado
    public static bool Signal;

    public static void Bind()
    {
       TextBoxes.Chat.OnMouseUp += Chat_MouseUp;
    }

    // Eventos
    public static void Chat_MouseUp()
    {
        // Altera o foco do digitalizador
        Loop.ChatTimer = Environment.TickCount + Chat.SleepTimer;
        Panels.Chat.Visible = true;
    }
}