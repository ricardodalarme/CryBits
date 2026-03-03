using System;
using System.Diagnostics;
using System.Threading;
using Arch.System;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Graphics;
using CryBits.Client.Network;
using CryBits.Client.Network.Senders;
using CryBits.Client.Systems.Combat;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Worlds;
using Screen = CryBits.Client.Framework.Interfacily.Components.Screen;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Client.Logic;

internal class GameLoop(RenderPipeline renderPipeline)
{
    public static GameLoop Instance { get; } = new(RenderPipeline.Instance);

    // Measured frames per second.
    public static short Fps;

    // Timing counters
    private static int TextBoxTimer;
    public static int ChatTimer;

    // Delta-time systems — receive seconds elapsed since last frame.
    private static readonly Group<float> _deltaTimeSystems = new(
        "DeltaTimeSystems",
        new FadeSystem(GameContext.Instance.World),
        new ScrollingSpriteSystem(GameContext.Instance.World),
        new WeatherSimulationSystem(GameContext.Instance.World, GameContext.Instance),
        new MovementSystem(GameContext.Instance.World),
        new CharacterAnimationControllerSystem(GameContext.Instance.World),
        new AnimatedSpriteSystem(GameContext.Instance.World),
        new DamageTintSystem(GameContext.Instance.World)
    );

    // High-resolution stopwatch for delta time
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Start the client main loop: handle network, update game state and present frames.
    /// </summary>
    public void Init()
    {
        var timer1000 = 0;
        var timer30 = 0;
        short fps = 0;

        while (Program.Working)
        {
            // Handle incoming network data.
            NetworkClient.Instance.HandleData();

            // Present the rendered frame.
            renderPipeline.Present();

            // Dispatch window events.
            Renderer.Instance.RenderWindow.DispatchEvents();

            UpdateTextBox();

            if (Screen.Current == Screens.Game)
            {
                if (timer30 < Environment.TickCount)
                {
                    Player.Me.Logic();

                    // Reset 30 ms timer
                    timer30 = Environment.TickCount + 30;
                }
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
                AuthSender.Instance.Latency();
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

    private void UpdateTextBox()
    {
        // Toggle textbox caret visibility on a timer.
        if (TextBoxTimer < Environment.TickCount)
        {
            TextBoxTimer = Environment.TickCount + 500;
            TextBox.BlinkSignal = !TextBox.BlinkSignal;

            // Re-evaluate focused textbox if needed.
            TextBox.Focus();
        }
    }
}
