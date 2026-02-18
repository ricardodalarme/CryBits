using System;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Enums;
using LiteNetLib;
using LiteNetLib.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network;

internal static class Send
{
    private static void Packet(NetDataWriter data)
    {
        Socket.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void Connect()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.Connect);
        data.Put(TextBoxes.ConnectUsername.Text);
        data.Put(TextBoxes.ConnectPassword.Text);
        data.Put(false); // Acesso pelo cliente
        Packet(data);
    }

    public static void Register()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.Register);
        data.Put(TextBoxes.RegisterUsername.Text);
        data.Put(TextBoxes.RegisterPassword.Text);
        Packet(data);
    }

    public static void CreateCharacter()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CreateCharacter);
        data.Put(TextBoxes.CreateCharacterName.Text);
        data.Put(Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value.Id.ToString());
        data.Put(CheckBoxes.GenderMale.Checked);
        data.Put(PanelsEvents.CreateCharacterTex);
        Packet(data);
    }

    public static void CharacterUse()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CharacterUse);
        data.Put(PanelsEvents.SelectCharacter);
        Packet(data);
    }

    public static void CharacterCreate()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CharacterCreate);
        Packet(data);
    }

    public static void CharacterDelete()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CharacterDelete);
        data.Put(PanelsEvents.SelectCharacter);
        Packet(data);
    }

    public static void RequestMap(bool order)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestMap);
        data.Put(order);
        Packet(data);
    }

    public static void Latency()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.Latency);
        Packet(data);

        // Define a contaem na hora do envio
        Socket.LatencySend = Environment.TickCount;
    }

    public static void Message(string message, Message type, string addressee = "")
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.Message);
        data.Put(message);
        data.Put((byte)type);
        data.Put(addressee);
        Packet(data);
    }

    public static void AddPoint(Attribute attribute)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.AddPoint);
        data.Put((byte)attribute);
        Packet(data);
    }

    public static void CollectItem()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CollectItem);
        Packet(data);
    }

    public static void DropItem(short slot, short amount)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.DropItem);
        data.Put(slot);
        data.Put(amount);
        Packet(data);
    }

    public static void InventoryChange(short old, short @new)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.InventoryChange);
        data.Put(old);
        data.Put(@new);
        Packet(data);

        // Fecha o painel de soltar item
        Panels.Drop.Visible = false;
    }

    public static void InventoryUse(byte slot)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.InventoryUse);
        data.Put(slot);
        Packet(data);

        // Fecha o painel de soltar item
        Panels.Drop.Visible = false;
    }

    public static void EquipmentRemove(byte slot)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.EquipmentRemove);
        data.Put(slot);
        Packet(data);
    }

    public static void HotbarAdd(short hotbarSlot, byte type, short slot)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.HotbarAdd);
        data.Put(hotbarSlot);
        data.Put(type);
        data.Put(slot);
        Packet(data);
    }

    public static void HotbarChange(short old, short @new)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.HotbarChange);
        data.Put(old);
        data.Put(@new);
        Packet(data);
    }

    public static void HotbarUse(byte slot)
    {
        if (TextBox.Focused == null)
        {
            var data = new NetDataWriter();

            // Envia os dados
            data.Put((byte)ClientPacket.HotbarUse);
            data.Put(slot);
            Packet(data);

            // Fecha o painel de soltar item
            Panels.Drop.Visible = false;
        }
    }

    public static void PartyInvite(string playerName)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyInvite);
        data.Put(playerName);
        Packet(data);
    }

    public static void PartyAccept()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyAccept);
        Packet(data);
    }

    public static void PartyDecline()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyDecline);
        Packet(data);
    }

    public static void PartyLeave()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PartyLeave);
        Packet(data);
    }

    public static void PlayerDirection()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PlayerDirection);
        data.Put((byte)Player.Me.Direction);
        Packet(data);
    }

    public static void PlayerMove()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PlayerMove);
        data.Put(Player.Me.X);
        data.Put(Player.Me.Y);
        data.Put((byte)Player.Me.Movement);
        Packet(data);
    }

    public static void PlayerAttack()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.PlayerAttack);
        Packet(data);
    }

    public static void TradeInvite(string playerName)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.TradeInvite);
        data.Put(playerName);
        Packet(data);
    }

    public static void TradeAccept()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.TradeAccept);
        Packet(data);
    }

    public static void TradeDecline()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.TradeDecline);
        Packet(data);
    }

    public static void TradeLeave()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.TradeLeave);
        Packet(data);
    }

    public static void TradeOffer(short slot, short inventorySlot, short amount = 1)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.TradeOffer);
        data.Put(slot);
        data.Put(inventorySlot);
        data.Put(amount);
        Packet(data);
    }

    public static void TradeOfferState(TradeStatus state)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.TradeOfferState);
        data.Put((byte)state);
        Packet(data);
    }

    public static void ShopBuy(short slot)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.ShopBuy);
        data.Put(slot);
        Packet(data);
    }

    public static void ShopSell(short slot, short amount)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.ShopSell);
        data.Put(slot);
        data.Put(amount);
        Packet(data);
    }

    public static void ShopClose()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.ShopClose);
        Packet(data);
    }
}