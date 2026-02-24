using System.IO;
using CryBits.Client.Entities;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Persistence.Repositories;
using CryBits.Client.Network.Senders;
using CryBits.Entities;
using CryBits.Extensions;
using CryBits.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal static class MapHandler
{
    [PacketHandler]
    internal static void MapRevision(MapRevisionPacket packet)
    {
        var needed = false;
        var id = packet.MapId;
        var currentRevision = packet.Revision;

        // Clear other player entries
        for (byte i = 0; i < Player.List.Count; i++)
            if (Player.List[i] != Player.Me)
                Player.List.RemoveAt(i);

        // Check whether the map data needs to be downloaded
        if (File.Exists(Directories.MapsData.FullName + id + Directories.Format) ||
            CryBits.Entities.Map.Map.List.ContainsKey(id))
        {
            if (!CryBits.Entities.Map.Map.List.ContainsKey(id))
            {
                MapRepository.Read(id);
                MapInstance.Current.Weather.Update();
                MapInstance.Current.Data.Update();
            }

            if (CryBits.Entities.Map.Map.List[id].Revision != currentRevision)
                needed = true;
        }
        else
            needed = true;

        // Request map data
        MapSender.RequestMap(needed);
    }

    [PacketHandler]
    internal static void Map(MapPacket packet)
    {
        var map = packet.Map;
        var id = map.Id;

        // Store map data
        if (CryBits.Entities.Map.Map.List.ContainsKey(id)) CryBits.Entities.Map.Map.List[id] = map;
        else
        {
            CryBits.Entities.Map.Map.List.Add(id, map);
            MapInstance.List.Add(id, new MapInstance(map));
        }

        MapInstance.Current = MapInstance.List[id];

        // Persist map to disk
        MapRepository.Write(map);

        // Update weather particles and map state
        MapInstance.Current.Weather.UpdateType();
        MapInstance.Current.Data.Update();
    }

    [PacketHandler]
    internal static void JoinMap(JoinMapPacket _)
    {
        // Play map background music if present
        if (string.IsNullOrEmpty(MapInstance.Current.Data.Music))
            Music.Stop();
        else
            Music.Play(MapInstance.Current.Data.Music);
    }

    [PacketHandler]
    internal static void MapItems(MapItemsPacket packet)
    {
        // Item count
        MapInstance.Current.Item = new MapItemInstance[packet.Items.Length];

        // Read all map items
        for (byte i = 0; i < MapInstance.Current.Item.Length; i++)
            MapInstance.Current.Item[i] = new MapItemInstance
            {
                Item = Item.List.Get(packet.Items[i].ItemId),
                X = packet.Items[i].X,
                Y = packet.Items[i].Y
            };
    }
}
