using Arch.Core;
using CryBits.Client.Components.Character;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Movement;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using System;
using System.Collections.Generic;
using static CryBits.Globals;

namespace CryBits.Client.Network.Handlers;

internal class NpcHandler(GameContext context)
{
    [PacketHandler]
    internal void Npcs(NpcsPacket packet)
    {
        // Read NPCs dictionary
        Npc.List = packet.List;
    }

    [PacketHandler]
    internal void MapNpcs(MapNpcsPacket packet)
    {
        // Destroy any existing NPC entities from the previous map.
        var npcQuery = new QueryDescription().WithAll<NpcTagComponent, NetworkIdComponent>();
        var toDestroy = new List<(Guid id, Entity e)>();
        context.World.Query(in npcQuery, (Entity e, ref NetworkIdComponent nid) =>
            toDestroy.Add((nid.Value, e)));
        foreach (var (id, e) in toDestroy)
        {
            context.UnregisterNetworkEntity(id);
            context.World.Destroy(e);
        }

        // Spawn new NPC entities for the current map.
        for (byte i = 0; i < packet.Npcs.Length; i++)
        {
            var npc = packet.Npcs[i];
            var data = Npc.List.Get(npc.NpcId);
            var direction = (Direction)npc.Direction;
            var vitals = new short[(byte)Vital.Count];
            for (byte n = 0; n < (byte)Vital.Count; n++) vitals[n] = npc.Vital[n];

            var entity = NpcSpawner.Spawn(context.World, npc.InstanceId, data, npc.X, npc.Y, direction, vitals);
            context.RegisterNetworkEntity(npc.InstanceId, entity);
        }
    }

    [PacketHandler]
    internal void MapNpc(MapNpcPacket packet)
    {
        var old = context.GetNetworkEntity(packet.InstanceId);
        if (old != Entity.Null)
        {
            context.UnregisterNetworkEntity(packet.InstanceId);
            context.World.Destroy(old);
        }

        var data = Npc.List.Get(packet.NpcId);
        var direction = (Direction)packet.Direction;
        var vitals = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) vitals[n] = packet.Vital[n];

        var entity = NpcSpawner.Spawn(context.World, packet.InstanceId, data, packet.X, packet.Y, direction, vitals);
        context.RegisterNetworkEntity(packet.InstanceId, entity);
    }

    [PacketHandler]
    internal void MapNpcMovement(MapNpcMovementPacket packet)
    {
        var npc = context.GetNetworkEntity(packet.InstanceId);

        ref var movement = ref context.World.Get<MovementComponent>(npc);
        byte prevX = movement.TileX, prevY = movement.TileY;
        movement.TileX = packet.X;
        movement.TileY = packet.Y;
        movement.Direction = (Direction)packet.Direction;
        movement.MovementState = (Movement)packet.Movement;
        movement.SpeedPixelsPerSecond = packet.Speed;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;

        // Prime the starting pixel offset when the tile actually changed so the
        // movement system can interpolate back to zero.
        if (prevX != movement.TileX || prevY != movement.TileY)
            switch (movement.Direction)
            {
                case Direction.Up: movement.OffsetY = Grid; break;
                case Direction.Down: movement.OffsetY = -Grid; break;
                case Direction.Right: movement.OffsetX = -Grid; break;
                case Direction.Left: movement.OffsetX = Grid; break;
            }
    }

    [PacketHandler]
    internal void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        var npc = context.GetNetworkEntity(packet.InstanceId);

        ref var movement = ref context.World.Get<MovementComponent>(npc);
        movement.Direction = (Direction)packet.Direction;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;
    }

    [PacketHandler]
    internal void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var npc = context.GetNetworkEntity(packet.InstanceId);

        // Write vital changes directly to ECS.
        ref var vitals = ref context.World.Get<VitalsComponent>(npc);
        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Current[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal void MapNpcDied(MapNpcDiedPacket packet)
    {
        var entity = context.GetNetworkEntity(packet.InstanceId);
        if (entity != Entity.Null)
        {
            context.UnregisterNetworkEntity(packet.InstanceId);
            context.World.Destroy(entity);
        }
    }
}
