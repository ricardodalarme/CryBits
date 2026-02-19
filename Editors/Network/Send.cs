using CryBits.Editors.AvaloniaUI;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Globals;
using static CryBits.Extensions.NetworkExtensions;

namespace CryBits.Editors.Network;

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
        data.Put(AvaloniaLoginLauncher.Username);
        data.Put(AvaloniaLoginLauncher.Password);
        data.Put(true); // Acesso pelo editor
        Packet(data);
    }

    public static void RequestServerData()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteSettings);
        Packet(data);
    }

    public static void RequestClasses()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestClasses);
        Packet(data);
    }

    public static void RequestMap(Map map)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestMap);
        data.Put(map.Id.ToString());
        Packet(data);
    }

    public static void RequestNpcs()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestNpcs);
        Packet(data);
    }

    public static void RequestItems()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestItems);
        Packet(data);
    }

    public static void RequestShops()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.RequestShops);
        Packet(data);
    }

    public static void WriteServerData()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteSettings);
        data.WriteObject(Config);
        Packet(data);
    }

    public static void WriteClasses()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteClasses);
        data.WriteObject(Class.List);
        Packet(data);
    }

    public static void WriteMaps()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteMaps);
        data.WriteObject(Map.List);
        Packet(data);
    }

    public static void WriteNpcs()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteNpcs);
        data.WriteObject(Npc.List);
        Packet(data);
    }

    public static void WriteItems()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteItems);
        data.WriteObject(Item.List);
        Packet(data);
    }

    public static void WriteShops()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.WriteShops);
        data.WriteObject(Shop.List);
        Packet(data);
    }
}