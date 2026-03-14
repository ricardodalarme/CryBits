using CryBits.Client.Framework.Network;
using CryBits.Editors.Forms;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Entities.Npc;
using CryBits.Entities.Shop;
using CryBits.Packets.Client;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal class PackageSender(PacketSender packetSender)
{
    public static PackageSender Instance { get; } = new(PacketSender.Instance);

    public void Connect() => packetSender.Packet(new ConnectPacket { Username = LoginWindow.Username, Password = LoginWindow.Password, IsClientAccess = true });
    public void RequestServerData() => packetSender.Packet(new WriteSettingsPacket());
    public void RequestClasses() => packetSender.Packet(new RequestClassesPacket());
    public void RequestMap(Map map) => packetSender.Packet(new RequestMapPacket { Id = map.Id });
    public void RequestNpcs() => packetSender.Packet(new RequestNpcsPacket());
    public void RequestItems() => packetSender.Packet(new RequestItemsPacket());
    public void RequestShops() => packetSender.Packet(new RequestShopsPacket());
    public void WriteServerData() => packetSender.Packet(new WriteSettingsPacket { Config = Config });
    public void WriteClasses() => packetSender.Packet(new WriteClassesPacket { Classes = Class.List });
    public void WriteMaps() => packetSender.Packet(new WriteMapsPacket { Maps = Map.List });
    public void WriteNpcs() => packetSender.Packet(new WriteNpcsPacket { Npcs = Npc.List });
    public void WriteItems() => packetSender.Packet(new WriteItemsPacket { Items = Item.List });
    public void WriteShops() => packetSender.Packet(new WriteShopsPacket { Shops = Shop.List });
}
