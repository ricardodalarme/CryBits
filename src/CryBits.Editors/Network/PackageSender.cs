using CryBits.Client.Framework.Network;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Editors.Forms;
using CryBits.Transport.Packets.Client;
using LiteNetLib;

namespace CryBits.Editors.Network;

internal class PackageSender(PacketSender packetSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static PackageSender Instance { get; } = new(PacketSender.Instance, DefinitionCatalog.Instance);

    public void Connect() => packetSender.Packet(new ConnectPacket { Username = LoginWindow.Username, Password = LoginWindow.Password, IsClientAccess = true });
    public void RequestClasses() => packetSender.Packet(new RequestClassesPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestMap(Map map) => packetSender.Packet(new RequestMapPacket { Id = map.Id }, DeliveryMethod.ReliableUnordered);
    public void RequestNpcs() => packetSender.Packet(new RequestNpcsPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestItems() => packetSender.Packet(new RequestItemsPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestShops() => packetSender.Packet(new RequestShopsPacket(), DeliveryMethod.ReliableUnordered);
    public void WriteClasses() => packetSender.Packet(new WriteClassesPacket { Classes = _catalog.Classes });
    public void WriteMaps() => packetSender.Packet(new WriteMapsPacket { Maps = _catalog.Maps });
    public void WriteNpcs() => packetSender.Packet(new WriteNpcsPacket { Npcs = _catalog.Npcs });
    public void WriteItems() => packetSender.Packet(new WriteItemsPacket { Items = _catalog.Items });
    public void WriteShops() => packetSender.Packet(new WriteShopsPacket { Shops = _catalog.Shops });
}
