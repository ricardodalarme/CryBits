using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using Lidgren.Network;
using static CryBits.Globals;
using static CryBits.Utils;

namespace CryBits.Editors.Network;

internal static class Send
{
    private static void Packet(NetOutgoingMessage data)
    {
        // Envia os dados ao servidor
        Socket.Device.SendMessage(data, NetDeliveryMethod.ReliableOrdered);
    }

    public static void Connect()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.Connect);
        data.Write(Login.Form.txtUsername.Text);
        data.Write(Login.Form.txtPassword.Text);
        data.Write(true); // Acesso pelo editor
        Packet(data);
    }

    public static void RequestServerData()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteSettings);
        Packet(data);
    }

    public static void RequestClasses()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.RequestClasses);
        Packet(data);
    }

    public static void RequestMap(Map map)
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.RequestMap);
        data.Write(map.Id.ToString());
        Packet(data);
    }

    public static void RequestNpcs()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.RequestNpcs);
        Packet(data);
    }

    public static void RequestItems()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.RequestItems);
        Packet(data);
    }

    public static void RequestShops()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.RequestShops);
        Packet(data);
    }

    public static void WriteServerData()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteSettings);
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
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteClasses);
        ObjectToByteArray(data, Class.List);
        Packet(data);
    }

    public static void WriteMaps()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteMaps);
        ObjectToByteArray(data, Map.List);
        Packet(data);
    }

    public static void WriteNpcs()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteNpcs);
        ObjectToByteArray(data, Npc.List);
        Packet(data);
    }

    public static void WriteItems()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteItems);
        ObjectToByteArray(data, Item.List);
        Packet(data);
    }

    public static void WriteShops()
    {
        var data = Socket.Device.CreateMessage();

        // Envia os dados
        data.Write((byte)ClientPacket.WriteShops);
        ObjectToByteArray(data, Shop.List);
        Packet(data);
    }
}