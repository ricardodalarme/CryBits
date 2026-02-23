using CryBits.Entities.Map;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal static class MapSender
{
    private static CryBits.Server.ECS.World World => ServerContext.Instance.World;

    public static void Map(GameSession session, Map map)
    {
        Send.ToPlayer(session, new MapPacket { Map = map });
    }

    public static void Maps(GameSession session)
    {
        Send.ToPlayer(session, new MapsPacket { List = CryBits.Entities.Map.Map.List });
        foreach (var map in CryBits.Entities.Map.Map.List.Values) Map(session, map);
    }

    public static void MapRevision(Player player, Map map)
    {
        Send.ToPlayer(player, new MapRevisionPacket { MapId = map.GetId(), Revision = map.Revision });
    }

    public static void MapPlayers(Player player)
    {
        var playerPos = player.Get<PositionComponent>();
        for (var i = 0; i < GameWorld.Current.Sessions.Count; i++)
        {
            var other = GameWorld.Current.Sessions[i].Character;
            if (!GameWorld.Current.Sessions[i].IsPlaying || other == null || other == player) continue;
            var otherPos = World.Get<PositionComponent>(other.EntityId);
            if (otherPos.MapId == playerPos.MapId)
                Send.ToPlayer(player, PlayerDataCache(other));
        }

        Send.ToMap(player.MapInstance, PlayerDataCache(player));
    }

    public static void MapItems(Player player, MapInstance mapInstance)
    {
        var items = ServerContext.Instance.GetMapItems(mapInstance.Data.Id);
        var packet = new MapItemsPacket { Items = new PacketsMapItem[items.Count] };
        for (int i = 0; i < items.Count; i++)
        {
            var mi = items[i];
            packet.Items[i] = new PacketsMapItem
            {
                ItemId = mi.Item.GetId(),
                X = mi.X,
                Y = mi.Y
            };
        }

        Send.ToPlayer(player, packet);
    }

    public static void MapItems(MapInstance mapInstance)
    {
        var items = ServerContext.Instance.GetMapItems(mapInstance.Data.Id);
        var packet = new MapItemsPacket { Items = new PacketsMapItem[items.Count] };
        for (int i = 0; i < items.Count; i++)
        {
            var mi = items[i];
            packet.Items[i] = new PacketsMapItem
            {
                ItemId = mi.Item.GetId(),
                X = mi.X,
                Y = mi.Y
            };
        }

        Send.ToMap(mapInstance, packet);
    }

    private static PlayerDataPacket PlayerDataCache(Player player)
    {
        var pd = player.Get<PlayerDataComponent>();
        var pos = player.Get<PositionComponent>();
        var dir = player.Get<DirectionComponent>();
        var vitals = player.Get<VitalsComponent>();
        var attr = player.Get<AttributeComponent>();
        var equip = player.Get<EquipmentComponent>();

        var packet = new PlayerDataPacket
        {
            Name = pd.Name,
            TextureNum = pd.TextureNum,
            Level = pd.Level,
            MapId = player.MapInstance.GetId(),
            X = pos.X,
            Y = pos.Y,
            Direction = (byte)dir.Value,
            Vital = new short[(byte)Vital.Count],
            MaxVital = new short[(byte)Vital.Count],
            Attribute = new short[(byte)Attribute.Count],
            Equipment = new System.Guid[(byte)Equipment.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            packet.Vital[n] = vitals.Values[n];
            packet.MaxVital[n] = player.MaxVital(n);
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) packet.Attribute[n] = attr.Values[n];
        for (byte n = 0; n < (byte)Equipment.Count; n++) packet.Equipment[n] = equip.Slots[n].GetId();

        return packet;
    }
}
