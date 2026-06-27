using CryBits.Definitions.Maps;
using CryBits.Host.Ingress;
using CryBits.Host.Network;
using CryBits.Host.Scheduling;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using CryBits.Transport.Abstractions;

namespace CryBits.Host.Core;

internal sealed class WorldHost
{
    public World Simulation { get; }
    public ITransport Transport { get; }
    public TickPipeline Pipeline { get; }
    public SessionManager Sessions { get; }
    public PackageSender PackageSender { get; }
    public IntentFunnel IntentFunnel { get; } = new();
    public Tick? CurrentTick { get; set; }
    public EntityRegistry Entities => Simulation.Entities;
    public Dictionary<Guid, Map> Maps => Simulation.MapDefs;
    public EntityId? FindPlayer(string name) => Simulation.FindPlayer(name);

    private readonly TickDriver _tickDriver;

    public WorldHost(ITransport transport, World simulation, TickPipeline pipeline,
        SessionManager sessions, PackageSender packageSender)
    {
        Transport = transport;
        Simulation = simulation;
        Pipeline = pipeline;
        Sessions = sessions;
        PackageSender = packageSender;
        _tickDriver = new TickDriver(this);
    }

    public void StartTickLoop(CancellationToken ct)
    {
        _ = _tickDriver.MainAsync(ct);
    }

    public void Tick()
    {
        Simulation.TickCount++;
        var tick = new Tick(Simulation.TickCount, new IntentBuffer(), new EventBuffer());
        CurrentTick = tick;

        Transport.Poll();

        var funnelIntents = IntentFunnel.Drain();
        foreach (var intent in funnelIntents.All)
            tick.Intents.Enqueue(intent);
        Pipeline.Execute(Simulation, tick);

        CurrentTick = null;
    }
}
