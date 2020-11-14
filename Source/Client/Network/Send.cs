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
            data.Write((byte)ClientServer.Connect);
            data.Write(TextBoxes.List["Connect_Username"].Text);
            data.Write(TextBoxes.List["Connect_Password"].Text);
            data.Write(false); // Acesso pelo cliente
            Packet(data);
        }

        public static void Register()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.Register);
            data.Write(TextBoxes.List["Register_Username"].Text);
            data.Write(TextBoxes.List["Register_Password"].Text);
            Packet(data);
        }

        public static void CreateCharacter()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.CreateCharacter);
            data.Write(TextBoxes.List["CreateCharacter_Name"].Text);
            data.Write(Class.List.ElementAt(Panels.CreateCharacterClass).Value.ID.ToString());
            data.Write(CheckBoxes.List["GenderMale"].Checked);
            data.Write(Panels.CreateCharacterTex);
            Packet(data);
        }

        public static void Character_Use()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.CharacterUse);
            data.Write(Panels.SelectCharacter);
            Packet(data);
        }

        public static void Character_Create()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.CharacterCreate);
            Packet(data);
        }

        public static void Character_Delete()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.CharacterDelete);
            data.Write(Panels.SelectCharacter);
            Packet(data);
        }

        public static void RequestMap(bool order)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.RequestMap);
            data.Write(order);
            Packet(data);
        }

        public static void Latency()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.Latency);
            Packet(data);

            // Define a contaem na hora do envio
            Socket.LatencySend = Environment.TickCount;
        }

        public static void Message(string message, Messages type, string addressee = "")
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.Message);
            data.Write(message);
            data.Write((byte)type);
            data.Write(addressee);
            Packet(data);
        }

        public static void AddPoint(Attributes attribute)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.AddPoint);
            data.Write((byte)attribute);
            Packet(data);
        }

        public static void CollectItem()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.CollectItem);
            Packet(data);
        }

        public static void DropItem(byte slot, short amount)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.DropItem);
            data.Write(slot);
            data.Write(amount);
            Packet(data);
        }

        public static void Inventory_Change(byte old, byte @new)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.InventoryChange);
            data.Write(old);
            data.Write(@new);
            Packet(data);

            // Fecha o painel de soltar item
            Panels.List["Drop"].Visible = false;
        }

        public static void Inventory_Use(byte slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.InventoryUse);
            data.Write(slot);
            Packet(data);

            // Fecha o painel de soltar item
            Panels.List["Drop"].Visible = false;
        }

        public static void Equipment_Remove(byte slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.EquipmentRemove);
            data.Write(slot);
            Packet(data);
        }

        public static void Hotbar_Add(short hotbarSlot, byte type, byte slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.HotbarAdd);
            data.Write(hotbarSlot);
            data.Write(type);
            data.Write(slot);
            Packet(data);
        }

        public static void Hotbar_Change(short old, short @new)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.HotbarChange);
            data.Write(old);
            data.Write(@new);
            Packet(data);
        }

        public static void Hotbar_Use(byte slot)
        {
            if (TextBoxes.Focused == null)
            {
                NetOutgoingMessage data = Socket.Device.CreateMessage();

                // Envia os dados
                data.Write((byte)ClientServer.HotbarUse);
                data.Write(slot);
                Packet(data);

                // Fecha o painel de soltar item
                Panels.List["Drop"].Visible = false;
            }
        }

        public static void Party_Invite(string playerName)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PartyInvite);
            data.Write(playerName);
            Packet(data);
        }

        public static void Party_Accept()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PartyAccept);
            Packet(data);
        }

        public static void Party_Decline()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PartyDecline);
            Packet(data);
        }

        public static void Party_Leave()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PartyLeave);
            Packet(data);
        }

        public static void Player_Direction()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PlayerDirection);
            data.Write((byte)Player.Me.Direction);
            Packet(data);
        }

        public static void Player_Move()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PlayerMove);
            data.Write(Player.Me.X);
            data.Write(Player.Me.Y);
            data.Write((byte)Player.Me.Movement);
            Packet(data);
        }

        public static void Player_Attack()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.PlayerAttack);
            Packet(data);
        }

        public static void Trade_Invite(string playerName)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.TradeInvite);
            data.Write(playerName);
            Packet(data);
        }

        public static void Trade_Accept()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.TradeAccept);
            Packet(data);
        }

        public static void Trade_Decline()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.TradeDecline);
            Packet(data);
        }

        public static void Trade_Leave()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.TradeLeave);
            Packet(data);
        }

        public static void Trade_Offer(byte slot, byte inventorySlot, short amount = 1)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.TradeOffer);
            data.Write(slot);
            data.Write(inventorySlot);
            data.Write(amount);
            Packet(data);
        }

        public static void Trade_Offer_State(TradeStatus state)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.TradeOfferState);
            data.Write((byte)state);
            Packet(data);
        }

        public static void Shop_Buy(byte slot)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.ShopBuy);
            data.Write(slot);
            Packet(data);
        }

        public static void Shop_Sell(byte slot, short amount)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.ShopSell);
            data.Write(slot);
            data.Write(amount);
            Packet(data);
        }

        public static void Shop_Close()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientServer.ShopClose);
            Packet(data);
        }
    }
}