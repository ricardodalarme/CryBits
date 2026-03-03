using System;
using Arch.Core;
using CryBits.Client.Components.Combat;
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
        // Destroy any existing NPC entities before replacing the array (map transition).
        if (context.CurrentMap.Npc != null)
            foreach (var existing in context.CurrentMap.Npc)
                if (existing.Entity != Entity.Null) context.World.Destroy(existing.Entity);

        // Read temporary NPCs for the current map and immediately spawn their entities.
        context.CurrentMap.Npc = new NpcInstance[packet.Npcs.Length];
        for (byte i = 0; i < context.CurrentMap.Npc.Length; i++)
        {
            var npc = new NpcInstance();
            npc.Data = Npc.List.Get(packet.Npcs[i].NpcId);
            npc.X = packet.Npcs[i].X;
            npc.Y = packet.Npcs[i].Y;
            npc.Direction = (Direction)packet.Npcs[i].Direction;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                npc.Vital[n] = packet.Npcs[i].Vital[n];

            npc.Entity = NpcSpawner.Spawn(context.World, npc);
            context.CurrentMap.Npc[i] = npc;
        }
    }

    [PacketHandler]
    internal void MapNpc(MapNpcPacket packet)
    {
        var i = packet.Index;
        var npc = context.CurrentMap.Npc[i];

        // Destroy previous entity (respawn scenario — MapNpcDied sets Entity.Null already,
        // but guard is a safety net).
        if (npc.Entity != Entity.Null) context.World.Destroy(npc.Entity);

        npc.Data = Npc.List.Get(packet.NpcId);
        npc.X = packet.X;
        npc.Y = packet.Y;
        npc.Direction = (Direction)packet.Direction;
        npc.Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) npc.Vital[n] = packet.Vital[n];

        npc.Entity = NpcSpawner.Spawn(context.World, npc);
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
        var npc = context.CurrentMap.Npc[index];

        ref var state = ref context.World.Get<CharacterStateComponent>(npc.Entity);
        state.IsAttacking = true;
        state.AttackTimer = Environment.TickCount;

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
        ref var tint = ref context.World.Get<DamageTintComponent>(victimData.Entity);
        tint.IsHurt = true;
        tint.HurtTimestamp = Environment.TickCount;
    }

    [PacketHandler]
    internal void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        var i = packet.Index;
        var npc = context.CurrentMap.Npc[i];
        npc.Direction = (Direction)packet.Direction;

        ref var movement = ref context.World.Get<MovementComponent>(npc.Entity);
        movement.Direction = (Direction)packet.Direction;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;
    }

    [PacketHandler]
    internal void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var index = packet.Index;
        var npc = context.CurrentMap.Npc[index];

        // Write vital changes directly to ECS.
        ref var vitals = ref context.World.Get<VitalsComponent>(npc.Entity);
        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Current[n] = packet.Vital[n];
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
        context.CurrentMap.Npc[i].Entity = Entity.Null;
    }
}
