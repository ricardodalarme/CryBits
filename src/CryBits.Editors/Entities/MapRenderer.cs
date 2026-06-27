using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Maps;
using CryBits.Editors.Graphics;
using CryBits.Editors.Maps;
using SFML.Graphics;
using System.Drawing;
using static CryBits.Definitions.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Editors.Entities;

internal class MapRenderer(Renderer renderer, MapInstance mapInstance)
{
    public static MapRenderer Instance { get; } = new(Renderer.Instance, MapInstance.Instance);

    public RenderTexture? WinMap;
    public RenderTexture? WinMapTile;

    private const int ChunkSize = 32;

    private void Transparent(IRenderTarget window)
    {
        var textureSize = Textures.Transparent.ToSize();
        for (var x = 0; x <= window.Size.X / textureSize.Width; x++)
            for (var y = 0; y <= window.Size.Y / textureSize.Height; y++)
                renderer.Draw(window, Textures.Transparent, new Point(textureSize.Width * x, textureSize.Height * y));
    }

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
        RenderFog(selected);
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
                X = win.MapScrollX * -Grid,
                Y = win.MapScrollY * -Grid,
                Size = Textures.Panoramas[map.Panorama].ToSize()
            };
            renderer.Draw(WinMap!, Textures.Panoramas[map.Panorama], destiny);
        }
    }

    private void EditorMapsMapTiles(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        var scrollX = win.MapScrollX;
        var scrollY = win.MapScrollY;
        var viewW = win.MapCanvasWidth / Grid + ChunkSize;
        var viewH = win.MapCanvasHeight / Grid + ChunkSize;

        var startCx = scrollX / ChunkSize - 1;
        var startCy = scrollY / ChunkSize - 1;
        var endCx = (scrollX + viewW) / ChunkSize + 1;
        var endCy = (scrollY + viewH) / ChunkSize + 1;

        for (var cx = startCx; cx <= endCx; cx++)
        {
            for (var cy = startCy; cy <= endCy; cy++)
            {
                if (!map.Chunks.TryGetValue(new ChunkCoord((short)cx, (short)cy), out var chunk)) continue;
                if (chunk.Tiles == null) continue;

                for (var tx = 0; tx < ChunkSize; tx++)
                {
                    for (var ty = 0; ty < ChunkSize; ty++)
                    {
                        var data = chunk.Tiles[tx, ty];
                        if (data == null || data.Texture <= 0) continue;

                        var worldX = cx * ChunkSize + tx;
                        var worldY = cy * ChunkSize + ty;
                        var screenX = (worldX - scrollX) * Grid;
                        var screenY = (worldY - scrollY) * Grid;

                        var source = new Rectangle(new Point(data.SourceX * Grid, data.SourceY * Grid), new Size(Grid, Grid));
                        renderer.Draw(WinMap!, Textures.Tiles[data.Texture], source,
                            new Rectangle(new Point(screenX, screenY), new Size(Grid, Grid)));
                    }
                }
            }
        }

        // Draw "no chunk" indicator for missing chunks in visible area
        for (var cx = startCx; cx <= endCx; cx++)
        {
            for (var cy = startCy; cy <= endCy; cy++)
            {
                if (map.Chunks.ContainsKey(new ChunkCoord((short)cx, (short)cy))) continue;
                var screenX = (cx * ChunkSize - scrollX) * Grid;
                var screenY = (cy * ChunkSize - scrollY) * Grid;
                for (var tx = 0; tx < 2; tx++)
                    for (var ty = 0; ty < 2; ty++)
                    {
                        var x = screenX + tx * Grid * 16;
                        var y = screenY + ty * Grid * 16;
                        renderer.DrawRectangle(WinMap!, x, y, Grid * 16, Grid * 16,
                            (tx + ty) % 2 == 0 ? new Color(40, 40, 40, 120) : new Color(60, 60, 60, 120));
                    }
            }
        }

        // Attributes overlay
        if (win.ModeAttributes)
            EditorMapsMapAttributes(map, scrollX, scrollY, viewW, viewH);
    }

    private void EditorMapsMapAttributes(Map map, int scrollX, int scrollY, int viewW, int viewH)
    {
        var win = EditorMapsWindow.Instance!;
        var startCx = scrollX / ChunkSize - 1;
        var startCy = scrollY / ChunkSize - 1;
        var endCx = (scrollX + viewW) / ChunkSize + 1;
        var endCy = (scrollY + viewH) / ChunkSize + 1;

        for (var cx = startCx; cx <= endCx; cx++)
        {
            for (var cy = startCy; cy <= endCy; cy++)
            {
                if (!map.Chunks.TryGetValue(new ChunkCoord((short)cx, (short)cy), out var chunk)) continue;
                if (chunk.Tiles == null) continue;

                for (var tx = 0; tx < ChunkSize; tx++)
                {
                    for (var ty = 0; ty < ChunkSize; ty++)
                    {
                        var data = chunk.Tiles[tx, ty];
                        if (data == null) continue;

                        var worldX = cx * ChunkSize + tx;
                        var worldY = cy * ChunkSize + ty;
                        var screenX = (worldX - scrollX) * Grid;
                        var screenY = (worldY - scrollY) * Grid;

                        Color? color = null;
                        string? letter = null;

                        switch (data.Attribute)
                        {
                            case BlockedTile:
                                color = new Color(255, 0, 0, 100); letter = "B"; break;
                            case WarpTile:
                                color = new Color(0, 0, 255, 100); letter = "T"; break;
                            case ItemTile:
                                color = new Color(0, 255, 0, 100); letter = "I"; break;
                            case SpawnTile:
                                color = new Color(255, 165, 0, 100); letter = "Z"; break;
                        }

                        if (color.HasValue)
                        {
                            renderer.Draw(WinMap!, Textures.Blank,
                                new Rectangle(new Point(screenX, screenY), new Size(Grid, Grid)),
                                color.Value);
                            if (letter != null)
                                renderer.DrawText(WinMap!, letter, screenX, screenY, Color.White);
                        }
                    }
                }
            }
        }
    }

    private void RenderFog(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (map.DefaultFog == null || map.DefaultFog.Texture <= 0 || !win.ShowVisualizationSafe) return;

        var textureSize = Textures.Fogs[map.DefaultFog.Texture].ToSize();
        var tilesW = (win.MapCanvasWidth / Grid) / ChunkSize + 2;
        var tilesH = (win.MapCanvasHeight / Grid) / ChunkSize + 2;

        for (var x = -1; x <= tilesW; x++)
            for (var y = -1; y <= tilesH; y++)
            {
                var position = new Point(x * textureSize.Width + mapInstance.FogX,
                    y * textureSize.Height + mapInstance.FogY);
                renderer.Draw(WinMap!, Textures.Fogs[map.DefaultFog.Texture],
                    new Rectangle(position, textureSize),
                    new Color(255, 255, 255, map.DefaultFog.Alpha));
            }
    }

    private void EditorMapsMapWeather(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (!win.ShowVisualizationSafe || map.DefaultWeather == WeatherType.None) return;

        byte srcX = 0;
        if (map.DefaultWeather == WeatherType.Snow) srcX = 32;

        for (var i = 0; i < mapInstance.Weather.Length; i++)
            if (mapInstance.Weather[i].Visible)
                renderer.Draw(WinMap!, Textures.Weather, new Rectangle(srcX, 0, 32, 32),
                    new Rectangle(mapInstance.Weather[i].X, mapInstance.Weather[i].Y, 32, 32),
                    new Color(255, 255, 255, 150));
    }

    private void EditorMapsMapGrids(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        Rectangle source = win.TileSource, destiny = new();
        var begin = new Point(win.MapSelection.X - win.MapScrollX, win.MapSelection.Y - win.MapScrollY);

        destiny.Location = new Point(begin.X * Grid, begin.Y * Grid);
        destiny.Size = new Size(source.Width, source.Height);

        if (win.ShowGrid)
        {
            var scrollX = win.MapScrollX;
            var scrollY = win.MapScrollY;
            var startCx = scrollX / ChunkSize - 1;
            var startCy = scrollY / ChunkSize - 1;
            var endCx = (scrollX + win.MapCanvasWidth / Grid) / ChunkSize + 1;
            var endCy = (scrollY + win.MapCanvasHeight / Grid) / ChunkSize + 1;

            for (var cx = startCx; cx <= endCx; cx++)
                for (var cy = startCy; cy <= endCy; cy++)
                    for (var tx = 0; tx < ChunkSize; tx++)
                        for (var ty = 0; ty < ChunkSize; ty++)
                        {
                            var worldX = cx * ChunkSize + tx;
                            var worldY = cy * ChunkSize + ty;
                            var gx = (worldX - scrollX) * Grid;
                            var gy = (worldY - scrollY) * Grid;
                            renderer.DrawRectangle(WinMap!, gx, gy, Grid, Grid,
                                new Color(25, 25, 25, 70));
                        }
        }

        if (!win.AutoTile && win.ModeNormal)
        {
            if (win.ToolPencil)
                renderer.Draw(WinMap!, Textures.Tiles[win.TileSheetIndex + 1], source, destiny);
            else if (win.ToolRectangle)
                for (var x = begin.X; x < begin.X + win.MapSelection.Width; x++)
                    for (var y = begin.Y; y < begin.Y + win.MapSelection.Height; y++)
                        renderer.Draw(WinMap!, Textures.Tiles[win.TileSheetIndex + 1], source,
                            new Rectangle(new Point(x * Grid, y * Grid), destiny.Size));
        }

        renderer.DrawRectangle(WinMap!, destiny.X, destiny.Y, win.MapSelection.Width * Grid,
            win.MapSelection.Height * Grid);
    }

    private void EditorMapsMapNpcs(Map map)
    {
        var win = EditorMapsWindow.Instance!;
        if (!win.ModeNPCs) return;

        for (byte i = 0; i < map.Npc.Count; i++)
            if (map.Npc[i].Spawn)
            {
                var position = new Point((map.Npc[i].X - win.MapScrollX) * Grid,
                    (map.Npc[i].Y - win.MapScrollY) * Grid);
                renderer.Draw(WinMap!, Textures.Blank, new Rectangle(position, new Size(Grid, Grid)),
                    new Color(0, 220, 0, 150));
                renderer.DrawText(WinMap!, (i + 1).ToString(), position.X + 10, position.Y + 10, Color.White);
            }
    }
}
