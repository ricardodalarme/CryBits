using System;
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
            context.CurrentMap.Npc[i].X2 = 0;
            context.CurrentMap.Npc[i].Y2 = 0;
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
        context.CurrentMap.Npc[i].X2 = 0;
        context.CurrentMap.Npc[i].Y2 = 0;
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
        // Read NPC movement
        var i = packet.Index;
        byte x = context.CurrentMap.Npc[i].X, y = context.CurrentMap.Npc[i].Y;
        context.CurrentMap.Npc[i].X2 = 0;
        context.CurrentMap.Npc[i].Y2 = 0;
        context.CurrentMap.Npc[i].X = packet.X;
        context.CurrentMap.Npc[i].Y = packet.Y;
        context.CurrentMap.Npc[i].Direction = (Direction)packet.Direction;
        context.CurrentMap.Npc[i].Movement = (Movement)packet.Movement;

        // Set exact NPC screen offset if position changed
        if (x != context.CurrentMap.Npc[i].X || y != context.CurrentMap.Npc[i].Y)
            switch (context.CurrentMap.Npc[i].Direction)
            {
                case Direction.Up: context.CurrentMap.Npc[i].Y2 = Grid; break;
                case Direction.Down: context.CurrentMap.Npc[i].Y2 = Grid * -1; break;
                case Direction.Right: context.CurrentMap.Npc[i].X2 = Grid * -1; break;
                case Direction.Left: context.CurrentMap.Npc[i].X2 = Grid; break;
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
        // Set NPC direction
        var i = packet.Index;
        context.CurrentMap.Npc[i].Direction = (Direction)packet.Direction;
        context.CurrentMap.Npc[i].X2 = 0;
        context.CurrentMap.Npc[i].Y2 = 0;
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
        context.CurrentMap.Npc[i].X2 = 0;
        context.CurrentMap.Npc[i].Y2 = 0;
        context.CurrentMap.Npc[i].Data = null;
        context.CurrentMap.Npc[i].X = 0;
        context.CurrentMap.Npc[i].Y = 0;
        context.CurrentMap.Npc[i].Vital = new short[(byte)Vital.Count];
        context.CurrentMap.Npc[i].Entity = Arch.Core.Entity.Null;
    }
}
