using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Simulation.Core;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using System;
using System.Collections.Generic;

namespace CryBits.Host.Core;

internal sealed class WorldHost
{
    public static WorldHost Current { get; private set; } = null!;

    public World Simulation { get; } = new();
    public NetworkServer NetworkServer { get; } = NetworkServer.Instance;
    public TickPipeline Pipeline { get; } = TickPipeline.CreateDefault();
    public ChatSender ChatSender { get; } = ChatSender.Instance;

    public Dictionary<Guid, MapState> Maps => Simulation.Maps;
    public EntityRegistry Entities => Simulation.Entities;
    public DirtyTracking Dirty => Simulation.Dirty;
    public Tick? CurrentTick => Simulation.CurrentTick;

    public List<GameSession> Sessions { get; } = [];
    public SessionMap SessionMap { get; } = new();

    public EntityId? FindPlayer(string name) => Simulation.FindPlayer(name);

    public WorldHost() => Current = this;

    public void Tick()
    {
        Simulation.TickCount++;
        var tick = new Tick(Simulation.TickCount, new IntentBuffer(), new EventBuffer());
        Simulation.CurrentTick = tick;

        NetworkServer.HandleData();
        Pipeline.Execute(Simulation, tick);

        foreach (var ev in tick.Events.Events)
        {
            if (ev is ChatMessageEvent chat)
            {
                var session = SessionMap.Get(new EntityId(chat.RecipientId));
                if (session != null)
                    ChatSender.SendMessage(session, chat.Text, chat.ColorArgb);
            }
        }

        Simulation.CurrentTick = null;
    }
}
