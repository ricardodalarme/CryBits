using System.Drawing;
using CryBits.Client.Entities.TempMap;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Utils;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Client.Logic.Camera;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal static class MapRenderer
{
    /// <summary>
    /// Render the tiles for the specified layer type (ground, fringe, etc.).
    /// </summary>
    /// <param name="layerType">Layer type to render.</param>
    public static void MapTiles(byte layerType)
    {
        if (TempMap.Current.Data.Name == null) return;

        var tempColor = TempMap.Current.Data.Color;
        var color = new Color(tempColor.R, tempColor.G, tempColor.B);
        var map = TempMap.Current.Data;

        for (byte c = 0; c < map.Layer.Count; c++)
            if (map.Layer[c].Type == layerType)
                for (var x = TileSight.X; x <= TileSight.Width; x++)
                    for (var y = TileSight.Y; y <= TileSight.Height; y++)
                        if (!Map.OutLimit((short)x, (short)y))
                        {
                            var data = map.Layer[c].Tile[x, y];
                            if (data.Texture > 0)
                            {
                                var x2 = data.X * Grid;
                                var y2 = data.Y * Grid;

                                if (!map.Layer[c].Tile[x, y].IsAutoTile)
                                    Renders.Render(Textures.Tiles[data.Texture], CameraUtils.ConvertX(x * Grid),
                                        CameraUtils.ConvertY(y * Grid), x2, y2, Grid, Grid, color);
                                else
                                    MapAutoTile(new Point(CameraUtils.ConvertX(x * Grid), CameraUtils.ConvertY(y * Grid)),
                                        data, color);
                            }
                        }
    }

    private static void MapAutoTile(Point position, MapTileData data, Color cor)
    {
        // Render the four 16x16 sub-tiles
        for (byte i = 0; i < 4; i++)
        {
            Point destiny = position, source = data.Mini[i];

            // Tile sub-parts: position the sub-tile destination.
            switch (i)
            {
                case 1: destiny.X += 16; break;
                case 2: destiny.Y += 16; break;
                case 3:
                    destiny.X += 16;
                    destiny.Y += 16;
                    break;
            }

            // Render the sub-tile.
            Renders.Render(Textures.Tiles[data.Texture], new Rectangle(source.X, source.Y, 16, 16),
                new Rectangle(destiny, new Size(16, 16)), cor);
        }
    }

    /// <summary>Render the map's panorama background.</summary>
    public static void MapPanorama()
    {
        if (TempMap.Current.Data.Panorama > 0)
            Renders.Render(Textures.Panoramas[TempMap.Current.Data.Panorama], new Point(0));
    }

    /// <summary>Render map fog overlay.</summary>
    public static void MapFog()
    {
        var data = TempMap.Current.Data.Fog;
        if (data.Texture <= 0) return;

        var textureSize = Textures.Fogs[data.Texture].ToSize();
        for (var x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
            for (var y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
                Renders.Render(Textures.Fogs[data.Texture],
                    new Point(x * textureSize.Width + TempMap.Current.Fog.X,
                        y * textureSize.Height + TempMap.Current.Fog.Y), new Color(255, 255, 255, data.Alpha));
    }

    /// <summary>Render current map weather (particles and lightning overlay).</summary>
    public static void MapWeather()
    {
        byte x = 0;

        if (TempMap.Current.Data.Weather.Type == 0) return;

        x = TempMap.Current.Data.Weather.Type switch
        {
            Weather.Snowing => 32,
            _ => x
        };

        foreach (var weather in TempMap.Current.Weather.Particles)
            if (weather.Visible)
                Renders.Render(Textures.Weather, new Rectangle(x, 0, 32, 32),
                    new Rectangle(weather.X, weather.Y, 32, 32),
                    new Color(255, 255, 255, 150));

        Renders.Render(Textures.Blank, 0, 0, 0, 0, ScreenWidth, ScreenHeight,
            new Color(255, 255, 255, TempMap.Current.Weather.Lightning));
    }

    /// <summary>Render the current map name (color varies by map moral).</summary>
    public static void MapName()
    {
        if (string.IsNullOrEmpty(TempMap.Current.Data.Name)) return;

        var color = TempMap.Current.Data.Moral switch
        {
            Moral.Dangerous => Color.Red,
            _ => Color.White
        };

        Renders.DrawText(TempMap.Current.Data.Name, 426, 48, color);
    }

    /// <summary>Render all ground items on the current map.</summary>
    public static void MapItems()
    {
        for (byte i = 0; i < TempMap.Current.Item.Length; i++)
        {
            var data = TempMap.Current.Item[i];
            if (data.Item == null) continue;

            var position = new Point(CameraUtils.ConvertX(data.X * Grid), CameraUtils.ConvertY(data.Y * Grid));
            Renders.Render(Textures.Items[data.Item.Texture], position);
        }
    }

    /// <summary>Render blood splatters on the map.</summary>
    public static void MapBlood()
    {
        for (byte i = 0; i < TempMap.Current.Blood.Count; i++)
        {
            var data = TempMap.Current.Blood[i];
            Renders.Render(Textures.Blood, CameraUtils.ConvertX(data.X * Grid), CameraUtils.ConvertY(data.Y * Grid),
                data.TextureNum * 32, 0, 32, 32, new Color(255, 255, 255, data.Opacity));
        }
    }
}
