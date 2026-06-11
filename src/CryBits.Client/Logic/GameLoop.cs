using CryBits.Client.Framework.Network;
using CryBits.Client.Graphics;
using CryBits.Client.Managers;
using CryBits.Client.Systems;
using System;
using System.Diagnostics;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Client.Logic;

internal class GameLoop(
    RenderPipeline renderPipeline,
    NetworkClient networkClient,
    Renderer renderer,
    InputManager inputManager,
    SystemScheduler scheduler)
{
    public static GameLoop Instance { get; } = new(
        RenderPipeline.Instance,
        NetworkClient.Instance,
        Renderer.Instance,
        InputManager.Instance,
        SystemScheduler.Instance);

    // Measured frames per second.
    public static short Fps;

    // Timing counters
    private long TextBoxTimer;

    // High-resolution stopwatch for delta time.
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Start the client main loop: handle network, update game state and present frames.
    /// </summary>
    public void Init()
    {
        long timer1000 = 0;
        short fps = 0;

        scheduler.Initialize();

        while (Program.Working)
        {
            try
            {
                // Handle incoming network data.
                networkClient.HandleData();

                // Present the rendered frame.
                renderPipeline.Present();

                // Clear previous frame's edge state, then dispatch window events to fill it.
                inputManager.BeginFrame();
                renderer.RenderWindow.DispatchEvents();

                UpdateTextBox();

                // Use high-resolution stopwatch for delta time.
                var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
                _stopwatch.Restart();

                scheduler.Update.BeforeUpdate(in deltaTime);
                scheduler.Update.Update(in deltaTime);
                scheduler.Update.AfterUpdate(in deltaTime);

                // Update FPS counter.
                if (timer1000 < Environment.TickCount64)
                {
                    Fps = fps;
                    fps = 0;
                    timer1000 = Environment.TickCount64 + 1000;
                }
                else
                    fps++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
        }


        // Close the client.
        scheduler.Dispose();
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
