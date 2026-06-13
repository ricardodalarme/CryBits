using CryBits.Host.Core;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Simulation.Core;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CryBits.Host.Logic;

internal sealed class Loop(NetworkServer networkServer, TickPipeline pipeline, ChatSender chatSender)
{
    public static Loop Instance { get; } = new(
        NetworkServer.Instance,
        TickPipeline.CreateDefault(),
        ChatSender.Instance);

    // Target simulation rate: 20 ticks per second (50ms per tick)
    private const int TicksPerSecond = 20;

    // Measured loops per second (static so CpsCommand can access without Instance).
    public static int Cps;
    private long _cpsTimer;

    public async Task MainAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000 / TicksPerSecond));
        var cps = 0;

        while (await timer.WaitForNextTickAsync(ct))
        {
            try
            {
                var tick = new Tick(Environment.TickCount64, new IntentBuffer(), new EventBuffer());
                WorldHost.Current.CurrentTick = tick;

                // Handle incoming network data — handlers call systems which emit events.
                networkServer.HandleData();

                // Run pipeline — systems react to events emitted during handler processing.
                pipeline.Execute(WorldHost.Current.Simulation, tick);

                // Route chat messages (server-side transport concern, not gameplay)
                foreach (var ev in tick.Events.Events)
                {
                    if (ev is ChatMessageEvent chat)
                    {
                        var session = WorldHost.Current.SessionMap.Get(new EntityId(chat.RecipientId));
                        if (session != null)
                            chatSender.SendMessage(session, chat.Text, chat.ColorArgb);
                    }
                }

                WorldHost.Current.CurrentTick = null;

                // Compute CPS.
                if (_cpsTimer < Environment.TickCount64)
                {
                    Cps = cps;
                    cps = 1;
                    _cpsTimer = Environment.TickCount64 + 1000;
                }
                else
                    cps++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Main loop threw an exception: {ex}");
            }
        }
    }

    public void Commands(Action<string> dispatch, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                Console.Write("Execute: ");
                dispatch(Console.ReadLine());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Command loop threw an exception: {ex}");
            }
        }
    }
}
