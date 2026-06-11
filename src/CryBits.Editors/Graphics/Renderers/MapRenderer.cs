using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms;
using CryBits.Entities.Map;
using CryBits.Enums;
using SFML.Graphics;
using System.Drawing;
using static CryBits.Editors.Logic.Utils;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Editors.Graphics.Renderers;

internal class MapRenderer(Renderer renderer, MapInstance mapInstance)
{
    public static MapRenderer Instance { get; } = new(Renderer.Instance, MapInstance.Instance);

    public RenderTexture? WinMap;
    public RenderTexture? WinMapTile;

    private void Transparent(IRenderTarget window)
    {
        var textureSize = Textures.Transparent.ToSize();

        for (var x = 0; x <= window.Size.X / textureSize.Width; x++)
            for (var y = 0; y <= window.Size.Y / textureSize.Height; y++)
                renderer.Draw(window, Textures.Transparent, new Point(textureSize.Width * x, textureSize.Height * y));
    }

    /// <summary>
    /// Render the tile-sheet preview for the Maps editor.
    /// </summary>
    public void EditorMapsTile()
    {
        var win = EditorMapsWindow.Instance;
        if (WinMapTile == null || win == null || !win.ModeNormal) return;

        WinMapTile.Clear(Color.Black);

        var texture = Textures.Tiles[win.TileSheetIndex + 1];
        var position = new Point(win.TileScrollX, win.TileScrollY);

        Transparent(WinMapTile);
        renderer.Draw(WinMapTile, texture, new Rectangle(position, texture.ToSize()),
            new Rectangle(new Point(0), texture.ToSize()));
        renderer.DrawRectangle(WinMapTile,
            new Rectangle(new Point(win.TileSource.X - position.X, win.TileSource.Y - position.Y), win.TileSource.Size),
            new Color(165, 42, 42, 250));
        renderer.DrawRectangle(WinMapTile, win.TileMouse.X, win.TileMouse.Y, Grid, Grid, new Color(65, 105, 225, 250));

        WinMapTile.Display();
    }

    /// <summary>
    /// Render the map view for the Maps editor.
    /// </summary>
    public void EditorMapsMap()
    {
        var win = EditorMapsWindow.Instance;
        if (WinMap == null || win == null) return;

        var selected = win.SelectedMap;
        if (selected == null) return;

        WinMap.Clear(Color.Black);

        EditorMapsMapPanorama(selected);
        EditorMapsMapTiles(selected);
        EditorMapsMapWeather(selected);
        EditorMapsMapFog(selected);
        EditorMapsMapGrids(selected);
        EditorMapsMapNpcs(selected);

        WinMap.Display();
    }

    private void EditorMapsMapPanorama(Map map)
    {
        var win = EditorMapsWindow.Instance!;

        if (win.ShowVisualizationSafe && map.Panorama > 0)
        {
            var destiny = new Rectangle
            {
                X = win.MapScrollX * -win.GridZoom,
                Y = win.MapScrollY * -win.GridZoom,
                Size = Textures.Panoramas[map.Panorama].ToSize()
            };
            renderer.Draw(WinMap!, Textures.Panoramas[map.Panorama], win.ZoomRect(destiny));
        }
    }

    private void EditorMapsMapTiles(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        int beginX = win.MapScrollX, beginY = win.MapScrollY;

        for (byte c = 0; c < map.Layer.Count; c++)
        {
            if (!win.IsLayerVisible(c)) continue;

            var color = new Color();
            if (win.ShowEdition && win.ModeNormal)
            {
                if (win.SelectedLayerIndex() >= 0 && c != win.SelectedLayerIndex())
                    color = new Color(255, 255, 255, 150);
            }
            else
                color = new Color(map.Color.R, map.Color.G, map.Color.B);

            for (var x = beginX; x < Map.Width; x++)
                for (var y = beginY; y < Map.Height; y++)
                    if (map.Layer[c].Tile[x, y].Texture > 0)
                    {
                        var data = map.Layer[c].Tile[x, y];
                        var source = new Rectangle(new Point(data.X * Grid, data.Y * Grid), GridSize);
                        var destiny = new Rectangle(new Point((x - beginX) * Grid, (y - beginY) * Grid), GridSize);

                        if (!data.IsAutoTile)
                            renderer.Draw(WinMap!, Textures.Tiles[data.Texture], source, win.ZoomRect(destiny), color);
                        else
                            EditorMapsAutoTile(destiny.Location, data, color);
                    }
        }
    }

    private void EditorMapsAutoTile(Point position, MapTileData data, Color color)
    {
        var win = EditorMapsWindow.Instance!;
        for (byte i = 0; i < 4; i++)
        {
            switch (i)
            {
                case 1: position.X += 16; break;
                case 2: position.Y += 16; break;
                case 3:
                    position.X += 16;
                    position.Y += 16;
                    break;
            }

            renderer.Draw(WinMap!, Textures.Tiles[data.Texture], new Rectangle(data.Mini[i].X, data.Mini[i].Y, 16, 16),
                win.ZoomRect(new Rectangle(position, new Size(16, 16))), color);
        }
    }

    private void EditorMapsMapFog(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (map.Fog.Texture <= 0 || !win.ShowVisualizationSafe) return;

        var textureSize = Textures.Fogs[map.Fog.Texture].ToSize();
        for (var x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
            for (var y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
            {
                var position = new Point(x * textureSize.Width + mapInstance.FogX,
                    y * textureSize.Height + mapInstance.FogY);
                renderer.Draw(WinMap!, Textures.Fogs[map.Fog.Texture], win.ZoomRect(new Rectangle(position, textureSize)),
                    new Color(255, 255, 255, map.Fog.Alpha));
            }
    }

    private void EditorMapsMapWeather(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (!win.ShowVisualizationSafe || map.Weather.Type == Weather.Normal) return;

        byte x = 0;
        if (map.Weather.Type == Weather.Snowing) x = 32;

        for (var i = 0; i < mapInstance.Weather.Length; i++)
            if (mapInstance.Weather[i].Visible)
                renderer.Draw(WinMap!, Textures.Weather, new Rectangle(x, 0, 32, 32),
                    win.ZoomRect(new Rectangle(mapInstance.Weather[i].X, mapInstance.Weather[i].Y, 32, 32)),
                    new Color(255, 255, 255, 150));
    }

    private void EditorMapsMapGrids(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        Rectangle source = win.TileSource, destiny = new();
        var begin = new Point(win.MapSelection.X - win.MapScrollX, win.MapSelection.Y - win.MapScrollY);

        destiny.Location = win.ZoomGrid(begin.X, begin.Y);
        destiny.Size = new Size(source.Width / win.Zoom(), source.Height / win.Zoom());

        if (win.ShowGrid)
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                {
                    renderer.DrawRectangle(WinMap!, x * win.GridZoom, y * win.GridZoom, win.GridZoom, win.GridZoom,
                        new Color(25, 25, 25, 70));
                    EditorMapsMapZones(map, x, y);
                    EditorMapsMapAttributes(map, x, y);
                    EditorMapsMapDirBlock(map, x, y);
                }

        if (!win.AutoTile && win.ModeNormal)
        {
            if (win.ToolPencil)
                renderer.Draw(WinMap!, Textures.Tiles[win.TileSheetIndex + 1], source, destiny);
            else if (win.ToolRectangle)
                for (var x = begin.X; x < begin.X + win.MapSelection.Width; x++)
                    for (var y = begin.Y; y < begin.Y + win.MapSelection.Height; y++)
                        renderer.Draw(WinMap!, Textures.Tiles[win.TileSheetIndex + 1], source,
                            new Rectangle(win.ZoomGrid(x, y), destiny.Size));
        }

        if (!win.ModeAttributes || !win.DirBlockMode)
            renderer.DrawRectangle(WinMap!, destiny.X, destiny.Y, win.MapSelection.Width * win.GridZoom,
                win.MapSelection.Height * win.GridZoom);
    }

    private void EditorMapsMapZones(Map map, byte x, byte y)
    {
        var win = EditorMapsWindow.Instance!;
        var position = new Point((x - win.MapScrollX) * win.GridZoom, (y - win.MapScrollY) * win.GridZoom);
        var zoneNum = map.Attribute[x, y].Zone;
        Color color;

        if (!win.ModeZones || zoneNum == 0) return;

        if (zoneNum % 2 == 0)
            color = new Color((byte)((zoneNum * 42) ^ 3), (byte)(zoneNum * 22), (byte)(zoneNum * 33), 150);
        else
            color = new Color((byte)(zoneNum * 33), (byte)(zoneNum * 22), (byte)(zoneNum * 42), 150 ^ 3);

        renderer.Draw(WinMap!, Textures.Blank, new Rectangle(position, new Size(win.GridZoom, win.GridZoom)), color);
        renderer.DrawText(WinMap!, zoneNum.ToString(), position.X, position.Y, Color.White);
    }

    private void EditorMapsMapAttributes(Map map, byte x, byte y)
    {
        var win = EditorMapsWindow.Instance!;
        var position = new Point((x - win.MapScrollX) * win.GridZoom, (y - win.MapScrollY) * win.GridZoom);
        var attribute = (TileAttribute)map.Attribute[x, y].Type;
        Color color;
        string letter;

        if (!win.ModeAttributes || win.DirBlockMode || attribute == TileAttribute.None) return;

        switch (attribute)
        {
            case TileAttribute.Block:
                letter = "B";
                color = Color.Red;
                break;
            case TileAttribute.Warp:
                letter = "T";
                color = Color.Blue;
                break;
            case TileAttribute.Item:
                letter = "I";
                color = Color.Green;
                break;
            default: return;
        }

        color = new Color(color.R, color.G, color.B, 100);

        renderer.Draw(WinMap!, Textures.Blank, new Rectangle(position, new Size(win.GridZoom, win.GridZoom)), color);
        renderer.DrawText(WinMap!, letter, position.X, position.Y, Color.White);
    }

    private void EditorMapsMapDirBlock(Map map, byte x, byte y)
    {
        var win = EditorMapsWindow.Instance!;
        var tile = new Point(win.MapScrollX + x, win.MapScrollY + y);

        if (!win.ModeAttributes || !win.DirBlockMode) return;
        if (tile.X > map.Attribute.GetUpperBound(0)) return;
        if (tile.Y > map.Attribute.GetUpperBound(1)) return;

        for (byte i = 0; i < (byte)Direction.Count; i++)
        {
            var sourceY = map.Attribute[tile.X, tile.Y].Block[i] ? (byte)8 : (byte)0;
            renderer.Draw(WinMap!, Textures.Directions, x * Grid + Block_Position(i).X, y * Grid + Block_Position(i).Y,
                i * 8, sourceY, 6, 6);
        }
    }

    private void EditorMapsMapNpcs(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (!win.ModeNPCs) return;

        for (byte i = 0; i < map.Npc.Count; i++)
            if (map.Npc[i].Spawn)
            {
                var position = new Point((map.Npc[i].X - win.MapScrollX) * win.GridZoom,
                    (map.Npc[i].Y - win.MapScrollY) * win.GridZoom);
                renderer.Draw(WinMap!, Textures.Blank, new Rectangle(position, new Size(win.GridZoom, win.GridZoom)),
                    new Color(0, 220, 0, 150));
                renderer.DrawText(WinMap!, (i + 1).ToString(), position.X + 10, position.Y + 10, Color.White);
            }
    }
}
