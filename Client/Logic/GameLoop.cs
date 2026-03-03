using System;
using System.Diagnostics;
using System.Threading;
using Arch.System;
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
    private static long TextBoxTimer;
    public static long ChatTimer;

    // Delta-time systems — receive seconds elapsed since last frame.
    private static readonly Group<float> _deltaTimeSystems = new(
        "DeltaTimeSystems",
        new FadeSystem(GameContext.Instance.World),
        new ScrollingSpriteSystem(GameContext.Instance.World),
        new WeatherSimulationSystem(GameContext.Instance.World, GameContext.Instance),
        new LocalPlayerInputSystem(GameContext.Instance.World, GameContext.Instance),
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
        long timer1000 = 0;
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

            // Use high-resolution stopwatch for delta time
            var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
            _deltaTimeSystems.Update(deltaTime);

            // Yield briefly to avoid busy-wait.
            Thread.Yield();
            Thread.Sleep(1);

            // Update FPS counter.
            if (timer1000 < Environment.TickCount64)
            {
                AuthSender.Instance.Latency();
                Fps = fps;
                fps = 0;
                timer1000 = Environment.TickCount64 + 1000;
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
        if (TextBoxTimer < Environment.TickCount64)
        {
            TextBoxTimer = Environment.TickCount64 + 500;
            TextBox.BlinkSignal = !TextBox.BlinkSignal;

            // Re-evaluate focused textbox if needed.
            TextBox.Focus();
        }
    }
}
