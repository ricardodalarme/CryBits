using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Network.Packets.Server;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using CryBits.Host.Core;
using System;
using System.Collections.Generic;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Host.Network.Senders;

internal sealed class MapSender(PackageSender packageSender, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    public static MapSender Instance { get; } = new(PackageSender.Instance, DefinitionCatalog.Instance);

    public void Map(Session session, Map map)
    {
        packageSender.ToPlayer(session, new MapPacket { Map = map });
    }

    public void Maps(Session session)
    {
        packageSender.ToPlayer(session, new MapsPacket { List = _catalog.Maps });
        foreach (var map in _catalog.Maps.Values) Map(session, map);
    }

    public void MapRevision(EntityId entityId, Map map)
    {
        packageSender.ToPlayer(entityId, new MapRevisionPacket { MapId = map.GetId(), Revision = map.Revision });
    }

    public void MapPlayers(EntityId entityId)
    {
        var pos = WorldHost.Current.Entities.Get(entityId)!.Get<Position>()!;
        for (var i = 0; i < WorldHost.Current.Sessions.Count; i++)
        {
            var session = WorldHost.Current.Sessions[i];
            if (!session.IsPlaying) continue;
            if (session.Character is not { } otherId) continue;
            if (otherId.Equals(entityId)) continue;
            var otherPos = WorldHost.Current.Entities.Get(otherId)?.Get<Position>();
            if (otherPos?.MapId == pos.MapId)
                packageSender.ToPlayer(entityId, PlayerDataCache(otherId));
        }
        packageSender.ToMap(pos.MapId, PlayerDataCache(entityId));
    }

    public void MapItems(EntityId entityId, MapState mapState)
    {
        var items = BuildItemsPacket(mapState);
        if (items.Length > 0)
            packageSender.ToPlayer(entityId, new MapItemsPacket { Items = items });
    }

    public void MapItems(MapState mapState)
    {
        var items = BuildItemsPacket(mapState);
        if (items.Length > 0)
            packageSender.ToMap(mapState.Id, new MapItemsPacket { Items = items });
    }

    public void MapGroundItem(EntityId entityId)
    {
        var entity = WorldHost.Current.Entities.Get(entityId);
        if (entity == null) return;
        var pos = entity.Get<Position>();
        var comp = entity.Get<GroundItem>();
        if (pos == null || comp == null) return;

        var packet = new MapItemsPacket
        {
            Items = [new PacketsMapItem { ItemId = comp.ItemDefId, X = pos.X, Y = pos.Y }]
        };
        packageSender.ToMap(pos.MapId, packet);
    }

    public void RemoveGroundItem(Guid entityId)
    {
        packageSender.ToMap(Guid.Empty, new MapItemsPacket());  // Placeholder: actual removal packet
    }

    private static PacketsMapItem[] BuildItemsPacket(MapState mapState)
    {
        var world = WorldHost.Current;
        var items = new List<PacketsMapItem>();
        foreach (var id in mapState.GroundItemIds)
        {
            var entity = world.Entities.Get(id);
            if (entity == null) continue;
            var pos = entity.Get<Position>();
            var comp = entity.Get<GroundItem>();
            if (pos == null || comp == null) continue;
            items.Add(new PacketsMapItem { ItemId = comp.ItemDefId, X = pos.X, Y = pos.Y });
        }
        return items.ToArray();
    }

    private static PlayerDataPacket PlayerDataCache(EntityId entityId)
    {
        var entity = WorldHost.Current.Entities.Get(entityId)!;
        var appearance = entity.Get<PlayerAppearance>()!;
        var pos = entity.Get<Position>()!;
        var vitals = entity.Get<Vitals>()!;
        var stats = entity.Get<StatBlock>()!;
        var equip = entity.Get<EquipmentState>()!;
        var packet = new PlayerDataPacket
        {
            NetworkId = entityId.Value,
            Name = appearance.Name,
            TextureNum = appearance.TextureNum,
            Level = stats.Level,
            MapId = pos.MapId,
            X = pos.X,
            Y = pos.Y,
            Direction = (byte)pos.Direction,
            Vital = new short[(byte)Vital.Count],
            MaxVital = new short[(byte)Vital.Count],
            Attribute = new short[(byte)Attribute.Count],
            Equipment = new System.Guid[(byte)Equipment.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            packet.Vital[n] = n == 0 ? vitals.Hp : vitals.Mp;
            packet.MaxVital[n] = n == 0 ? vitals.MaxHp : vitals.MaxMp;
        }

        for (byte n = 0; n < (byte)Attribute.Count; n++) packet.Attribute[n] = stats.Attribute[n];
        for (byte n = 0; n < (byte)Equipment.Count; n++) packet.Equipment[n] = equip.Slots[n];

        return packet;
    }
}
