using CryBits.Client.Entities;
using CryBits.Client.Interface;
using Lidgren.Network;
using CryBits.Client.Logic;
using System;
using System.Linq;

namespace CryBits.Client.Network
{
    class Send
    {
        // Pacotes do cliente
        public enum Packets
        {
            Connect,
            Latency,
            Register,
            CreateCharacter,
            Character_Use,
            Character_Create,
            Character_Delete,
            Player_Direction,
            Player_Move,
            Player_Attack,
            RequestMap,
            Message,
            AddPoint,
            CollectItem,
            DropItem,
            Inventory_Change,
            Inventory_Use,
            Equipment_Remove,
            Hotbar_Add,
            Hotbar_Change,
            Hotbar_Use,
            Party_Invite,
            Party_Accept,
            Party_Decline,
            Party_Leave,
            Trade_Invite,
            Trade_Accept,
            Trade_Decline,
            Trade_Leave,
            Trade_Offer,
            Trade_Offer_State,
            Shop_Buy,
            Shop_Sell,
            Shop_Close
        }

        private static void Packet(NetOutgoingMessage Data)
        {
            // Envia os dados ao servidor
            Socket.Device.SendMessage(Data, NetDeliveryMethod.ReliableOrdered);
        }

        public static void Connect()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Connect);
            Data.Write(TextBoxes.List["Connect_Username"].Text);
            Data.Write(TextBoxes.List["Connect_Password"].Text);
            Data.Write(false); // Acesso pelo cliente
            Packet(Data);
        }

        public static void Register()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Register);
            Data.Write(TextBoxes.List["Register_Username"].Text);
            Data.Write(TextBoxes.List["Register_Password"].Text);
            Packet(Data);
        }

        public static void CreateCharacter()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.CreateCharacter);
            Data.Write(TextBoxes.List["CreateCharacter_Name"].Text);
            Data.Write(Class.List.ElementAt(Panels.CreateCharacter_Class).Value.ID.ToString());
            Data.Write(CheckBoxes.List["GenderMale"].Checked);
            Data.Write(Panels.CreateCharacter_Tex);
            Packet(Data);
        }

        public static void Character_Use()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Character_Use);
            Data.Write(Panels.SelectCharacter);
            Packet(Data);
        }

        public static void Character_Create()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Character_Create);
            Packet(Data);
        }

        public static void Character_Delete()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Character_Delete);
            Data.Write(Panels.SelectCharacter);
            Packet(Data);
        }

        public static void RequestMap(bool Order)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.RequestMap);
            Data.Write(Order);
            Packet(Data);
        }

        public static void Latency()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Latency);
            Packet(Data);

            // Define a contaem na hora do envio
            Socket.Latency_Send = Environment.TickCount;
        }

        public static void Message(string Message, Messages Type, string Addressee = "")
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Message);
            Data.Write(Message);
            Data.Write((byte)Type);
            Data.Write(Addressee);
            Packet(Data);
        }

        public static void AddPoint(Attributes Attribute)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.AddPoint);
            Data.Write((byte)Attribute);
            Packet(Data);
        }

        public static void CollectItem()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.CollectItem);
            Packet(Data);
        }

        public static void DropItem(byte Slot, short Amount)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.DropItem);
            Data.Write(Slot);
            Data.Write(Amount);
            Packet(Data);
        }

        public static void Inventory_Change(byte Old, byte New)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Inventory_Change);
            Data.Write(Old);
            Data.Write(New);
            Packet(Data);

            // Fecha o painel de soltar item
            Panels.List["Drop"].Visible = false;
        }

        public static void Inventory_Use(byte Slot)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Inventory_Use);
            Data.Write(Slot);
            Packet(Data);

            // Fecha o painel de soltar item
            Panels.List["Drop"].Visible = false;
        }

        public static void Equipment_Remove(byte Slot)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Equipment_Remove);
            Data.Write(Slot);
            Packet(Data);
        }

        public static void Hotbar_Add(short Hotbar_Slot, byte Type, byte Slot)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Hotbar_Add);
            Data.Write(Hotbar_Slot);
            Data.Write(Type);
            Data.Write(Slot);
            Packet(Data);
        }

        public static void Hotbar_Change(short Old, short New)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Hotbar_Change);
            Data.Write(Old);
            Data.Write(New);
            Packet(Data);
        }

        public static void Hotbar_Use(byte Slot)
        {
            if (TextBoxes.Focused == null)
            {
                NetOutgoingMessage Data = Socket.Device.CreateMessage();

                // Envia os dados
                Data.Write((byte)Packets.Hotbar_Use);
                Data.Write(Slot);
                Packet(Data);

                // Fecha o painel de soltar item
                Panels.List["Drop"].Visible = false;
            }
        }

        public static void Party_Invite(string Player_Name)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Party_Invite);
            Data.Write(Player_Name);
            Packet(Data);
        }

        public static void Party_Accept()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Party_Accept);
            Packet(Data);
        }

        public static void Party_Decline()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Party_Decline);
            Packet(Data);
        }

        public static void Party_Leave()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Party_Leave);
            Packet(Data);
        }

        public static void Player_Direction()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Player_Direction);
            Data.Write((byte)Player.Me.Direction);
            Packet(Data);
        }

        public static void Player_Move()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Player_Move);
            Data.Write(Player.Me.X);
            Data.Write(Player.Me.Y);
            Data.Write((byte)Player.Me.Movement);
            Packet(Data);
        }

        public static void Player_Attack()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Player_Attack);
            Packet(Data);
        }

        public static void Trade_Invite(string Player_Name)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Trade_Invite);
            Data.Write(Player_Name);
            Packet(Data);
        }

        public static void Trade_Accept()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Trade_Accept);
            Packet(Data);
        }

        public static void Trade_Decline()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Trade_Decline);
            Packet(Data);
        }

        public static void Trade_Leave()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Trade_Leave);
            Packet(Data);
        }

        public static void Trade_Offer(byte Slot, byte Inventory_Slot, short Amount = 1)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Trade_Offer);
            Data.Write(Slot);
            Data.Write(Inventory_Slot);
            Data.Write(Amount);
            Packet(Data);
        }

        public static void Trade_Offer_State(TradeStatus State)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Trade_Offer_State);
            Data.Write((byte)State);
            Packet(Data);
        }

        public static void Shop_Buy(byte Slot)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Shop_Buy);
            Data.Write(Slot);
            Packet(Data);
        }

        public static void Shop_Sell(byte Slot, short Amount)
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Shop_Sell);
            Data.Write(Slot);
            Data.Write(Amount);
            Packet(Data);
        }

        public static void Shop_Close()
        {
            NetOutgoingMessage Data = Socket.Device.CreateMessage();

            // Envia os dados
            Data.Write((byte)Packets.Shop_Close);
            Packet(Data);
        }
    }
}