using System;
using Arch.Core;
using CryBits.Client.Components.Movement;
using CryBits.Client.Entities;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
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
        // Read temporary NPCs for the current map
        context.CurrentMap.Npc = new NpcInstance[packet.Npcs.Length];
        for (byte i = 0; i < context.CurrentMap.Npc.Length; i++)
        {
            context.CurrentMap.Npc[i] = new NpcInstance();
            context.CurrentMap.Npc[i].Data = Npc.List.Get(packet.Npcs[i].NpcId);
            context.CurrentMap.Npc[i].X = packet.Npcs[i].X;
            context.CurrentMap.Npc[i].Y = packet.Npcs[i].Y;
            context.CurrentMap.Npc[i].Direction = (Direction)packet.Npcs[i].Direction;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                context.CurrentMap.Npc[i].Vital[n] = packet.Npcs[i].Vital[n];
        }
    }

    [PacketHandler]
    internal void MapNpc(MapNpcPacket packet)
    {
        var i = packet.Index;
        context.CurrentMap.Npc[i].Data = Npc.List.Get(packet.NpcId);
        context.CurrentMap.Npc[i].X = packet.X;
        context.CurrentMap.Npc[i].Y = packet.Y;
        context.CurrentMap.Npc[i].Direction = (Direction)packet.Direction;
        context.CurrentMap.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) context.CurrentMap.Npc[i].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal void MapNpcMovement(MapNpcMovementPacket packet)
    {
        var i = packet.Index;
        var npc = context.CurrentMap.Npc[i];

        byte prevX = npc.X, prevY = npc.Y;
        npc.X = packet.X;
        npc.Y = packet.Y;
        npc.Direction = (Direction)packet.Direction;

        if (npc.Entity == Entity.Null) return;

        ref var movement = ref context.World.Get<MovementComponent>(npc.Entity);
        movement.TileX = packet.X;
        movement.TileY = packet.Y;
        movement.Direction = (Direction)packet.Direction;
        movement.MovementState = (Movement)packet.Movement;
        movement.SpeedPixelsPerSecond = packet.Speed;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;

        // Prime the starting pixel offset when the tile actually changed so the
        // movement system can interpolate back to zero.
        if (prevX != npc.X || prevY != npc.Y)
            switch (npc.Direction)
            {
                case Direction.Up: movement.OffsetY = Grid; break;
                case Direction.Down: movement.OffsetY = -Grid; break;
                case Direction.Right: movement.OffsetX = -Grid; break;
                case Direction.Left: movement.OffsetX = Grid; break;
            }
    }

    [PacketHandler]
    internal void MapNpcAttack(MapNpcAttackPacket packet)
    {
        var index = packet.Index;
        var victim = packet.Victim;
        var victimType = (Target)packet.VictimType;

        // Start NPC attack
        context.CurrentMap.Npc[index].Attacking = true;
        context.CurrentMap.Npc[index].AttackTimer = Environment.TickCount;

        if (victim == string.Empty || victimType == Target.None) return;

        Character victimData = victimType switch
        {
            Target.Player => Player.Get(victim),
            Target.Npc => context.CurrentMap.Npc[byte.Parse(victim)],
            _ => throw new ArgumentOutOfRangeException()
        };

        // Apply damage to victim
        var world = context.World;
        BloodSplatSpawner.Spawn(world, victimData.X, victimData.Y);
        victimData.Hurt = Environment.TickCount;
    }

    [PacketHandler]
    internal void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        var i = packet.Index;
        var npc = context.CurrentMap.Npc[i];
        npc.Direction = (Direction)packet.Direction;

        if (npc.Entity == Entity.Null) return;

        ref var movement = ref context.World.Get<MovementComponent>(npc.Entity);
        movement.Direction = (Direction)packet.Direction;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;
    }

    [PacketHandler]
    internal void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var index = packet.Index;

        // Set NPC vitals
        for (byte n = 0; n < (byte)Vital.Count; n++)
            context.CurrentMap.Npc[index].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal void MapNpcDied(MapNpcDiedPacket packet)
    {
        var i = packet.Index;

        // Destroy entity
        context.World.Destroy(context.CurrentMap.Npc[i].Entity);

        // Clear NPC data on death
        context.CurrentMap.Npc[i].Data = null;
        context.CurrentMap.Npc[i].X = 0;
        context.CurrentMap.Npc[i].Y = 0;
        context.CurrentMap.Npc[i].Vital = new short[(byte)Vital.Count];
        context.CurrentMap.Npc[i].Entity = Entity.Null;
    }
}
