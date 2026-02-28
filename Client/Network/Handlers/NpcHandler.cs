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

internal static class NpcHandler
{
    [PacketHandler]
    internal static void Npcs(NpcsPacket packet)
    {
        // Read NPCs dictionary
        Npc.List = packet.List;
    }

    [PacketHandler]
    internal static void MapNpcs(MapNpcsPacket packet)
    {
        // Read temporary NPCs for the current map
        GameContext.Instance.CurrentMap.Npc = new NpcInstance[packet.Npcs.Length];
        for (byte i = 0; i < GameContext.Instance.CurrentMap.Npc.Length; i++)
        {
            GameContext.Instance.CurrentMap.Npc[i] = new NpcInstance();
            GameContext.Instance.CurrentMap.Npc[i].X2 = 0;
            GameContext.Instance.CurrentMap.Npc[i].Y2 = 0;
            GameContext.Instance.CurrentMap.Npc[i].Data = Npc.List.Get(packet.Npcs[i].NpcId);
            GameContext.Instance.CurrentMap.Npc[i].X = packet.Npcs[i].X;
            GameContext.Instance.CurrentMap.Npc[i].Y = packet.Npcs[i].Y;
            GameContext.Instance.CurrentMap.Npc[i].Direction = (Direction)packet.Npcs[i].Direction;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                GameContext.Instance.CurrentMap.Npc[i].Vital[n] = packet.Npcs[i].Vital[n];
        }
    }

    [PacketHandler]
    internal static void MapNpc(MapNpcPacket packet)
    {
        var i = packet.Index;
        GameContext.Instance.CurrentMap.Npc[i].X2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].Y2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].Data = Npc.List.Get(packet.NpcId);
        GameContext.Instance.CurrentMap.Npc[i].X = packet.X;
        GameContext.Instance.CurrentMap.Npc[i].Y = packet.Y;
        GameContext.Instance.CurrentMap.Npc[i].Direction = (Direction)packet.Direction;
        GameContext.Instance.CurrentMap.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) GameContext.Instance.CurrentMap.Npc[i].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcMovement(MapNpcMovementPacket packet)
    {
        // Read NPC movement
        var i = packet.Index;
        byte x = GameContext.Instance.CurrentMap.Npc[i].X, y = GameContext.Instance.CurrentMap.Npc[i].Y;
        GameContext.Instance.CurrentMap.Npc[i].X2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].Y2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].X = packet.X;
        GameContext.Instance.CurrentMap.Npc[i].Y = packet.Y;
        GameContext.Instance.CurrentMap.Npc[i].Direction = (Direction)packet.Direction;
        GameContext.Instance.CurrentMap.Npc[i].Movement = (Movement)packet.Movement;

        // Set exact NPC screen offset if position changed
        if (x != GameContext.Instance.CurrentMap.Npc[i].X || y != GameContext.Instance.CurrentMap.Npc[i].Y)
            switch (GameContext.Instance.CurrentMap.Npc[i].Direction)
            {
                case Direction.Up: GameContext.Instance.CurrentMap.Npc[i].Y2 = Grid; break;
                case Direction.Down: GameContext.Instance.CurrentMap.Npc[i].Y2 = Grid * -1; break;
                case Direction.Right: GameContext.Instance.CurrentMap.Npc[i].X2 = Grid * -1; break;
                case Direction.Left: GameContext.Instance.CurrentMap.Npc[i].X2 = Grid; break;
            }
    }

    [PacketHandler]
    internal static void MapNpcAttack(MapNpcAttackPacket packet)
    {
        var index = packet.Index;
        var victim = packet.Victim;
        var victimType = (Target)packet.VictimType;

        // Start NPC attack
        GameContext.Instance.CurrentMap.Npc[index].Attacking = true;
        GameContext.Instance.CurrentMap.Npc[index].AttackTimer = Environment.TickCount;

        if (victim == string.Empty || victimType == Target.None) return;

        Character victimData = victimType switch
        {
            Target.Player => Player.Get(victim),
            Target.Npc => GameContext.Instance.CurrentMap.Npc[byte.Parse(victim)],
            _ => throw new ArgumentOutOfRangeException()
        };

        // Apply damage to victim
        var world = GameContext.Instance.World;
        BloodSplatSpawner.Spawn(world, victimData.X, victimData.Y);
        victimData.Hurt = Environment.TickCount;
    }

    [PacketHandler]
    internal static void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        // Set NPC direction
        var i = packet.Index;
        GameContext.Instance.CurrentMap.Npc[i].Direction = (Direction)packet.Direction;
        GameContext.Instance.CurrentMap.Npc[i].X2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].Y2 = 0;
    }

    [PacketHandler]
    internal static void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var index = packet.Index;

        // Set NPC vitals
        for (byte n = 0; n < (byte)Vital.Count; n++)
            GameContext.Instance.CurrentMap.Npc[index].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcDied(MapNpcDiedPacket packet)
    {
        var i = packet.Index;

        // Destroy entity
        GameContext.Instance.World.Destroy(GameContext.Instance.CurrentMap.Npc[i].Entity);

        // Clear NPC data on death
        GameContext.Instance.CurrentMap.Npc[i].X2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].Y2 = 0;
        GameContext.Instance.CurrentMap.Npc[i].Data = null;
        GameContext.Instance.CurrentMap.Npc[i].X = 0;
        GameContext.Instance.CurrentMap.Npc[i].Y = 0;
        GameContext.Instance.CurrentMap.Npc[i].Vital = new short[(byte)Vital.Count];
        GameContext.Instance.CurrentMap.Npc[i].Entity = Arch.Core.Entity.Null;
    }
}
