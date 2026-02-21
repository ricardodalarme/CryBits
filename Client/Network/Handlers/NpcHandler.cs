using System;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using static CryBits.Globals;
using static CryBits.Utils;

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
        TempMap.Current.Npc = new TempNpc[packet.Npcs.Length];
        for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
        {
            TempMap.Current.Npc[i] = new TempNpc();
            TempMap.Current.Npc[i].X2 = 0;
            TempMap.Current.Npc[i].Y2 = 0;
            TempMap.Current.Npc[i].Data = Npc.List.Get(packet.Npcs[i].NpcId);
            TempMap.Current.Npc[i].X = packet.Npcs[i].X;
            TempMap.Current.Npc[i].Y = packet.Npcs[i].Y;
            TempMap.Current.Npc[i].Direction = (Direction)packet.Npcs[i].Direction;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                TempMap.Current.Npc[i].Vital[n] = packet.Npcs[i].Vital[n];
        }
    }

    [PacketHandler]
    internal static void MapNpc(MapNpcPacket packet)
    {
        // Read temporary NPC data
        var i = packet.Index;
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = Npc.List.Get(packet.NpcId);
        TempMap.Current.Npc[i].X = packet.X;
        TempMap.Current.Npc[i].Y = packet.Y;
        TempMap.Current.Npc[i].Direction = (Direction)packet.Direction;
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) TempMap.Current.Npc[i].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcMovement(MapNpcMovementPacket packet)
    {
        // Read NPC movement
        var i = packet.Index;
        byte x = TempMap.Current.Npc[i].X, y = TempMap.Current.Npc[i].Y;
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].X = packet.X;
        TempMap.Current.Npc[i].Y = packet.Y;
        TempMap.Current.Npc[i].Direction = (Direction)packet.Direction;
        TempMap.Current.Npc[i].Movement = (Movement)packet.Movement;

        // Set exact NPC screen offset if position changed
        if (x != TempMap.Current.Npc[i].X || y != TempMap.Current.Npc[i].Y)
            switch (TempMap.Current.Npc[i].Direction)
            {
                case Direction.Up: TempMap.Current.Npc[i].Y2 = Grid; break;
                case Direction.Down: TempMap.Current.Npc[i].Y2 = Grid * -1; break;
                case Direction.Right: TempMap.Current.Npc[i].X2 = Grid * -1; break;
                case Direction.Left: TempMap.Current.Npc[i].X2 = Grid; break;
            }
    }

    [PacketHandler]
    internal static void MapNpcAttack(MapNpcAttackPacket packet)
    {
        var index = packet.Index;
        var victim = packet.Victim;
        var victimType = packet.VictimType;

        // Start NPC attack
        TempMap.Current.Npc[index].Attacking = true;
        TempMap.Current.Npc[index].AttackTimer = Environment.TickCount;

        // Apply damage to victim
        if (victim != string.Empty)
            if (victimType == (byte)Target.Player)
            {
                var victimData = Player.Get(victim);
                victimData.Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new TempMapBlood((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
            }
            else if (victimType == (byte)Target.Npc)
            {
                TempMap.Current.Npc[byte.Parse(victim)].Hurt = Environment.TickCount;
                TempMap.Current.Blood.Add(new TempMapBlood((byte)MyRandom.Next(0, 3),
                  TempMap.Current.Npc[byte.Parse(victim)].X, TempMap.Current.Npc[byte.Parse(victim)].Y, 255));
            }
    }

    [PacketHandler]
    internal static void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        // Set NPC direction
        var i = packet.Index;
        TempMap.Current.Npc[i].Direction = (Direction)packet.Direction;
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
    }

    [PacketHandler]
    internal static void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var index = packet.Index;

        // Set NPC vitals
        for (byte n = 0; n < (byte)Vital.Count; n++)
            TempMap.Current.Npc[index].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcDied(MapNpcDiedPacket packet)
    {
        var i = packet.Index;

        // Clear NPC data on death
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = null;
        TempMap.Current.Npc[i].X = 0;
        TempMap.Current.Npc[i].Y = 0;
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
    }
}
