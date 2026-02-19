using System;
using System.Collections.Generic;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using LiteNetLib.Utils;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Client.Network.Handlers;

internal static class NpcHandler
{
    internal static void Npcs(NetDataReader data)
    {
        // Read NPCs dictionary
        Npc.List = (Dictionary<Guid, Npc>)data.ReadObject();
    }

    internal static void MapNpcs(NetDataReader data)
    {
        // Read temporary NPCs for the current map
        TempMap.Current.Npc = new TempNpc[data.GetShort()];
        for (byte i = 0; i < TempMap.Current.Npc.Length; i++)
        {
            TempMap.Current.Npc[i] = new TempNpc();
            TempMap.Current.Npc[i].X2 = 0;
            TempMap.Current.Npc[i].Y2 = 0;
            TempMap.Current.Npc[i].Data = Npc.List.Get(data.GetGuid());
            TempMap.Current.Npc[i].X = data.GetByte();
            TempMap.Current.Npc[i].Y = data.GetByte();
            TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();


            for (byte n = 0; n < (byte)Vital.Count; n++)
                TempMap.Current.Npc[i].Vital[n] = data.GetShort();
        }
    }

    internal static void MapNpc(NetDataReader data)
    {
        // Read temporary NPC data
        var i = data.GetByte();
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = Npc.List.Get(data.GetGuid());
        TempMap.Current.Npc[i].X = data.GetByte();
        TempMap.Current.Npc[i].Y = data.GetByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
        for (byte n = 0; n < (byte)Vital.Count; n++) TempMap.Current.Npc[i].Vital[n] = data.GetShort();
    }

    internal static void MapNpcMovement(NetDataReader data)
    {
        // Read NPC movement
        var i = data.GetByte();
        byte x = TempMap.Current.Npc[i].X, y = TempMap.Current.Npc[i].Y;
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].X = data.GetByte();
        TempMap.Current.Npc[i].Y = data.GetByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();
        TempMap.Current.Npc[i].Movement = (Movement)data.GetByte();

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

    internal static void MapNpcAttack(NetDataReader data)
    {
        var index = data.GetByte();
        var victim = data.GetString();
        var victimType = data.GetByte();

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

    internal static void MapNpcDirection(NetDataReader data)
    {
        // Set NPC direction
        var i = data.GetByte();
        TempMap.Current.Npc[i].Direction = (Direction)data.GetByte();
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
    }

    internal static void MapNpcVitals(NetDataReader data)
    {
        var index = data.GetByte();

        // Set NPC vitals
        for (byte n = 0; n < (byte)Vital.Count; n++)
            TempMap.Current.Npc[index].Vital[n] = data.GetShort();
    }

    internal static void MapNpcDied(NetDataReader data)
    {
        var i = data.GetByte();

        // Clear NPC data on death
        TempMap.Current.Npc[i].X2 = 0;
        TempMap.Current.Npc[i].Y2 = 0;
        TempMap.Current.Npc[i].Data = null;
        TempMap.Current.Npc[i].X = 0;
        TempMap.Current.Npc[i].Y = 0;
        TempMap.Current.Npc[i].Vital = new short[(byte)Vital.Count];
    }
}
