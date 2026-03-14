using Arch.System;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Network;
using CryBits.Client.Graphics;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Worlds;
using System;
using System.Diagnostics;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Client.Logic;

internal class GameLoop(RenderPipeline renderPipeline, NetworkClient networkClient, Renderer renderer, GameContext context, InputManager inputManager, PlayerSender playerSender, AudioManager audioManager)
{
    public static GameLoop Instance { get; } = new(
        RenderPipeline.Instance,
        NetworkClient.Instance,
        Renderer.Instance,
        GameContext.Instance,
        InputManager.Instance,
        PlayerSender.Instance,
        AudioManager.Instance);

    // Measured frames per second.
    public static short Fps;

    // Timing counters
    private int TextBoxTimer;
    public static int ChatTimer;

    // Delta-time systems — receive seconds elapsed since last frame.
    private readonly Group<float> _deltaTimeSystems = new(
        "DeltaTimeSystems",
        new FadeSystem(context.World),
        new FogSystem(context.World),
        new WeatherSimulationSystem(context.World, context, audioManager),
        new LocalPlayerInputSystem(context.World, context, inputManager, playerSender),
        new MovementSystem(context.World),
        new CharacterAnimationControllerSystem(context.World),
        new AnimatedSpriteSystem(context.World)
    );

    // High-resolution stopwatch for delta time
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Start the client main loop: handle network, update game state and present frames.
    /// </summary>
    public void Init()
    {
        var timer1000 = 0;
        short fps = 0;

        while (Program.Working)
        {
            // Handle incoming network data.
            networkClient.HandleData();

            // Present the rendered frame.
            renderPipeline.Present();

            // Dispatch window events.
            renderer.RenderWindow.DispatchEvents();

            UpdateTextBox();

            // Use high-resolution stopwatch for delta time
            var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
            _deltaTimeSystems.Update(deltaTime);

            // Update FPS counter.
            if (timer1000 < Environment.TickCount)
            {
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
