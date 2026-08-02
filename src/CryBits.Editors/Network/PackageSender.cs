using CryBits.Client.Framework.Network;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Editors.Network;

internal class PackageSender(Connection connection, DefinitionCatalog catalog)
{
    public static PackageSender? Instance { get; set; }

    public void Connect(string username, string password) =>
        connection.SendPacket(new ConnectPacket { Username = username, Password = password, IsClientAccess = true });

    public void RequestClasses() =>
        connection.SendPacket(new RequestClassesPacket(), DeliveryChannel.ReliableUnordered);

    public void RequestMap(Map map) =>
        connection.SendPacket(new RequestMapPacket { Id = map.Id }, DeliveryChannel.ReliableUnordered);

    public void RequestNpcs() => connection.SendPacket(new RequestNpcsPacket(), DeliveryChannel.ReliableUnordered);
    public void RequestItems() => connection.SendPacket(new RequestItemsPacket(), DeliveryChannel.ReliableUnordered);
    public void RequestShops() => connection.SendPacket(new RequestShopsPacket(), DeliveryChannel.ReliableUnordered);
    public void WriteClasses() => connection.SendPacket(new WriteClassesPacket { Classes = catalog.Classes });
    public void WriteMaps() => connection.SendPacket(new WriteMapsPacket { Maps = catalog.Maps });
    public void WriteNpcs() => connection.SendPacket(new WriteNpcsPacket { Npcs = catalog.Npcs });
    public void WriteItems() => connection.SendPacket(new WriteItemsPacket { Items = catalog.Items });
    public void WriteShops() => connection.SendPacket(new WriteShopsPacket { Shops = catalog.Shops });
}
