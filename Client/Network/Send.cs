using System;
using System.Linq;
using CryBits.Client.Entities;
using CryBits.Client.UI;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;

namespace CryBits.Client.Network
{
    internal static class Send
    {
        private static void Packet(NetOutgoingMessage data)
        {
            // Envia os dados ao servidor
            Socket.Device.SendMessage(data, NetDeliveryMethod.ReliableOrdered);
        }

        public static void Connect()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.Connect);
            data.Write(TextBoxes.List["Connect_Username"].Text);
            data.Write(TextBoxes.List["Connect_Password"].Text);
            data.Write(false); // Acesso pelo cliente
            Packet(data);
        }

        public static void Register()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.Register);
            data.Write(TextBoxes.List["Register_Username"].Text);
            data.Write(TextBoxes.List["Register_Password"].Text);
            Packet(data);
        }

        public static void CreateCharacter()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.CreateCharacter);
            data.Write(TextBoxes.List["CreateCharacter_Name"].Text);
            data.Write(Class.List.ElementAt(Panels.CreateCharacterClass).Value.ID.ToString());
            data.Write(CheckBoxes.List["GenderMale"].Checked);
            data.Write(Panels.CreateCharacterTex);
            Packet(data);
        }

        public static void CharacterUse()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.CharacterUse);
            data.Write(Panels.SelectCharacter);
            Packet(data);
        }

        public static void CharacterCreate()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.CharacterCreate);
            Packet(data);
        }

        public static void CharacterDelete()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.CharacterDelete);
            data.Write(Panels.SelectCharacter);
            Packet(data);
        }

        public static void RequestMap(bool order)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestMap);
            data.Write(order);
            Packet(data);
        }

        public static void Latency()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.Latency);
            Packet(data);

            // Define a contaem na hora do envio
            Socket.LatencySend = Environment.TickCount;
        }

        public static void Message(string message, Messages type, string addressee = "")
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.Message);
            data.Write(message);
            data.Write((byte)type);
            data.Write(addressee);
            Packet(data);
        }

        public static void AddPoint(Attributes attribute)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.AddPoint);
            data.Write((byte)attribute);
            Packet(data);
        }

        public static void CollectItem()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.CollectItem);
            Packet(data);
        }

        public static void DropItem(short slot, short amount)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.DropItem);
            data.Write(slot);
            data.Write(amount);
            Packet(data);
        }

        public static void InventoryChange(short old, short @new)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.InventoryChange);
            data.Write(old);
            data.Write(@new);
            Packet(data);

            // Fecha o painel de soltar item
            Panels.List["Drop"].Visible = false;
        }

        public static void InventoryUse(byte slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.InventoryUse);
            data.Write(slot);
            Packet(data);

            // Fecha o painel de soltar item
            Panels.List["Drop"].Visible = false;
        }

        public static void EquipmentRemove(byte slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.EquipmentRemove);
            data.Write(slot);
            Packet(data);
        }

        public static void HotbarAdd(short hotbarSlot, byte type, short slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.HotbarAdd);
            data.Write(hotbarSlot);
            data.Write(type);
            data.Write(slot);
            Packet(data);
        }

        public static void HotbarChange(short old, short @new)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.HotbarChange);
            data.Write(old);
            data.Write(@new);
            Packet(data);
        }

        public static void HotbarUse(byte slot)
        {
            if (TextBoxes.Focused == null)
            {
                NetOutgoingMessage data = Socket.Device.CreateMessage();

                // Envia os dados
                data.Write((byte)ClientPackets.HotbarUse);
                data.Write(slot);
                Packet(data);

                // Fecha o painel de soltar item
                Panels.List["Drop"].Visible = false;
            }
        }

        public static void PartyInvite(string playerName)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PartyInvite);
            data.Write(playerName);
            Packet(data);
        }

        public static void PartyAccept()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PartyAccept);
            Packet(data);
        }

        public static void PartyDecline()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PartyDecline);
            Packet(data);
        }

        public static void PartyLeave()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PartyLeave);
            Packet(data);
        }

        public static void PlayerDirection()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PlayerDirection);
            data.Write((byte)Player.Me.Direction);
            Packet(data);
        }

        public static void PlayerMove()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PlayerMove);
            data.Write(Player.Me.X);
            data.Write(Player.Me.Y);
            data.Write((byte)Player.Me.Movement);
            Packet(data);
        }

        public static void PlayerAttack()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.PlayerAttack);
            Packet(data);
        }

        public static void TradeInvite(string playerName)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.TradeInvite);
            data.Write(playerName);
            Packet(data);
        }

        public static void TradeAccept()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.TradeAccept);
            Packet(data);
        }

        public static void TradeDecline()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.TradeDecline);
            Packet(data);
        }

        public static void TradeLeave()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.TradeLeave);
            Packet(data);
        }

        public static void TradeOffer(short slot, short inventorySlot, short amount = 1)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.TradeOffer);
            data.Write(slot);
            data.Write(inventorySlot);
            data.Write(amount);
            Packet(data);
        }

        public static void TradeOfferState(TradeStatus state)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.TradeOfferState);
            data.Write((byte)state);
            Packet(data);
        }

        public static void ShopBuy(short slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.ShopBuy);
            data.Write(slot);
            Packet(data);
        }

        public static void ShopSell(short slot, short amount)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.ShopSell);
            data.Write(slot);
            data.Write(amount);
            Packet(data);
        }

        public static void ShopClose()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.ShopClose);
            Packet(data);
        }
    }
}