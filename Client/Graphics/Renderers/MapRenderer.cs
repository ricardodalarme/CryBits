using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Worlds;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class MapRenderer
{
    /// <summary>
    /// Render the tiles for the specified layer type (ground, fringe, etc.).
    /// Tiles are drawn in world space — the SFML view handles panning and culling
    /// beyond <see cref="CameraManager.TileSight"/> is done manually for performance.
    /// </summary>
    public static void MapTiles(byte layerType)
    {
        if (GameContext.Instance.CurrentMap.Data.Name == null) return;

        var tempColor = GameContext.Instance.CurrentMap.Data.Color;
        var color = new Color(tempColor.R, tempColor.G, tempColor.B);
        var map = GameContext.Instance.CurrentMap.Data;
        var sight = CameraManager.Instance.TileSight;

        for (byte c = 0; c < map.Layer.Count; c++)
            if (map.Layer[c].Type == layerType)
                for (var x = sight.X; x <= sight.Width; x++)
                for (var y = sight.Y; y <= sight.Height; y++)
                    if (!Map.OutLimit((short)x, (short)y))
                    {
                        var data = map.Layer[c].Tile[x, y];
                        if (data.Texture > 0)
                        {
                            var srcX = data.X * Grid;
                            var srcY = data.Y * Grid;

                            // Draw at world-pixel position — SFML view handles translation.
                            if (!map.Layer[c].Tile[x, y].IsAutoTile)
                                Renderer.Instance.Draw(Textures.Tiles[data.Texture],
                                    x * Grid, y * Grid, srcX, srcY, Grid, Grid, color);
                            else
                                MapAutoTile(new Point(x * Grid, y * Grid), data, color);
                        }
                    }
    }

    private static void MapAutoTile(Point position, MapTileData data, Color color)
    {
        for (byte i = 0; i < 4; i++)
        {
            Point dest = position, source = data.Mini[i];

            switch (i)
            {
                case 1: dest.X += 16; break;
                case 2: dest.Y += 16; break;
                case 3:
                    dest.X += 16;
                    dest.Y += 16;
                    break;
            }

            Renderer.Instance.Draw(Textures.Tiles[data.Texture],
                new Rectangle(source.X, source.Y, 16, 16),
                new Rectangle(dest, new Size(16, 16)), color);
        }
    }

    /// <summary>Render the map's panorama background at world origin.</summary>
    public static void MapPanorama()
    {
        if (GameContext.Instance.CurrentMap.Data.Panorama > 0)
            Renderer.Instance.Draw(Textures.Panoramas[GameContext.Instance.CurrentMap.Data.Panorama], new Point(0));
    }

    /// <summary>Render current map weather particles and lightning overlay in world space.</summary>
    public static void MapWeather()
    {
        byte x = 0;

        if (GameContext.Instance.CurrentMap.Data.Weather.Type == 0) return;

        x = GameContext.Instance.CurrentMap.Data.Weather.Type switch
        {
            Weather.Snowing => 32,
            _ => x
        };

        foreach (var weather in GameContext.Instance.CurrentMap.Weather.Particles)
            if (weather.Visible)
                Renderer.Instance.Draw(Textures.Weather, new Rectangle(x, 0, 32, 32),
                    new Rectangle(weather.X, weather.Y, 32, 32),
                    new Color(255, 255, 255, 150));

        Renderer.Instance.Draw(Textures.Blank, 0, 0, 0, 0, ScreenWidth, ScreenHeight,
            new Color(255, 255, 255, GameContext.Instance.CurrentMap.Weather.Lightning));
    }

    /// <summary>Render the current map name (color varies by map moral).</summary>
    public static void MapName()
    {
        if (string.IsNullOrEmpty(GameContext.Instance.CurrentMap.Data.Name)) return;

        var color = GameContext.Instance.CurrentMap.Data.Moral switch
        {
            Moral.Dangerous => Color.Red,
            _ => Color.White
        };

        Renderer.Instance.DrawText(GameContext.Instance.CurrentMap.Data.Name, 426, 48, color);
    }
}
