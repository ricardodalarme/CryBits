using Arch.Core;
using CryBits.Client.Components.Combat;
using CryBits.Client.Components.Movement;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using System;
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
        if (context.CurrentMap.Npcs != null)
            foreach (var existing in context.CurrentMap.Npcs)
                if (existing != Entity.Null) context.World.Destroy(existing);

        // Read temporary NPCs for the current map and immediately spawn their entities.
        context.CurrentMap.Npcs = new Entity[packet.Npcs.Length];
        for (byte i = 0; i < context.CurrentMap.Npcs.Length; i++)
        {
            var data = Npc.List.Get(packet.Npcs[i].NpcId);
            var direction = (Direction)packet.Npcs[i].Direction;
            var vitals = new short[(byte)Vital.Count];
            for (byte n = 0; n < (byte)Vital.Count; n++)
                vitals[n] = packet.Npcs[i].Vital[n];

            context.CurrentMap.Npcs[i] = NpcSpawner.Spawn(context.World, data, packet.Npcs[i].X, packet.Npcs[i].Y, direction, vitals);
        }
    }

    [PacketHandler]
    internal void MapNpc(MapNpcPacket packet)
    {
        var i = packet.Index;
        ref var npc = ref context.CurrentMap.Npcs[i];

        if (npc != Entity.Null) context.World.Destroy(npc);

        var data = Npc.List.Get(packet.NpcId);
        var direction = (Direction)packet.Direction;
        var vitals = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) vitals[n] = packet.Vital[n];

        npc = NpcSpawner.Spawn(context.World, data, packet.X, packet.Y, direction, vitals);
    }

    [PacketHandler]
    internal void MapNpcMovement(MapNpcMovementPacket packet)
    {
        var i = packet.Index;
        var npc = context.CurrentMap.Npcs[i];

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
    internal void MapNpcAttack(MapNpcAttackPacket packet)
    {
        var index = packet.Index;
        var victim = packet.Victim;
        var victimType = (Target)packet.VictimType;
        var npc = context.CurrentMap.Npcs[index];

        ref var state = ref context.World.Get<CharacterStateComponent>(npc);
        state.AttackCountdown = AttackSpeed / 1000f;

        if (victim == string.Empty || victimType == Target.None) return;

        var victimEntity = victimType switch
        {
            Target.Player => context.GetPlayerEntity(victim),
            Target.Npc => context.CurrentMap.Npcs[byte.Parse(victim)],
            _ => throw new ArgumentOutOfRangeException()
        };

        var world = context.World;
        ref var victimMovement = ref world.Get<MovementComponent>(victimEntity);
        BloodSplatSpawner.Spawn(world, victimMovement.TileX, victimMovement.TileY);
        ref var tint = ref context.World.Get<DamageComponent>(victimEntity);
        tint.HurtCountdown = DamageComponent.Duration;
    }

    [PacketHandler]
    internal void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        var i = packet.Index;
        var npc = context.CurrentMap.Npcs[i];

        ref var movement = ref context.World.Get<MovementComponent>(npc);
        movement.Direction = (Direction)packet.Direction;
        movement.OffsetX = 0f;
        movement.OffsetY = 0f;
    }

    [PacketHandler]
    internal void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var index = packet.Index;
        var npc = context.CurrentMap.Npcs[index];

        // Write vital changes directly to ECS.
        ref var vitals = ref context.World.Get<VitalsComponent>(npc);
        for (byte n = 0; n < (byte)Vital.Count; n++)
            vitals.Current[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal void MapNpcDied(MapNpcDiedPacket packet)
    {
        var i = packet.Index;

        // Destroy entity
        context.World.Destroy(context.CurrentMap.Npcs[i]);

        // Clear NPC data on death
        context.CurrentMap.Npcs[i] = Entity.Null;
    }
}
