using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Extensions;
using CryBits.Packets.Client;
using LiteNetLib;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal static class Send
{
    public static void Packet(IClientPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        Socket.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void Connect() => Packet(new ConnectPacket { Username = LoginWindow.Username, Password = LoginWindow.Password, IsClientAccess = true });
    public static void RequestServerData() => Packet(new WriteSettingsPacket());
    public static void RequestClasses() => Packet(new RequestClassesPacket());
    public static void RequestMap(Map map) => Packet(new RequestMapPacket { Id = map.Id });
    public static void RequestNpcs() => Packet(new RequestNpcsPacket());
    public static void RequestItems() => Packet(new RequestItemsPacket());
    public static void RequestShops() => Packet(new RequestShopsPacket());
    public static void WriteServerData() => Packet(new WriteSettingsPacket { Config = Config });
    public static void WriteClasses() => Packet(new WriteClassesPacket { Classes = Class.List });
    public static void WriteMaps() => Packet(new WriteMapsPacket { Maps = Map.List });
    public static void WriteNpcs() => Packet(new WriteNpcsPacket { Npcs = Npc.List });
    public static void WriteItems() => Packet(new WriteItemsPacket { Items = Item.List });
    public static void WriteShops() => Packet(new WriteShopsPacket { Shops = Shop.List });
}
