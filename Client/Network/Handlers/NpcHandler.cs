using System;
using CryBits.Client.Entities;
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
        MapInstance.Current.Npc = new NpcInstance[packet.Npcs.Length];
        for (byte i = 0; i < MapInstance.Current.Npc.Length; i++)
        {
            MapInstance.Current.Npc[i] = new NpcInstance();
            MapInstance.Current.Npc[i].X2 = 0;
            MapInstance.Current.Npc[i].Y2 = 0;
            MapInstance.Current.Npc[i].Data = Npc.List.Get(packet.Npcs[i].NpcId);
            MapInstance.Current.Npc[i].X = packet.Npcs[i].X;
            MapInstance.Current.Npc[i].Y = packet.Npcs[i].Y;
            MapInstance.Current.Npc[i].Direction = (Direction)packet.Npcs[i].Direction;

            for (byte n = 0; n < (byte)Vital.Count; n++)
                MapInstance.Current.Npc[i].Vital[n] = packet.Npcs[i].Vital[n];
        }
    }

    [PacketHandler]
    internal static void MapNpc(MapNpcPacket packet)
    {
        // Read temporary NPC data
        var i = packet.Index;
        MapInstance.Current.Npc[i].X2 = 0;
        MapInstance.Current.Npc[i].Y2 = 0;
        MapInstance.Current.Npc[i].Data = Npc.List.Get(packet.NpcId);
        MapInstance.Current.Npc[i].X = packet.X;
        MapInstance.Current.Npc[i].Y = packet.Y;
        MapInstance.Current.Npc[i].Direction = (Direction)packet.Direction;
        MapInstance.Current.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) MapInstance.Current.Npc[i].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcMovement(MapNpcMovementPacket packet)
    {
        // Read NPC movement
        var i = packet.Index;
        byte x = MapInstance.Current.Npc[i].X, y = MapInstance.Current.Npc[i].Y;
        MapInstance.Current.Npc[i].X2 = 0;
        MapInstance.Current.Npc[i].Y2 = 0;
        MapInstance.Current.Npc[i].X = packet.X;
        MapInstance.Current.Npc[i].Y = packet.Y;
        MapInstance.Current.Npc[i].Direction = (Direction)packet.Direction;
        MapInstance.Current.Npc[i].Movement = (Movement)packet.Movement;

        // Set exact NPC screen offset if position changed
        if (x != MapInstance.Current.Npc[i].X || y != MapInstance.Current.Npc[i].Y)
            switch (MapInstance.Current.Npc[i].Direction)
            {
                case Direction.Up: MapInstance.Current.Npc[i].Y2 = Grid; break;
                case Direction.Down: MapInstance.Current.Npc[i].Y2 = Grid * -1; break;
                case Direction.Right: MapInstance.Current.Npc[i].X2 = Grid * -1; break;
                case Direction.Left: MapInstance.Current.Npc[i].X2 = Grid; break;
            }
    }

    [PacketHandler]
    internal static void MapNpcAttack(MapNpcAttackPacket packet)
    {
        var index = packet.Index;
        var victim = packet.Victim;
        var victimType = packet.VictimType;

        // Start NPC attack
        MapInstance.Current.Npc[index].Attacking = true;
        MapInstance.Current.Npc[index].AttackTimer = Environment.TickCount;

        // Apply damage to victim
        if (victim != string.Empty)
            if (victimType == (byte)Target.Player)
            {
                var victimData = Player.Get(victim);
                victimData.Hurt = Environment.TickCount;
                MapInstance.Current.Blood.Add(new MapBloodInstance((byte)MyRandom.Next(0, 3), victimData.X, victimData.Y, 255));
            }
            else if (victimType == (byte)Target.Npc)
            {
                MapInstance.Current.Npc[byte.Parse(victim)].Hurt = Environment.TickCount;
                MapInstance.Current.Blood.Add(new MapBloodInstance((byte)MyRandom.Next(0, 3),
                  MapInstance.Current.Npc[byte.Parse(victim)].X, MapInstance.Current.Npc[byte.Parse(victim)].Y, 255));
            }
    }

    [PacketHandler]
    internal static void MapNpcDirection(MapNpcDirectionPacket packet)
    {
        // Set NPC direction
        var i = packet.Index;
        MapInstance.Current.Npc[i].Direction = (Direction)packet.Direction;
        MapInstance.Current.Npc[i].X2 = 0;
        MapInstance.Current.Npc[i].Y2 = 0;
    }

    [PacketHandler]
    internal static void MapNpcVitals(MapNpcVitalsPacket packet)
    {
        var index = packet.Index;

        // Set NPC vitals
        for (byte n = 0; n < (byte)Vital.Count; n++)
            MapInstance.Current.Npc[index].Vital[n] = packet.Vital[n];
    }

    [PacketHandler]
    internal static void MapNpcDied(MapNpcDiedPacket packet)
    {
        var i = packet.Index;

        // Clear NPC data on death
        MapInstance.Current.Npc[i].X2 = 0;
        MapInstance.Current.Npc[i].Y2 = 0;
        MapInstance.Current.Npc[i].Data = null;
        MapInstance.Current.Npc[i].X = 0;
        MapInstance.Current.Npc[i].Y = 0;
        MapInstance.Current.Npc[i].Vital = new short[(byte)Vital.Count];
    }
}
