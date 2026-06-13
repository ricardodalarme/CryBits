using CryBits.Definitions.Catalog;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Simulation.Core;
using CryBits.Host.Systems.Combat;
using CryBits.Host.Systems.Inventory;
using CryBits.Host.Systems.Movement;
using CryBits.Host.Systems.Npc;
using CryBits.Host.Systems.Party;
using CryBits.Host.Systems.Progression;
using CryBits.Host.Systems.Shops;
using CryBits.Host.Systems.Spawning;
using CryBits.Host.Systems.Trade;
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
    public TickPipeline Pipeline { get; }
    public ChatSender ChatSender { get; } = ChatSender.Instance;

    public Dictionary<Guid, MapState> Maps => Simulation.Maps;
    public EntityRegistry Entities => Simulation.Entities;
    public DirtyTracking Dirty => Simulation.Dirty;
    public Tick? CurrentTick => Simulation.CurrentTick;

    public List<GameSession> Sessions { get; } = [];
    public SessionMap SessionMap { get; } = new();

    public EntityId? FindPlayer(string name) => Simulation.FindPlayer(name);

    public WorldHost()
    {
        Current = this;

        Pipeline = new TickPipeline();
        Pipeline.AddSystem(new VitalsRegenSystem());
        Pipeline.AddSystem(MovementSystem.Instance);
        Pipeline.AddSystem(NpcBrainSystem.Instance);
        Pipeline.AddSystem(new CombatSystem(CombatSender.Instance));
        Pipeline.AddSystem(LevelingSystem.Instance);
        Pipeline.AddSystem(new DeathSystem(DefinitionCatalog.Instance));
        Pipeline.AddSystem(new GroundItemSystem(MapSender.Instance));
        Pipeline.AddSystem(new EquipmentSystem(DefinitionCatalog.Instance));
        Pipeline.AddSystem(InventorySystem.Instance);
        Pipeline.AddSystem(new HotbarSystem(InventorySystem.Instance));
        Pipeline.AddSystem(new TradeSystem(InventorySystem.Instance, DefinitionCatalog.Instance));
        Pipeline.AddSystem(new ShopSystem(InventorySystem.Instance, DefinitionCatalog.Instance));
        Pipeline.AddSystem(new PartySystem());
        Pipeline.AddSystem(new SpawnSystem());
        Pipeline.AddSystem(new ReplicationSystem(
            PlayerSender.Instance, NpcSender.Instance,
            MapSender.Instance));
    }

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
