using System;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Handlers;

internal static class PlayerHandler
{
    [PacketHandler]
    internal static void PlayerData(PlayerDataPacket packet)
    {
        var name = packet.Name;
        Player player;

        if (name != Player.Me.Name)
        {
            player = new Player(name);
            Player.List.Add(player);
        }
        else
            player = Player.Me;

        player.TextureNum = packet.TextureNum;
        player.Level = packet.Level;
        player.MapInstance = MapInstance.List[packet.MapId];
        player.X = packet.X;
        player.Y = packet.Y;
        player.Direction = (Direction)packet.Direction;
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            player.Vital[n] = packet.Vital[n];
            player.MaxVital[n] = packet.MaxVital[n];
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) player.Attribute[n] = packet.Attribute[n];
        for (byte n = 0; n < (byte)Equipment.Count; n++) player.Equipment[n] = Item.List.Get(packet.Equipment[n]);
        MapInstance.Current = player.MapInstance;
    }

    [PacketHandler]
    internal static void PlayerPosition(PlayerPositionPacket packet)
    {
        var player = Player.Get(packet.Name);

        player.X = packet.X;
        player.Y = packet.Y;
        player.Direction = (Direction)packet.Direction;

        player.X2 = 0;
        player.Y2 = 0;
        player.Movement = Movement.Stopped;
    }

    [PacketHandler]
    internal static void PlayerVitals(PlayerVitalsPacket packet)
    {
        var player = Player.Get(packet.Name);

        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            player.Vital[i] = packet.Vital[i];
            player.MaxVital[i] = packet.MaxVital[i];
        }
    }

    [PacketHandler]
    internal static void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var player = Player.Get(packet.Name);

        // Update player's equipped items
        for (byte i = 0; i < (byte)Equipment.Count; i++) player.Equipment[i] = Item.List.Get(packet.Equipments[i]);
    }

    [PacketHandler]
    internal static void PlayerLeave(PlayerLeavePacket packet)
    {
        // Remove player from list
        Player.List.Remove(Player.Get(packet.Name));
    }

    [PacketHandler]
    internal static void PlayerMove(PlayerMovePacket packet)
    {
        var player = Player.Get(packet.Name);

        player.X = packet.X;
        player.Y = packet.Y;
        player.Direction = (Direction)packet.Direction;
        player.Movement = (Movement)packet.Movement;
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

    [PacketHandler]
    internal static void PlayerDirection(PlayerDirectionPacket packet)
    {
        // Update player's direction
        Player.Get(packet.Name).Direction = (Direction)packet.Direction;
    }

    [PacketHandler]
    internal static void PlayerAttack(PlayerAttackPacket packet)
    {
        var player = Player.Get(packet.Name);
        var victim = packet.Victim;
        var victimType = packet.VictimType;

        player.Attacking = true;
        player.AttackTimer = Environment.TickCount;

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
    internal static void PlayerExperience(PlayerExperiencePacket packet)
    {
        Player.Me.Experience = packet.Experience;
        Player.Me.ExpNeeded = packet.ExpNeeded;
        Player.Me.Points = packet.Points;

        Buttons.AttributesStrength.Visible = Player.Me.Points > 0;
        Buttons.AttributesResistance.Visible = Player.Me.Points > 0;
        Buttons.AttributesIntelligence.Visible = Player.Me.Points > 0;
        Buttons.AttributesAgility.Visible = Player.Me.Points > 0;
        Buttons.AttributesVitality.Visible = Player.Me.Points > 0;
    }

    [PacketHandler]
    internal static void PlayerInventory(PlayerInventoryPacket packet)
    {
        for (byte i = 0; i < MaxInventory; i++)
            Player.Me.Inventory[i] = new ItemSlot(Item.List.Get(packet.ItemIds[i]), packet.Amounts[i]);
    }

    [PacketHandler]
    internal static void PlayerHotbar(PlayerHotbarPacket packet)
    {
        for (byte i = 0; i < MaxHotbar; i++)
        {
            Player.Me.Hotbar[i] = new HotbarSlot((SlotType)packet.Types[i], packet.Slots[i]);
        }
    }
}
