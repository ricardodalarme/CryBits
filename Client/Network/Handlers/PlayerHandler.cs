using System;
using CryBits.Client.Entities;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework.Constants;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using LiteNetLib.Utils;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Handlers;

internal static class PlayerHandler
{
    internal static void PlayerData(NetDataReader data)
    {
        var name = data.GetString();
        Player player;

        if (name != Player.Me.Name)
        {
            player = new Player(name);
            Player.List.Add(player);
        }
        else
            player = Player.Me;

        player.TextureNum = data.GetShort();
        player.Level = data.GetShort();
        player.Map = TempMap.List[data.GetGuid()];
        player.X = data.GetByte();
        player.Y = data.GetByte();
        player.Direction = (Direction)data.GetByte();
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            player.Vital[n] = data.GetShort();
            player.MaxVital[n] = data.GetShort();
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) player.Attribute[n] = data.GetShort();
        for (byte n = 0; n < (byte)Equipment.Count; n++) player.Equipment[n] = Item.List.Get(data.GetGuid());
        TempMap.Current = player.Map;
    }

    internal static void PlayerPosition(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        player.X = data.GetByte();
        player.Y = data.GetByte();
        player.Direction = (Direction)data.GetByte();

        player.X2 = 0;
        player.Y2 = 0;
        player.Movement = Movement.Stopped;
    }

    internal static void PlayerVitals(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            player.Vital[i] = data.GetShort();
            player.MaxVital[i] = data.GetShort();
        }
    }

    internal static void PlayerEquipments(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        // Update player's equipped items
        for (byte i = 0; i < (byte)Equipment.Count; i++) player.Equipment[i] = Item.List.Get(data.GetGuid());
    }

    internal static void PlayerLeave(NetDataReader data)
    {
        // Remove player from list
        Player.List.Remove(Player.Get(data.GetString()));
    }

    internal static void PlayerMove(NetDataReader data)
    {
        var player = Player.Get(data.GetString());

        player.X = data.GetByte();
        player.Y = data.GetByte();
        player.Direction = (Direction)data.GetByte();
        player.Movement = (Movement)data.GetByte();
        player.X2 = 0;
        player.Y2 = 0;

        switch (player.Direction)
        {
            case Direction.Up: player.Y2 = Grid; break;
            case Direction.Down: player.Y2 = Grid * -1; break;
            case Direction.Right: player.X2 = Grid * -1; break;
            case Direction.Left: player.X2 = Grid; break;
        }
    }

    internal static void PlayerDirection(NetDataReader data)
    {
        // Update player's direction
        Player.Get(data.GetString()).Direction = (Direction)data.GetByte();
    }

    internal static void PlayerAttack(NetDataReader data)
    {
        var player = Player.Get(data.GetString());
        var victim = data.GetString();
        var victimType = data.GetByte();

        player.Attacking = true;
        player.AttackTimer = Environment.TickCount;

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

    internal static void PlayerExperience(NetDataReader data)
    {
        Player.Me.Experience = data.GetInt();
        Player.Me.ExpNeeded = data.GetInt();
        Player.Me.Points = data.GetByte();

        Buttons.AttributesStrength.Visible = Player.Me.Points > 0;
        Buttons.AttributesResistance.Visible = Player.Me.Points > 0;
        Buttons.AttributesIntelligence.Visible = Player.Me.Points > 0;
        Buttons.AttributesAgility.Visible = Player.Me.Points > 0;
        Buttons.AttributesVitality.Visible = Player.Me.Points > 0;
    }

    internal static void PlayerInventory(NetDataReader data)
    {
        for (byte i = 0; i < MaxInventory; i++)
            Player.Me.Inventory[i] = new ItemSlot(Item.List.Get(data.GetGuid()), data.GetShort());
    }

    internal static void PlayerHotbar(NetDataReader data)
    {
        for (byte i = 0; i < MaxHotbar; i++)
        {
            Player.Me.Hotbar[i] = new HotbarSlot((SlotType)data.GetByte(), data.GetByte());
        }
    }
}
