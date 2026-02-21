using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Entities.Map;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Systems;
using CryBits.Server.World;
using static CryBits.Utils;

namespace CryBits.Server.Entities;

internal class MapInstance(Guid id, Map map) : Entity(id)
{
    // Map data and runtime caches.
    public readonly Map Data = map;
    public NpcInstance[] Npc = Array.Empty<NpcInstance>();
    public List<MapItemInstance> Item = [];

    public void Logic()
    {
        // Skip all calculations if no players are on the map
        if (!HasPlayers()) return;

        for (byte j = 0; j < Npc.Length; j++) NpcAiSystem.Tick(Npc[j]);
    }

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
        foreach (var account in Account.List)
            if (account.IsPlaying)
                if ((account.Character.X, account.Character.Y, Map: account.Character.MapInstance) == (x, y, this))
                    return account.Character;

        return null;
    }

    public bool HasPlayers()
    {
        // Return true if any player is on this map.
        foreach (var account in Account.List)
            if (account.IsPlaying)
                if (account.Character.MapInstance == this)
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
                    Item.Add(new MapItemInstance(CryBits.Entities.Item.List.Get(new Guid(Data.Attribute[x, y].Data1)),
                        Data.Attribute[x, y].Data2, x, y));
    }

    public bool TileBlocked(byte x, byte y, Direction direction, bool countEntities = true)
    {
        byte nextX = x, nextY = y;

        // Compute next tile coordinates.
        NextTile(direction, ref nextX, ref nextY);

        // Check if the next tile is blocked by map data, attributes or entities.
        if (Data.TileBlocked(nextX, nextY)) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (countEntities && (HasPlayer(nextX, nextY) != null || HasNpc(nextX, nextY) != null)) return true;
        return false;
    }

    public static void Create(Map map, bool isOriginal)
    {
        var id = isOriginal ? map.Id : Guid.NewGuid();
        var tempMap = new MapInstance(id, map);
        GameWorld.Current.Maps.Add(id, tempMap);

        // Initialize NPCs for the map.
        tempMap.Npc = new NpcInstance[map.Npc.Count];
        for (byte i = 0; i < tempMap.Npc.Length; i++)
        {
            tempMap.Npc[i] = new NpcInstance(i, tempMap, map.Npc[i].Npc);
            NpcAiSystem.Spawn(tempMap.Npc[i]);
        }

        // Spawn map items.
        tempMap.SpawnItems();
    }
}
