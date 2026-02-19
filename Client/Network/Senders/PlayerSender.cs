using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Enums;
using LiteNetLib.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Senders;

internal static class PlayerSender
{
    public static void PlayerDirection()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.PlayerDirection);
        data.Put((byte)Player.Me.Direction);
        Send.Packet(data);
    }

    public static void PlayerMove()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.PlayerMove);
        data.Put(Player.Me.X);
        data.Put(Player.Me.Y);
        data.Put((byte)Player.Me.Movement);
        Send.Packet(data);
    }

    public static void PlayerAttack()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.PlayerAttack);
        Send.Packet(data);
    }

    public static void AddPoint(Attribute attribute)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.AddPoint);
        data.Put((byte)attribute);
        Send.Packet(data);
    }

    public static void CollectItem()
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.CollectItem);
        Send.Packet(data);
    }

    public static void DropItem(short slot, short amount)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.DropItem);
        data.Put(slot);
        data.Put(amount);
        Send.Packet(data);
    }

    public static void InventoryChange(short old, short @new)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.InventoryChange);
        data.Put(old);
        data.Put(@new);
        Send.Packet(data);

        // Close drop panel
        Panels.Drop.Visible = false;
    }

    public static void InventoryUse(byte slot)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.InventoryUse);
        data.Put(slot);
        Send.Packet(data);

        // Close drop panel
        Panels.Drop.Visible = false;
    }

    public static void EquipmentRemove(byte slot)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.EquipmentRemove);
        data.Put(slot);
        Send.Packet(data);
    }

    public static void HotbarAdd(short hotbarSlot, byte type, short slot)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.HotbarAdd);
        data.Put(hotbarSlot);
        data.Put(type);
        data.Put(slot);
        Send.Packet(data);
    }

    public static void HotbarChange(short old, short @new)
    {
        var data = new NetDataWriter();

        data.Put((byte)ClientPacket.HotbarChange);
        data.Put(old);
        data.Put(@new);
        Send.Packet(data);
    }

    public static void HotbarUse(byte slot)
    {
        if (TextBox.Focused == null)
        {
            var data = new NetDataWriter();

            // Send data.
            data.Put((byte)ClientPacket.HotbarUse);
            data.Put(slot);
            Send.Packet(data);

            // Close drop panel
            Panels.Drop.Visible = false;
        }
    }
}
