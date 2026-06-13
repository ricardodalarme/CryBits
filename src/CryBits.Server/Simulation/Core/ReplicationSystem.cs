using CryBits.Definitions.Common;
using CryBits.Server.Network.Senders;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using CryBits.Simulation.State;
using CryBits.Server.Core;

namespace CryBits.Server.Simulation.Core;

internal sealed class ReplicationSystem(
    PlayerSender playerSender,
    NpcSender npcSender,
    MapSender mapSender,
    ChatSender chatSender) : ISimulationSystem
{
    public void Execute(GameWorld world, Tick tick)
    {
        // 0. Process chat intents (pure network routing, not gameplay)
        foreach (var intent in tick.Intents.All)
        {
            if (intent is ChatMessageIntent chat)
            {
                switch (chat.Type)
                {
                    case Message.Map: chatSender.MessageMap(chat.SourceEntityId, chat.Text); break;
                    case Message.Global: chatSender.MessageGlobal(chat.SourceEntityId, chat.Text); break;
                    case Message.Private: chatSender.MessagePrivate(chat.SourceEntityId, chat.Addressee, chat.Text); break;
                }
            }
        }

        // 1. Replicate component changes via dirty tracking
        foreach (var (entityId, componentType) in world.Dirty.All)
        {
            var entity = world.Entities.Get(entityId);
            if (entity == null) continue;

            if (componentType == typeof(Position))
                ReplicatePosition(world, entityId, entity.Get<Position>()!);
            else if (componentType == typeof(Vitals))
                ReplicateVitals(world, entityId, entity);
            else if (componentType == typeof(StatBlock))
                ReplicateStats(world, entityId, entity);
            else if (componentType == typeof(InventoryState))
                playerSender.PlayerInventory(entityId);
            else if (componentType == typeof(EquipmentState))
                playerSender.PlayerEquipments(entityId);
            else if (componentType == typeof(HotbarState))
                playerSender.PlayerHotbar(entityId);
            else if (componentType == typeof(NpcState))
                ReplicateNpcState(world, entityId, entity);
        }

        // 2. Replicate tick events
        foreach (var ev in tick.Events.Events)
        {
            switch (ev)
            {
                case ChatMessageEvent chat:
                    {
                        var session = world.SessionMap.Get(new EntityId(chat.RecipientId));
                        if (session != null)
                            chatSender.SendMessage(session, chat.Text, chat.ColorArgb);
                        break;
                    }
                case EntityDiedEvent died:
                    {
                        if (died.EntityIsPlayer)
                            ReplicateEntityDied(world, died);
                        break;
                    }
            }
        }

        world.Dirty.Clear();
    }

    private void ReplicatePosition(GameWorld world, EntityId entityId, Position pos)
    {
        var entity = world.Entities.Get(entityId);
        if (entity == null) return;
        var combat = entity.Get<CombatState>();

        if (combat?.GettingMap == true) return;

        if (entity.Has<PlayerTag>())
        {
            playerSender.PlayerMove(entityId, 1);
        }
        else if (entity.Has<NpcTag>())
        {
            npcSender.MapNpcMovement(entityId, 1);
        }
    }

    private void ReplicateVitals(GameWorld world, EntityId entityId, EntityState entity)
    {
        if (entity.Has<PlayerTag>())
            playerSender.PlayerVitals(entityId);
        else if (entity.Has<NpcTag>())
            npcSender.MapNpcVitals(entityId);
    }

    private void ReplicateStats(GameWorld world, EntityId entityId, EntityState entity)
    {
        if (!entity.Has<PlayerTag>()) return;
        playerSender.PlayerExperience(entityId);
        var pos = entity.Get<Position>();
        if (pos != null)
            mapSender.MapPlayers(entityId);
    }

    private void ReplicateNpcState(GameWorld world, EntityId entityId, EntityState entity)
    {
        var npcState = entity.Get<NpcState>();
        if (npcState == null) return;

        if (!npcState.Alive)
            npcSender.MapNpcDied(entityId);
    }

    private void ReplicateEntityDied(GameWorld world, EntityDiedEvent died)
    {
        var playerId = world.FindPlayerByValue(died.EntityId);
        if (playerId == null) return;

        var entity = world.Entities.Get(playerId.Value);
        if (entity == null) return;

        var vitals = entity.Get<Vitals>();
        if (vitals != null)
        {
            vitals.Hp = vitals.MaxHp;
            vitals.Mp = vitals.MaxMp;
            world.Dirty.Mark<Vitals>(playerId.Value);
        }
    }
}
