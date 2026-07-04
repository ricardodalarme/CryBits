using CryBits.Definitions.Catalog;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Editors.Network.Handlers;

internal class ContentHandler(DefinitionCatalog catalog)
{
    [PacketHandler]
    internal void Classes(ClassesPacket packet)
    {
        catalog.Classes = packet.List;
    }

    [PacketHandler]
    internal void Items(ItemsPacket packet)
    {
        catalog.Items = packet.List;
    }

    [PacketHandler]
    internal void Maps(MapsPacket packet)
    {
        catalog.Maps.Clear();
        foreach (var (id, map) in packet.List)
            catalog.Maps[id] = map;
    }

    [PacketHandler]
    internal void Map(MapPacket packet)
    {
        var map = packet.Map;
        catalog.Maps[map.Id] = map;
    }

    [PacketHandler]
    internal void Npcs(NpcsPacket packet)
    {
        catalog.Npcs = packet.List;
    }

    [PacketHandler]
    internal void Shops(ShopsPacket packet)
    {
        catalog.Shops = packet.List;
    }
}
