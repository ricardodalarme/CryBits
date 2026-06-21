using CryBits.Definitions.Catalog;
using CryBits.Protocol.Packets.Server;
using CryBits.Host.Core;

namespace CryBits.Host.Network.Senders;

internal sealed class ContentSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    public void Map(Session session, Guid mapId)
    {
        var map = catalog.Maps.GetValueOrDefault(mapId);
        if (map != null)
            packageSender.ToPlayer(session, new MapPacket { Map = map });
    }

    public void Maps(Session session)
    {
        packageSender.ToPlayer(session, new MapsPacket { List = catalog.Maps });
        foreach (var map in catalog.Maps.Values) Map(session, map.Id);
    }

    public void MapRevision(Session session, Guid mapId)
    {
        var map = catalog.Maps.GetValueOrDefault(mapId);
        if (map != null)
            packageSender.ToPlayer(session, new MapRevisionPacket { MapId = mapId, Revision = map.Revision });
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
