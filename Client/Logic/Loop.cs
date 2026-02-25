using System;
using System.Diagnostics;
using System.Threading;
using Arch.System;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Graphics;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.Systems;
using CryBits.Client.UI.Events;
using CryBits.Client.Worlds;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;

namespace CryBits.Client.Logic;

internal static class Loop
{
    // Measured frames per second.
    public static short Fps;

    // Timing counters
    public static int TextBoxTimer;
    public static int ChatTimer;


    // Delta-time systems — receive seconds elapsed since last frame.
    private static readonly Group<float> _deltaTimeSystems = new(
        "DeltaTimeSystems",
        new FadeSystem(GameContext.Instance.World),
        new ScrollingSpriteSystem(GameContext.Instance.World),
        new CharacterAnimationControllerSystem(GameContext.Instance.World),
        new AnimatedSpriteSystem(GameContext.Instance.World),
        new DamageTintSystem(GameContext.Instance.World)
    );

    // High-resolution stopwatch for delta time
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

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
                MapInstance.Current.Logic();
                if (timer30 < Environment.TickCount)
                {
                    // Player logic.
                    for (byte i = 0; i < Player.List.Count; i++)
                        Player.List[i].Logic();

                    // NPC logic.
                    for (byte i = 0; i < MapInstance.Current.Npc.Length; i++)
                        if (MapInstance.Current.Npc[i].Data != null)
                            MapInstance.Current.Npc[i].Logic();

                    // Reset 30 ms timer
                    timer30 = Environment.TickCount + 30;
                }

                // Update information panel visibility
                PanelsEvents.CheckInformation();
            }

            // Use high-resolution stopwatch for delta time
            var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
            _deltaTimeSystems.Update(deltaTime);

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
