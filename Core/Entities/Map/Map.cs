using System;
using System.Collections.Generic;
using System.Drawing;
using CryBits.Enums;

namespace CryBits.Entities.Map;

[Serializable]
public class Map : Entity
{
    // Lista de dados
    public static Dictionary<Guid, Map> List = [];

    // Tamanho dos mapas
    public const byte Width = 25;
    public const byte Height = 19;

    // Dados
    public short Revision { get; set; }
    public Moral Moral { get; set; }
    public IList<MapLayer> Layer { get; set; } = [];
    public MapAttribute[,] Attribute { get; set; } = new MapAttribute[Width, Height];
    public byte Panorama { get; set; }
    public string Music { get; set; }
    public Color Color { get; set; } = Color.FromArgb(-1);
    public MapWeather Weather { get; set; } = new();
    public MapFog Fog { get; set; } = new();
    public IList<MapNpc> Npc { get; set; } = [];
    public byte Lighting { get; set; } = 100;
    public Map[] Link { get; set; } = new Map[(byte)Direction.Count];

    // Construtor
    public Map()
    {
        Name = "New map";
        Layer.Add(new MapLayer ("Ground"));

        for (byte x = 0; x < Width; x++)
        for (byte y = 0; y < Height; y++)
        {
            Attribute[x, y] = new MapAttribute();
            Layer[0].Tile[x, y] = new MapTileData();
        }
    }

    // Verifica se as coordenas estão no limite do mapa
    public static bool OutLimit(short x, short y) => x >= Width || y >= Height || x < 0 || y < 0;

    public bool TileBlocked(short x, short y)
    {
        // Verifica se o azulejo está bloqueado
        if (OutLimit(x, y)) return true;
        if (Attribute[x, y].Type == (byte)TileAttribute.Block) return true;
        return false;
    }

    public void Update()
    {
        // Atualiza os azulejos necessários
        for (byte x = 0; x < Width; x++)
        for (byte y = 0; y < Height; y++)
        for (byte c = 0; c < Layer.Count; c++)
            if (Layer[c].Tile[x, y].IsAutoTile)
                // Faz os cálculos para a autocriação
                Layer[c].Calculate(x, y);
    }
}