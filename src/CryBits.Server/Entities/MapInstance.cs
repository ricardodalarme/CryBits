using CryBits.Definitions.Catalog;
using CryBits.Definitions.Common;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Server.Systems.Npc;
using CryBits.Server.World;
using System;
using System.Collections.Generic;
namespace CryBits.Server.Entities;

internal class MapInstance(Guid id, Map map, DefinitionCatalog catalog) : Entity(id)
{
    private readonly DefinitionCatalog _catalog = catalog;

    // Map data and runtime caches.
    public readonly Map Data = map;
    public NpcInstance[] Npc = [];
    public List<MapItemInstance> Item = [];

    public NpcInstance HasNpc(byte x, byte y)
    {
        // Return NPC at the given coordinates if present
        for (byte i = 0; i < Npc.Length; i++)
            if (Npc[i].Alive)
                if (Npc[i].X == x && Npc[i].Y == y)
                    return Npc[i];

        return null;
    }

    public Player HasPlayer(byte x, byte y)
    {
        // Return player at the given coordinates if present
        foreach (var session in GameWorld.Current.Sessions)
            if (session.IsPlaying)
                if ((session.Character!.X, session.Character.Y, session.Character.MapInstance) == (x, y, this))
                    return session.Character;

        return null;
    }

    public bool HasPlayers()
    {
        // Return true if any player is on this map.
        foreach (var session in GameWorld.Current.Sessions)
            if (session.IsPlaying)
                if (session.Character!.MapInstance == this)
                    return true;

        return false;
    }

    public MapItemInstance HasItem(byte x, byte y)
    {
        // Return item at the given coordinates if present.
        for (var i = Item.Count - 1; i >= 0; i--)
            if (Item[i].X == x && Item[i].Y == y)
                return Item[i];

        return null;
    }

    public void SpawnItems()
    {
        // Scan map attributes and spawn static map items.
        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
                if (Data.Attribute[x, y].Type == (byte)TileAttribute.Item)
                    // Add map item.
                    Item.Add(new MapItemInstance(_catalog.Items.Get(new Guid(Data.Attribute[x, y].Data1)),
                        Data.Attribute[x, y].Data2, x, y));
    }

    public bool TileBlocked(byte x, byte y, Direction direction, bool countEntities = true)
    {
        byte nextX = x, nextY = y;

        // Compute next tile coordinates.
        direction.NextTile(ref nextX, ref nextY);

        // Check if the next tile is blocked by map data, attributes or entities.
        if (Data.TileBlocked(nextX, nextY)) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)direction.Reverse()]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (countEntities && (HasPlayer(nextX, nextY) != null || HasNpc(nextX, nextY) != null)) return true;
        return false;
    }

    public static void Create(Map map, bool isOriginal)
    {
        var id = isOriginal ? map.Id : Guid.NewGuid();
        var tempMap = new MapInstance(id, map, DefinitionCatalog.Instance);
        GameWorld.Current.Maps.Add(id, tempMap);

        // Initialize NPCs for the map.
        tempMap.Npc = new NpcInstance[map.Npc.Count];
        for (byte i = 0; i < tempMap.Npc.Length; i++)
        {
            tempMap.Npc[i] = new NpcInstance(i, tempMap, map.Npc[i].Npc);
            NpcBrainSystem.Instance.Spawn(tempMap.Npc[i]);
        }

        // Spawn map items.
        tempMap.SpawnItems();
    }
}
