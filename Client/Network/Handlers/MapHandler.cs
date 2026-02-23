using System;
using System.Collections.Generic;
using System.IO;
using CryBits.Client.ECS;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network.Senders;
using CryBits.Entities;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class MapHandler
{
    private static GameContext Ctx => GameContext.Instance;

    [PacketHandler]
    internal static void MapRevision(MapRevisionPacket packet)
    {
        var id = packet.MapId;
        var currentRevision = packet.Revision;

        // Remove all remote players from the world — they re-join via PlayerData packets.
        Ctx.ClearAllPlayers();

        // Load the map from cache if we already have it.
        var needed = true;

        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) ||
            CryBits.Entities.Map.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.Map.List.ContainsKey(id))
            {
                MapRepository.Read(id);
                Ctx.CurrentMap?.Weather.Update();
                Ctx.CurrentMap?.Data.Update();
            }

            if (CryBits.Entities.Map.Map.List[id].Revision != currentRevision)
                needed = true;
            else
                needed = false;
        }

        MapSender.RequestMap(needed);

        // Destroy all transient map entities (blood splatters) when entering a new map.
        DestroyBloodEntities();
    }

    [PacketHandler]
    internal static void Map(MapPacket packet)
    {
        var map = packet.Map;
        var id = map.Id;

        if (CryBits.Entities.Map.Map.List.ContainsKey(id))
            CryBits.Entities.Map.Map.List[id] = map;
        else
        {
            CryBits.Entities.Map.Map.List.Add(id, map);
            Ctx.Maps.Add(id, new MapInstance(map));
        }

        Ctx.CurrentMap = Ctx.Maps[id];

        MapRepository.Write(map);

        Ctx.CurrentMap.Weather.UpdateType();
        Ctx.CurrentMap.Data.Update();
    }

    [PacketHandler]
    internal static void JoinMap(JoinMapPacket _)
    {
        if (Ctx.CurrentMap == null) return;

        if (string.IsNullOrEmpty(Ctx.CurrentMap.Data.Music))
            Music.Stop();
        else
            Music.Play(Ctx.CurrentMap.Data.Music);
    }

    [PacketHandler]
    internal static void MapItems(MapItemsPacket packet)
    {
        Ctx.ResetMapItemSlots(packet.Items.Length);

        for (byte i = 0; i < packet.Items.Length; i++)
        {
            var entityId = Ctx.GetOrCreateMapItemEntity(i);
            var item = Ctx.World.Get<ECS.Components.MapItemComponent>(entityId);
            item.Item = Item.List.GetValueOrDefault(packet.Items[i].ItemId);
            item.TileX = packet.Items[i].X;
            item.TileY = packet.Items[i].Y;
        }
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static void DestroyBloodEntities()
    {
        var toDestroy = new List<int>();
        foreach (var (id, _) in Ctx.World.Query<ECS.Components.BloodSplatComponent>())
            toDestroy.Add(id);
        foreach (var id in toDestroy)
            Ctx.World.Destroy(id);
    }
}
