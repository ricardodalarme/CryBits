using CryBits.Client.Framework.Network;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Protocol.Packets.Client;
using CryBits.Transport;

namespace CryBits.Editors.Network;

internal class PackageSender(Connection connection)
{
    public void Connect(string username, string password) =>
        connection.SendPacket(new ConnectPacket { Username = username, Password = password, IsClientAccess = true });

    public void RequestClasses() =>
        connection.SendPacket(new RequestClassesPacket(), DeliveryChannel.ReliableUnordered);

    public void RequestMap(Map map) =>
        connection.SendPacket(new RequestMapPacket { Id = map.Id }, DeliveryChannel.ReliableUnordered);

    public void RequestNpcs() => connection.SendPacket(new RequestNpcsPacket(), DeliveryChannel.ReliableUnordered);
    public void RequestItems() => connection.SendPacket(new RequestItemsPacket(), DeliveryChannel.ReliableUnordered);
    public void RequestShops() => connection.SendPacket(new RequestShopsPacket(), DeliveryChannel.ReliableUnordered);
    public void WriteClasses(Dictionary<Guid, Class> classes) =>
        connection.SendPacket(new WriteClassesPacket { Classes = classes });
    public void WriteMaps(Dictionary<Guid, Map> maps) =>
        connection.SendPacket(new WriteMapsPacket { Maps = maps });
    public void WriteNpcs(Dictionary<Guid, Npc> npcs) =>
        connection.SendPacket(new WriteNpcsPacket { Npcs = npcs });
    public void WriteItems(Dictionary<Guid, Item> items) =>
        connection.SendPacket(new WriteItemsPacket { Items = items });
    public void WriteShops(Dictionary<Guid, Shop> shops) =>
        connection.SendPacket(new WriteShopsPacket { Shops = shops });
}
