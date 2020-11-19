using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Packets;
using Lidgren.Network;
using static CryBits.Defaults;
using static CryBits.Utils;

namespace CryBits.Editors.Network
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
            data.Write(Login.Form.txtUsername.Text);
            data.Write(Login.Form.txtPassword.Text);
            data.Write(true); // Acesso pelo editor
            Packet(data);
        }

        public static void RequestServerData()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteSettings);
            Packet(data);
        }

        public static void RequestClasses()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestClasses);
            Packet(data);
        }

        public static void RequestMap(Map map)
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestMap);
            data.Write(map.ID.ToString());
            Packet(data);
        }

        public static void RequestMaps()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestMaps);
            Packet(data);
        }

        public static void RequestNPCs()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestNPCs);
            Packet(data);
        }

        public static void RequestItems()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestItems);
            Packet(data);
        }

        public static void RequestShops()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.RequestShops);
            Packet(data);
        }

        public static void WriteServerData()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteSettings);
            data.Write(GameName);
            data.Write(WelcomeMessage);
            data.Write(Port);
            data.Write(MaxPlayers);
            data.Write(MaxCharacters);
            data.Write(MaxPartyMembers);
            data.Write(MaxMapItems);
            data.Write(NumPoints);
            data.Write(MinNameLength);
            data.Write(MaxNameLength);
            data.Write(MinPasswordLength);
            data.Write(MaxPasswordLength);
            Packet(data);
        }

        public static void WriteClasses()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteClasses);
            ObjectToByteArray(data, Class.List);
            Packet(data);
        }

        public static void WriteMaps()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteMaps);
            ObjectToByteArray(data, Map.List);
            Packet(data);
        }

        public static void WriteNPCs()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteNPCs);
            ObjectToByteArray(data, NPC.List);
            Packet(data);
        }

        public static void WriteItems()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteItems);
            ObjectToByteArray(data, Item.List);
            Packet(data);
        }

        public static void WriteShops()
        {
            NetOutgoingMessage data = Socket.Device.CreateMessage();

            // Envia os dados
            data.Write((byte)ClientPackets.WriteShops);
            ObjectToByteArray(data, Shop.List);
            Packet(data);
        }
    }
}