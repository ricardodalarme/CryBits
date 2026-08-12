using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using CryBits.Host.Core;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Host.Network.Senders;

internal sealed class ContentSender(PackageSender packageSender)
{
    public void Map(Session session, Map map, bool editor = false)
    {
        packageSender.ToPlayer(session,
            new MapPacket { Map = editor ? map : map with { Chunks = [] } });
    }

    public void Maps(Session session, Dictionary<Guid, Map> maps, bool editor = false)
    {
        var result = new Dictionary<Guid, Map>(maps.Count);
        foreach (var (id, map) in maps)
            result[id] = editor ? map : map with { Chunks = [] };

        packageSender.ToPlayer(session, new MapsPacket { List = result });
    }

    public void MapRevision(Session session, Guid mapId)
    {
        packageSender.ToPlayer(session, new MapRevisionPacket { MapId = mapId });
    }

    public void Classes(Session session, Dictionary<Guid, Class> classes)
    {
        packageSender.ToPlayer(session, new ClassesPacket { List = classes });
    }

    public void Items(Session session, Dictionary<Guid, Item> items)
    {
        packageSender.ToPlayer(session, new ItemsPacket { List = items });
    }

    public void Npcs(Session session, Dictionary<Guid, Npc> npcs)
    {
        packageSender.ToPlayer(session, new NpcsPacket { List = npcs });
    }

    public void Shops(Session session, Dictionary<Guid, Shop> shops)
    {
        packageSender.ToPlayer(session, new ShopsPacket { List = shops });
    }
}
