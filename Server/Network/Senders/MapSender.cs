using CryBits.Entities.Map;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;

namespace CryBits.Server.Network.Senders;

internal static class MapSender
{
    public static void Map(Account account, Map map)
    {
        Send.ToPlayer(account, new MapPacket { Map = map });
    }

    public static void Maps(Account account)
    {
        Send.ToPlayer(account, new MapsPacket { List = CryBits.Entities.Map.Map.List });
        foreach (var map in CryBits.Entities.Map.Map.List.Values) Map(account, map);
    }

    public static void MapRevision(Player player, Map map)
    {
        Send.ToPlayer(player, new MapRevisionPacket { MapId = map.GetId(), Revision = map.Revision });
    }

    public static void MapPlayers(Player player)
    {
        for (var i = 0; i < Account.List.Count; i++)
            if (Account.List[i].IsPlaying)
                if (player != Account.List[i].Character)
                    if (Account.List[i].Character.Map == player.Map)
                        Send.ToPlayer(player, PlayerDataCache(Account.List[i].Character));
        Send.ToMap(player.Map, PlayerDataCache(player));
    }

    public static void MapItems(Player player, TempMap map)
    {
        var packet = new MapItemsPacket { Items = new PacketsMapItem[map.Item.Count] };
        for (byte i = 0; i < map.Item.Count; i++)
        {
            packet.Items[i] = new PacketsMapItem
            {
                ItemId = map.Item[i].Item.GetId(),
                X = map.Item[i].X,
                Y = map.Item[i].Y
            };
        }

        Send.ToPlayer(player, packet);
    }

    public static void MapItems(TempMap map)
    {
        var packet = new MapItemsPacket { Items = new PacketsMapItem[map.Item.Count] };
        for (byte i = 0; i < map.Item.Count; i++)
        {
            packet.Items[i] = new PacketsMapItem
            {
                ItemId = map.Item[i].Item.GetId(),
                X = map.Item[i].X,
                Y = map.Item[i].Y
            };
        }

        Send.ToMap(map, packet);
    }

    private static PlayerDataPacket PlayerDataCache(Player player)
    {
        var packet = new PlayerDataPacket
        {
            Name = player.Name,
            TextureNum = player.TextureNum,
            Level = player.Level,
            MapId = player.Map.GetId(),
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
