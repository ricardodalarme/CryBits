using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Maps;
using CryBits.Editors.Entities;
using CryBits.Editors.Forms.Maps;
using SysRect = System.Drawing.Rectangle;
using SysPoint = System.Drawing.Point;
using Color = Microsoft.Xna.Framework.Color;
using static CryBits.Definitions.Globals;
using static CryBits.Editors.Forms.Maps.MapMath;

namespace CryBits.Editors.Graphics.Renderers;

internal class MapRenderer(Renderer renderer, MapInstance mapInstance, Func<EditorMapsWindow?> getMapsWindow)
{
    public Microsoft.Xna.Framework.Graphics.RenderTarget2D? WinMap { get; set; }
    public Microsoft.Xna.Framework.Graphics.RenderTarget2D? WinMapTile { get; set; }

    private EditorMapsWindow? Window => getMapsWindow();

    public void EditorMapsTile()
    {
        var win = Window;
        if (win is not { ModeNormal: true }) return;

        var texture = Textures.Tiles[win.TileSheetIndex + 1];
        if (texture == null) return;

        var position = new SysPoint(win.TileScrollX, win.TileScrollY);
        renderer.DrawTransparentBackground();
        renderer.Draw(texture, new SysRect(position.X, position.Y, texture.Width, texture.Height),
            new SysRect(0, 0, texture.Width, texture.Height));
        renderer.DrawRectangle(
            new SysRect(new SysPoint(win.TileSource.X - position.X, win.TileSource.Y - position.Y), win.TileSource.Size),
            new Color(165, 42, 42, 250));
        renderer.DrawRectangle(win.TileMouse.X, win.TileMouse.Y, Grid, Grid, new Color(65, 105, 225, 250));
    }

    public void EditorMapsMap()
    {
        var win = Window;
        if (win == null) return;
        var selected = win.SelectedMap;
        if (selected == null) return;

        EditorMapsMapPanorama(selected);
        EditorMapsMapTiles(selected);
        EditorMapsMapWeather(selected);
        RenderFog(selected);
        EditorMapsMapGrids(selected);
        EditorMapsMapNpcs(selected);
    }

    private void EditorMapsMapPanorama(Map map)
    {
        var win = Window!;
        if (win.ShowVisualizationSafe && map.Panorama > 0)
        {
            var texture = Textures.Panoramas[map.Panorama];
            if (texture == null) return;

            var destiny = new SysRect
            {
                X = win.MapScrollX * -Grid,
                Y = win.MapScrollY * -Grid,
                Width = texture.Width,
                Height = texture.Height
            };
            renderer.Draw(texture, destiny);
        }
    }

    private void EditorMapsMapTiles(Map map)
    {
        var win = Window!;
        var scrollX = win.MapScrollX;
        var scrollY = win.MapScrollY;
        var viewW = (win.MapCanvasWidth / Grid) + ChunkSize;
        var viewH = (win.MapCanvasHeight / Grid) + ChunkSize;

        var startCx = (scrollX / ChunkSize) - 1;
        var startCy = (scrollY / ChunkSize) - 1;
        var endCx = ((scrollX + viewW) / ChunkSize) + 1;
        var endCy = ((scrollY + viewH) / ChunkSize) + 1;

        for (var cx = startCx; cx <= endCx; cx++)
            for (var cy = startCy; cy <= endCy; cy++)
            {
                if (!map.Chunks.TryGetValue(new ChunkCoord((short)cx, (short)cy), out var chunk)) continue;
                if (chunk.Tiles == null) continue;

                for (var tx = 0; tx < ChunkSize; tx++)
                    for (var ty = 0; ty < ChunkSize; ty++)
                    {
                        var data = chunk.Tiles[tx, ty];
                        if (data is not { Texture: > 0 }) continue;
                        if (!win.IsLayerVisible(data.Layer)) continue;

                        var worldX = (cx * ChunkSize) + tx;
                        var worldY = (cy * ChunkSize) + ty;
                        var screenX = (worldX - scrollX) * Grid;
                        var screenY = (worldY - scrollY) * Grid;

                        var source = new SysRect(data.SourceX * Grid, data.SourceY * Grid, Grid, Grid);

                        if (Textures.Tiles[data.Texture] is not { } texture) continue;

                        renderer.Draw(texture, source,
                            new SysRect(screenX, screenY, Grid, Grid));
                    }
            }

        // Draw "no chunk" indicator for missing chunks in visible area
        for (var cx = startCx; cx <= endCx; cx++)
            for (var cy = startCy; cy <= endCy; cy++)
            {
                if (map.Chunks.ContainsKey(new ChunkCoord((short)cx, (short)cy))) continue;
                var screenX = ((cx * ChunkSize) - scrollX) * Grid;
                var screenY = ((cy * ChunkSize) - scrollY) * Grid;
                for (var tx = 0; tx < 2; tx++)
                    for (var ty = 0; ty < 2; ty++)
                    {
                        var x = screenX + (tx * Grid * 16);
                        var y = screenY + (ty * Grid * 16);
                        renderer.DrawRectangle(x, y, Grid * 16, Grid * 16,
                            (tx + ty) % 2 == 0 ? new Color(40, 40, 40, 120) : new Color(60, 60, 60, 120));
                    }
            }

        // Attributes overlay
        if (win.ModeAttributes)
            EditorMapsMapAttributes(map, scrollX, scrollY, viewW, viewH);
    }

    private void EditorMapsMapAttributes(Map map, int scrollX, int scrollY, int viewW, int viewH)
    {
        var win = Window!;
        var startCx = (scrollX / ChunkSize) - 1;
        var startCy = (scrollY / ChunkSize) - 1;
        var endCx = ((scrollX + viewW) / ChunkSize) + 1;
        var endCy = ((scrollY + viewH) / ChunkSize) + 1;

        for (var cx = startCx; cx <= endCx; cx++)
            for (var cy = startCy; cy <= endCy; cy++)
            {
                if (!map.Chunks.TryGetValue(new ChunkCoord((short)cx, (short)cy), out var chunk)) continue;
                if (chunk.Tiles == null) continue;

                for (var tx = 0; tx < ChunkSize; tx++)
                    for (var ty = 0; ty < ChunkSize; ty++)
                    {
                        var data = chunk.Tiles[tx, ty];
                        if (data == null) continue;
                        if (!win.IsLayerVisible(data.Layer)) continue;

                        var worldX = (cx * ChunkSize) + tx;
                        var worldY = (cy * ChunkSize) + ty;
                        var screenX = (worldX - scrollX) * Grid;
                        var screenY = (worldY - scrollY) * Grid;

                        Color? color = null;
                        string? letter = null;

                        switch (data.Attribute)
                        {
                            case BlockedTile:
                                color = new Color(255, 0, 0, 100);
                                letter = "B";
                                break;
                            case WarpTile:
                                color = new Color(0, 0, 255, 100);
                                letter = "T";
                                break;
                            case ItemTile:
                                color = new Color(0, 255, 0, 100);
                                letter = "I";
                                break;
                            case SpawnTile:
                                color = new Color(255, 165, 0, 100);
                                letter = "Z";
                                break;
                        }

                        if (color.HasValue)
                        {
                            renderer.Draw(Textures.Blank,
                                new SysRect(screenX, screenY, Grid, Grid),
                                color.Value);
                            if (letter != null)
                                renderer.DrawText((screenX, screenY), letter, Color.White);
                        }
                    }
            }
    }

    private void RenderFog(Map map)
    {
        var win = Window!;
        if (map.DefaultFog is not { Texture: > 0 } || !win.ShowVisualizationSafe) return;

        var texture = Textures.Fogs[map.DefaultFog.Texture];
        if (texture == null) return;

        var tilesW = (win.MapCanvasWidth / Grid / ChunkSize) + 2;
        var tilesH = (win.MapCanvasHeight / Grid / ChunkSize) + 2;

        for (var x = -1; x <= tilesW; x++)
            for (var y = -1; y <= tilesH; y++)
            {
                var position = new SysPoint((x * texture.Width) + mapInstance.FogX,
                    (y * texture.Height) + mapInstance.FogY);
                renderer.Draw(texture,
                    new SysRect(position.X, position.Y, texture.Width, texture.Height),
                    new Color(255, 255, 255, (int)map.DefaultFog.Alpha));
            }
    }

    private void EditorMapsMapWeather(Map map)
    {
        var win = Window!;
        if (!win.ShowVisualizationSafe || map.DefaultWeather == WeatherType.None) return;

        var srcX = 0;
        if (map.DefaultWeather == WeatherType.Snow) srcX = 32;

        foreach (var t in mapInstance.Weather)
            if (t.Visible)
                renderer.Draw(Textures.Weather, new SysRect(srcX, 0, 32, 32),
                    new SysRect(t.X, t.Y, 32, 32),
                    new Color(255, 255, 255, 150));
    }

    private void EditorMapsMapGrids(Map map)
    {
        var win = Window!;
        SysRect source = win.TileSource, destiny = new();
        var begin = new SysPoint(win.MapSelection.X - win.MapScrollX, win.MapSelection.Y - win.MapScrollY);

        destiny.X = begin.X * Grid;
        destiny.Y = begin.Y * Grid;
        destiny.Width = source.Width;
        destiny.Height = source.Height;

        if (win.ShowGrid)
        {
            var scrollX = win.MapScrollX;
            var scrollY = win.MapScrollY;
            var startCx = (scrollX / ChunkSize) - 1;
            var startCy = (scrollY / ChunkSize) - 1;
            var endCx = ((scrollX + (win.MapCanvasWidth / Grid)) / ChunkSize) + 1;
            var endCy = ((scrollY + (win.MapCanvasHeight / Grid)) / ChunkSize) + 1;

            for (var cx = startCx; cx <= endCx; cx++)
                for (var cy = startCy; cy <= endCy; cy++)
                    for (var tx = 0; tx < ChunkSize; tx++)
                        for (var ty = 0; ty < ChunkSize; ty++)
                        {
                            var worldX = (cx * ChunkSize) + tx;
                            var worldY = (cy * ChunkSize) + ty;
                            var gx = (worldX - scrollX) * Grid;
                            var gy = (worldY - scrollY) * Grid;
                            renderer.DrawRectangle(gx, gy, Grid, Grid, new Color(25, 25, 25, 70));
                        }
        }

        if (!win.AutoTile && win.ModeNormal)
        {
            var tileTexture = Textures.Tiles[win.TileSheetIndex + 1];
            if (tileTexture == null) return;
            if (win.ToolPencil)
                renderer.Draw(tileTexture, source, destiny);
            else if (win.ToolRectangle)
                for (var x = begin.X; x < begin.X + win.MapSelection.Width; x++)
                    for (var y = begin.Y; y < begin.Y + win.MapSelection.Height; y++)
                        renderer.Draw(tileTexture, source,
                            new SysRect(x * Grid, y * Grid, destiny.Width, destiny.Height));
        }

        renderer.DrawRectangle(destiny.X, destiny.Y, win.MapSelection.Width * Grid,
            win.MapSelection.Height * Grid);
    }

    private void EditorMapsMapNpcs(Map map)
    {
        var win = Window!;
        if (!win.ModeNPCs) return;

        for (byte i = 0; i < map.Npc.Count; i++)
            if (map.Npc[i].Spawn)
            {
                var position = new SysPoint((map.Npc[i].X - win.MapScrollX) * Grid,
                    (map.Npc[i].Y - win.MapScrollY) * Grid);
                renderer.Draw(Textures.Blank, new SysRect(position.X, position.Y, Grid, Grid),
                    new Color(0, 220, 0, 150));
                renderer.DrawText((position.X + 10, position.Y + 10), (i + 1).ToString(), Color.White);
            }
    }
}
