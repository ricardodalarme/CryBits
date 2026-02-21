using CryBits.Editors.AvaloniaUI;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Enums;
using CryBits.Packets.Client;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Extensions.NetworkExtensions;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal static class Send
{
    public static void Packet(ClientPacket packetId, IClientPacket packet)
    {
        var data = new NetDataWriter();
        data.Put((byte)packetId);
        data.WriteObject(packet);
        Socket.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void Connect() => Packet(ClientPacket.Connect, new ConnectPacket { Username = AvaloniaLoginLauncher.Username, Password = AvaloniaLoginLauncher.Password, IsClientAccess = true });

    public static void RequestServerData() => Packet(ClientPacket.WriteSettings, new WriteSettingsPacket());

    public static void RequestClasses() => Packet(ClientPacket.RequestClasses, new RequestClassesPacket());

    public static void RequestMap(Map map) => Packet(ClientPacket.RequestMap, new RequestMapPacket { Id = map.Id });

    public static void RequestNpcs() => Packet(ClientPacket.RequestNpcs, new RequestNpcsPacket());

    public static void RequestItems() => Packet(ClientPacket.RequestItems, new RequestItemsPacket());

    public static void RequestShops() => Packet(ClientPacket.RequestShops, new RequestShopsPacket());

    public static void WriteServerData() => Packet(ClientPacket.WriteSettings, new WriteSettingsPacket { Config = Config });

    public static void WriteClasses() => Packet(ClientPacket.WriteClasses, new WriteClassesPacket { Classes = Class.List });

    public static void WriteMaps() => Packet(ClientPacket.WriteMaps, new WriteMapsPacket { Maps = Map.List });

    public static void WriteNpcs() => Packet(ClientPacket.WriteNpcs, new WriteNpcsPacket { Npcs = Npc.List });

    public static void WriteItems() => Packet(ClientPacket.WriteItems, new WriteItemsPacket { Items = Item.List });

    public static void WriteShops() => Packet(ClientPacket.WriteShops, new WriteShopsPacket { Shops = Shop.List });
}
