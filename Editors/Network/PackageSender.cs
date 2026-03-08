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

internal class PackageSender
{
    public static PackageSender Instance { get; } = new(NetworkClient.Instance);

    private readonly NetworkClient _client;

    public PackageSender(NetworkClient client)
    {
        _client = client;
    }

    public void Packet(IClientPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        _client.ServerPeer?.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public void Connect() => Packet(new ConnectPacket { Username = LoginWindow.Username, Password = LoginWindow.Password, IsClientAccess = true });
    public void RequestServerData() => Packet(new WriteSettingsPacket());
    public void RequestClasses() => Packet(new RequestClassesPacket());
    public void RequestMap(Map map) => Packet(new RequestMapPacket { Id = map.Id });
    public void RequestNpcs() => Packet(new RequestNpcsPacket());
    public void RequestItems() => Packet(new RequestItemsPacket());
    public void RequestShops() => Packet(new RequestShopsPacket());
    public void WriteServerData() => Packet(new WriteSettingsPacket { Config = Config });
    public void WriteClasses() => Packet(new WriteClassesPacket { Classes = Class.List });
    public void WriteMaps() => Packet(new WriteMapsPacket { Maps = Map.List });
    public void WriteNpcs() => Packet(new WriteNpcsPacket { Npcs = Npc.List });
    public void WriteItems() => Packet(new WriteItemsPacket { Items = Item.List });
    public void WriteShops() => Packet(new WriteShopsPacket { Shops = Shop.List });
}
