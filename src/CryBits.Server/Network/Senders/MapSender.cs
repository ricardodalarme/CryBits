using CryBits.Entities.Map;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class MapSender(PackageSender packageSender)
{
    public static MapSender Instance { get; } = new(PackageSender.Instance);

    public void Map(GameSession session, Map map)
    {
        packageSender.ToPlayer(session, new MapPacket { Map = map });
    }

    public void Maps(GameSession session)
    {
        packageSender.ToPlayer(session, new MapsPacket { List = CryBits.Entities.Map.Map.List });
        foreach (var map in CryBits.Entities.Map.Map.List.Values) Map(session, map);
    }

    public void MapRevision(Player player, Map map)
    {
        packageSender.ToPlayer(player, new MapRevisionPacket { MapId = map.GetId(), Revision = map.Revision });
    }

    public void MapPlayers(Player player)
    {
        for (var i = 0; i < GameWorld.Current.Sessions.Count; i++)
            if (GameWorld.Current.Sessions[i].IsPlaying)
                if (player != GameWorld.Current.Sessions[i].Character)
                    if (GameWorld.Current.Sessions[i].Character!.MapInstance == player.MapInstance)
                        packageSender.ToPlayer(player, PlayerDataCache(GameWorld.Current.Sessions[i].Character!));
        packageSender.ToMap(player.MapInstance.Id, PlayerDataCache(player));
    }

    public void MapItems(Player player, MapInstance mapInstance)
    {
        var packet = new MapItemsPacket { Items = new PacketsMapItem[mapInstance.Item.Count] };
        for (byte i = 0; i < mapInstance.Item.Count; i++)
        {
            packet.Items[i] = new PacketsMapItem
            {
                ItemId = mapInstance.Item[i].Item.GetId(),
                X = mapInstance.Item[i].X,
                Y = mapInstance.Item[i].Y
            };
        }

        packageSender.ToPlayer(player, packet);
    }

    public void MapItems(MapInstance mapInstance)
    {
        var packet = new MapItemsPacket { Items = new PacketsMapItem[mapInstance.Item.Count] };
        for (byte i = 0; i < mapInstance.Item.Count; i++)
        {
            packet.Items[i] = new PacketsMapItem
            {
                ItemId = mapInstance.Item[i].Item.GetId(),
                X = mapInstance.Item[i].X,
                Y = mapInstance.Item[i].Y
            };
        }

        packageSender.ToMap(mapInstance.Id, packet);
    }

    private static PlayerDataPacket PlayerDataCache(Player player)
    {
        var packet = new PlayerDataPacket
        {
            NetworkId = player.Id,
            Name = player.Name,
            TextureNum = player.TextureNum,
            Level = player.Level,
            MapId = player.MapInstance.GetId(),
            X = player.X,
            Y = player.Y,
            Direction = (byte)player.Direction,
            Vital = new short[(byte)Vital.Count],
            MaxVital = new short[(byte)Vital.Count],
            Attribute = new short[(byte)Attribute.Count],
            Equipment = new System.Guid[(byte)Equipment.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            packet.Vital[n] = player.Vital[n];
            packet.MaxVital[n] = player.MaxVital(n);
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) packet.Attribute[n] = player.Attribute[n];
        for (byte n = 0; n < (byte)Equipment.Count; n++) packet.Equipment[n] = player.Equipment[n].GetId();

        return packet;
    }
}
