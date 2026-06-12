using CryBits.Definitions.Catalog;
using CryBits.Client.Framework.Network;
using CryBits.Editors.Forms;
using CryBits.Definitions.Maps;
using CryBits.Packets.Client;
using LiteNetLib;
using static CryBits.Globals;

namespace CryBits.Editors.Network;

internal class PackageSender(PacketSender packetSender)
{
    public static PackageSender Instance { get; } = new(PacketSender.Instance);

    public void Connect() => packetSender.Packet(new ConnectPacket { Username = LoginWindow.Username, Password = LoginWindow.Password, IsClientAccess = true });
    public void RequestServerData() => packetSender.Packet(new RequestSettingPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestClasses() => packetSender.Packet(new RequestClassesPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestMap(Map map) => packetSender.Packet(new RequestMapPacket { Id = map.Id }, DeliveryMethod.ReliableUnordered);
    public void RequestNpcs() => packetSender.Packet(new RequestNpcsPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestItems() => packetSender.Packet(new RequestItemsPacket(), DeliveryMethod.ReliableUnordered);
    public void RequestShops() => packetSender.Packet(new RequestShopsPacket(), DeliveryMethod.ReliableUnordered);
    public void WriteServerData() => packetSender.Packet(new WriteSettingsPacket { Config = Config });
    public void WriteClasses() => packetSender.Packet(new WriteClassesPacket { Classes = DefinitionCatalog.Classes });
    public void WriteMaps() => packetSender.Packet(new WriteMapsPacket { Maps = DefinitionCatalog.Maps });
    public void WriteNpcs() => packetSender.Packet(new WriteNpcsPacket { Npcs = DefinitionCatalog.Npcs });
    public void WriteItems() => packetSender.Packet(new WriteItemsPacket { Items = DefinitionCatalog.Items });
    public void WriteShops() => packetSender.Packet(new WriteShopsPacket { Shops = DefinitionCatalog.Shops });
}
