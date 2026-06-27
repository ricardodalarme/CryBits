using CryBits.Definitions.Catalog;
using CryBits.Definitions.Maps;
using CryBits.Host.Core;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Host.Network.Senders;

internal sealed class ContentSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    public void Map(Session session, Guid mapId)
    {
        var map = catalog.Maps.GetValueOrDefault(mapId);
        if (map != null)
            packageSender.ToPlayer(session, new MapPacket { Map = session.InEditor ? map : map with { Chunks = [] } });
    }

    public void Maps(Session session)
    {
        var result = new Dictionary<Guid, Map>(catalog.Maps.Count);
        foreach (var (id, map) in catalog.Maps)
            result[id] = session.InEditor ? map : map with { Chunks = [] };

        packageSender.ToPlayer(session, new MapsPacket { List = result });
    }

    public void MapRevision(Session session, Guid mapId)
    {
        packageSender.ToPlayer(session, new MapRevisionPacket { MapId = mapId });
    }

    public void Classes(Session session)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = catalog.Classes });
    }

    public void Items(Session session)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = catalog.Items });
    }

    public void Npcs(Session session)
    {
        packageSender.ToPlayer(session, new NpcsPacket { List = catalog.Npcs });
    }

    public void Shops(Session session)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = catalog.Shops });
    }
}
