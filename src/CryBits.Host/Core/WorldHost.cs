using CryBits.Definitions.Catalog;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Transport.Abstractions;
using CryBits.Host.Services;
using CryBits.Simulation.Systems.Combat;
using CryBits.Simulation.Systems.Inventory;
using CryBits.Simulation.Systems.Regeneration;
using CryBits.Simulation.Systems.Movement;
using CryBits.Simulation.Systems.Npc;
using CryBits.Simulation.Systems.Party;
using CryBits.Simulation.Systems.Progression;
using CryBits.Simulation.Systems.Shops;
using CryBits.Simulation.Systems.Spawning;
using CryBits.Simulation.Systems.Trade;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.Spawners;
using CryBits.Simulation.State;
using System;
using System.Collections.Generic;

namespace CryBits.Host.Core;

internal sealed class WorldHost
{
    public static WorldHost Current { get; private set; } = null!;

    public World Simulation { get; } = new();
    public ITransport Transport { get; }
    public TickPipeline Pipeline { get; }
    public ChatSender ChatSender { get; } = ChatSender.Instance;
    public Dictionary<Guid, MapState> Maps => Simulation.Maps;
    public EntityRegistry Entities => Simulation.Entities;
    public DirtyTracking Dirty => Simulation.Dirty;
    public Tick? CurrentTick { get; set; }

    public SessionManager Sessions { get; } = new();

    public EntityId? FindPlayer(string name) => Simulation.FindPlayer(name);

    /// <summary>
    /// Loads all game data, creates map instances, and spawns NPCs.
    /// Called by both the server and offline client after construction.
    /// </summary>
    public void Initialize()
    {
        CryBits.Host.Persistence.DataLoader.Instance.LoadAll();
        foreach (var map in DefinitionCatalog.Instance.Maps.Values)
        {
            var mapState = new MapState(map.Id, map);
            mapState.SpawnItems(Simulation.Entities);
            Simulation.Maps.Add(map.Id, mapState);
            for (byte i = 0; i < map.Npc.Count; i++)
                NpcSpawner.Spawn(Simulation, DefinitionCatalog.Instance, mapState.Id, i);
        }
    }

    public WorldHost(ITransport transport)
    {
        Transport = transport;
        Current = this;

        var movementSystem = new MovementSystem();
        var npcBrainSystem = new NpcBrainSystem(DefinitionCatalog.Instance);
        var combatSystem = new CombatSystem(DefinitionCatalog.Instance);
        var levelingSystem = new LevelingSystem(DefinitionCatalog.Instance);
        var deathSystem = new DeathSystem(DefinitionCatalog.Instance);
        var groundItemSystem = new GroundItemSystem(DefinitionCatalog.Instance);
        var equipmentSystem = new EquipmentSystem(DefinitionCatalog.Instance);
        var inventorySystem = new InventorySystem(DefinitionCatalog.Instance);
        var hotbarSystem = new HotbarSystem();
        var tradeSystem = new TradeSystem();
        var shopSystem = new ShopSystem(DefinitionCatalog.Instance);

        Pipeline = new TickPipeline();
        Pipeline.AddSystem(new VitalsRegenSystem());
        Pipeline.AddSystem(npcBrainSystem);
        Pipeline.AddSystem(movementSystem);
        Pipeline.AddSystem(combatSystem);
        Pipeline.AddSystem(new AggroSystem());
        Pipeline.AddSystem(levelingSystem);
        Pipeline.AddSystem(deathSystem);
        Pipeline.AddSystem(groundItemSystem);
        Pipeline.AddSystem(equipmentSystem);
        Pipeline.AddSystem(inventorySystem);
        Pipeline.AddSystem(hotbarSystem);
        Pipeline.AddSystem(tradeSystem);
        Pipeline.AddSystem(shopSystem);
        Pipeline.AddSystem(new PartySystem());
        Pipeline.AddSystem(new NpcRespawnSystem(DefinitionCatalog.Instance));
        Pipeline.AddSystem(new ReplicationService(
            PlayerSender.Instance, NpcSender.Instance,
            MapSender.Instance, CombatSender.Instance));

        Transport.OnConnected += OnSessionConnected;
        Transport.OnDisconnected += OnSessionDisconnected;
        Transport.OnDataReceived += OnSessionDataReceived;
    }

    private void OnSessionConnected(Guid sessionId)
    {
        Sessions.Add(new Session(sessionId));
    }

    private void OnSessionDisconnected(Guid sessionId)
    {
        var session = Sessions.Find(s => s.Id == sessionId);
        if (session?.Character is { } characterId)
            CharacterService.Instance.Leave(characterId);
        if (session != null)
            Sessions.Remove(session);
    }

    private void OnSessionDataReceived(Guid sessionId, byte[] data)
    {
        var session = Sessions.Find(s => s.Id == sessionId);
        if (session != null)
        {
            PacketDispatcher.Dispatch(session, data);
        }
    }

    /// <summary>Registers the standard gameplay services needed for online and offline play.</summary>
    public void RegisterDefaultServices(bool includeEditor = false)
    {
        PacketDispatcher.Register(AuthService.Instance);
        PacketDispatcher.Register(CharacterService.Instance);
        PacketDispatcher.Register(PlayerService.Instance);
        PacketDispatcher.Register(ChatService.Instance);
        PacketDispatcher.Register(PartyService.Instance);
        PacketDispatcher.Register(TradeService.Instance);
        PacketDispatcher.Register(ShopService.Instance);
        if (includeEditor)
            PacketDispatcher.Register(EditorService.Instance);
    }

    public void Tick()
    {
        Simulation.TickCount++;
        var tick = new Tick(Simulation.TickCount, new IntentBuffer(), new EventBuffer { TickNumber = Simulation.TickCount });
        CurrentTick = tick;

        Transport.Poll();
        Pipeline.Execute(Simulation, tick);

        foreach (var ev in tick.Events.Events)
        {
            if (ev is ChatMessageEvent chat)
            {
                var session = Sessions.Get(chat.RecipientId);
                if (session != null)
                    ChatSender.SendMessage(session, chat.Text, chat.ColorArgb);
            }
        }

        CurrentTick = null;
    }
}
