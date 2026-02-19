using System;
using System.Collections.Generic;
using CryBits.Client.Framework.Entities.TempMap;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Utils;

namespace CryBits.Client.Entities.TempMap;

internal class TempMap
{
    // Current temporary map instance
    public static TempMap Current;

    // Map collection
    public static Dictionary<Guid, TempMap> List;

    // Map data
    public readonly Map Data;
    public TempNpc[] Npc;
    public TempMapItems[] Item = Array.Empty<TempMapItems>();
    public List<TempMapBlood> Blood = [];
    public TempMapWeather Weather { get; init; }
    public TempMapFog Fog { get; init; }

    private int _bloodTimer;

    public TempMap(Map data)
    {
        Data = data;
        Weather = new TempMapWeather(data.Weather);
        Fog = new TempMapFog(Data.Fog);
    }

    private bool HasNpc(byte x, byte y)
    {
        // Check if an NPC exists at the given tile
        for (byte i = 0; i < Npc.Length; i++)
            if (Npc[i].Data != null)
                if ((Npc[i].X, Npc[i].Y) == (x, y))
                    return true;

        return false;
    }

    private bool HasPlayer(short x, short y)
    {
        // Check if a player exists at the given tile
        for (byte i = 0; i < Player.List.Count; i++)
            if ((Player.List[i].X, Player.List[i].Y, Player.List[i].Map) == (x, y, this))
                return true;

        return false;
    }

    public bool TileBlocked(byte x, byte y, Direction direction)
    {
        byte nextX = x, nextY = y;

        // calculate the next tile in the given direction
        NextTile(direction, ref nextX, ref nextY);

        // if leaving map, check for a link
        if (Map.OutLimit(nextX, nextY)) return Data.Link[(byte)direction] == null;

        // check blocking attributes and occupants
        if (Data.Attribute[nextX, nextY].Type == (byte)TileAttribute.Block) return true;
        if (Data.Attribute[nextX, nextY].Block[(byte)ReverseDirection(direction)]) return true;
        if (Data.Attribute[x, y].Block[(byte)direction]) return true;
        if (HasPlayer(nextX, nextY) || HasNpc(nextX, nextY)) return true;
        return false;
    }

    public void Logic()
    {
        Fog.Update();
        Weather.Update();

        // Fade out and remove old blood splatters
        if (_bloodTimer < Environment.TickCount)
            for (byte i = 0; i < Blood.Count; i++)
            {
                Blood[i].Opacity--;
                if (Blood[i].Opacity == 0) Blood.RemoveAt(i);
                _bloodTimer = Environment.TickCount + 100;
            }
    }
}