using CryBits.Definitions.Catalog;
using CryBits.Host.Network;
using CryBits.Host.Network.Senders;
using CryBits.Host.Services;
using CryBits.Simulation.Systems.Combat;
using CryBits.Simulation.Systems.Inventory;
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

    public SessionManager Sessions { get; } = new();

    public EntityId? FindPlayer(string name) => Simulation.FindPlayer(name);

    public WorldHost()
    {
        Current = this;

        var movementSystem = new MovementSystem();
        var npcBrainSystem = new NpcBrainSystem(DefinitionCatalog.Instance);
        var combatSystem = new CombatSystem(DefinitionCatalog.Instance);
        var levelingSystem = new LevelingSystem(DefinitionCatalog.Instance);
        var deathSystem = new DeathSystem(DefinitionCatalog.Instance);
        var groundItemSystem = new GroundItemSystem();
        var equipmentSystem = new EquipmentSystem(DefinitionCatalog.Instance);
        var inventorySystem = new InventorySystem(DefinitionCatalog.Instance);
        var hotbarSystem = new HotbarSystem();
        var tradeSystem = new TradeSystem(DefinitionCatalog.Instance);
        var shopSystem = new ShopSystem(DefinitionCatalog.Instance);

        Pipeline = new TickPipeline();
        Pipeline.AddSystem(new VitalsRegenSystem(DefinitionCatalog.Instance));
        Pipeline.AddSystem(movementSystem);
        Pipeline.AddSystem(npcBrainSystem);
        Pipeline.AddSystem(combatSystem);
        Pipeline.AddSystem(levelingSystem);
        Pipeline.AddSystem(deathSystem);
        Pipeline.AddSystem(groundItemSystem);
        Pipeline.AddSystem(equipmentSystem);
        Pipeline.AddSystem(inventorySystem);
        Pipeline.AddSystem(hotbarSystem);
        Pipeline.AddSystem(tradeSystem);
        Pipeline.AddSystem(shopSystem);
        Pipeline.AddSystem(new PartySystem());
        Pipeline.AddSystem(new SpawnSystem(DefinitionCatalog.Instance));
        Pipeline.AddSystem(new ReplicationService(
            PlayerSender.Instance, NpcSender.Instance,
            MapSender.Instance, CombatSender.Instance));
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
                var session = Sessions.Get(new EntityId(chat.RecipientId));
                if (session != null)
                    ChatSender.SendMessage(session, chat.Text, chat.ColorArgb);
            }
        }

        Simulation.CurrentTick = null;
    }
}
