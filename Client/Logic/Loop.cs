using System;
using System.Threading;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Graphics;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.UI.Events;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;

namespace CryBits.Client.Logic;

internal static class Loop
{
    // Measured frames per second.
    public static short Fps;

    // Timing counters
    public static int TextBoxTimer;
    public static int ChatTimer;

    /// <summary>
    /// Start the client main loop: handle network, update game state and present frames.
    /// </summary>
    public static void Init()
    {
        var timer1000 = 0;
        var timer30 = 0;
        short fps = 0;

        while (Program.Working)
        {
            // Handle incoming network data.
            Socket.HandleData();

            // Present the rendered frame.
            RenderPipeline.Present();

            // Dispatch window events.
            Renders.RenderWindow.DispatchEvents();

            TextBox();

            if (Screen.Current == Screens.Game)
            {
                TempMap.Current.Logic();
                if (timer30 < Environment.TickCount)
                {
                    // Player logic.
                    for (byte i = 0; i < Player.List.Count; i++)
                        Player.List[i].Logic();

                    // NPC logic.
                    for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
                        if (TempMap.Current.Npc[i].Data != null)
                            TempMap.Current.Npc[i].Logic();

                    // Reset 30 ms timer
                    timer30 = Environment.TickCount + 30;
                }

                // Update information panel visibility
                PanelsEvents.CheckInformation();
            }

            // Yield briefly to avoid busy-wait.
            Thread.Yield();
            Thread.Sleep(1);

            // Update FPS counter.
            if (timer1000 < Environment.TickCount)
            {
                AuthSender.Latency();
                Fps = fps;
                fps = 0;
                timer1000 = Environment.TickCount + 1000;
            }
            else
                fps++;
        }

        // Close the client.
        Program.Close();
    }

    private static void TextBox()
    {
        // Toggle textbox caret visibility on a timer.
        if (TextBoxTimer < Environment.TickCount)
        {
            TextBoxTimer = Environment.TickCount + 500;
            TextBoxesEvents.Signal = !TextBoxesEvents.Signal;

            // Re-evaluate focused textbox if needed.
            Framework.Interfacily.Components.TextBox.Focus();
        }
    }
}
