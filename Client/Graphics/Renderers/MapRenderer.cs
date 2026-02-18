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
    public static void MapTiles(byte layerType)
    {
        // Previne erros
        if (TempMap.Current.Data.Name == null) return;

        // Dados
        var tempColor = TempMap.Current.Data.Color;
        var color = new Color(tempColor.R, tempColor.G, tempColor.B);
        var map = TempMap.Current.Data;

        // Desenha todas as camadas dos azulejos
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

                            // Desenha o azulejo
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
        // Desenha os 4 mini azulejos
        for (byte i = 0; i < 4; i++)
        {
            Point destiny = position, source = data.Mini[i];

            // Partes do azulejo
            switch (i)
            {
                case 1: destiny.X += 16; break;
                case 2: destiny.Y += 16; break;
                case 3:
                    destiny.X += 16;
                    destiny.Y += 16;
                    break;
            }

            // Renderiza o mini azulejo
            Renders.Render(Textures.Tiles[data.Texture], new Rectangle(source.X, source.Y, 16, 16),
                new Rectangle(destiny, new Size(16, 16)), cor);
        }
    }

    public static void MapPanorama()
    {
        // Desenha o panorama
        if (TempMap.Current.Data.Panorama > 0)
            Renders.Render(Textures.Panoramas[TempMap.Current.Data.Panorama], new Point(0));
    }

    public static void MapFog()
    {
        var data = TempMap.Current.Data.Fog;

        // Previne erros
        if (data.Texture <= 0) return;

        // Desenha a fumaça
        var textureSize = Textures.Fogs[data.Texture].ToSize();
        for (var x = -1; x <= Map.Width * Grid / textureSize.Width; x++)
        for (var y = -1; y <= Map.Height * Grid / textureSize.Height; y++)
            Renders.Render(Textures.Fogs[data.Texture],
                new Point(x * textureSize.Width + TempMap.Current.Fog.X,
                    y * textureSize.Height + TempMap.Current.Fog.Y), new Color(255, 255, 255, data.Alpha));
    }

    public static void MapWeather()
    {
        byte x = 0;

        // Somente se necessário
        if (TempMap.Current.Data.Weather.Type == 0) return;

        // Textura
        x = TempMap.Current.Data.Weather.Type switch
        {
            Weather.Snowing => 32,
            _ => x
        };

        // Desenha as partículas
        foreach (var weather in TempMap.Current.Weather.Particles)
            if (weather.Visible)
                Renders.Render(Textures.Weather, new Rectangle(x, 0, 32, 32),
                    new Rectangle(weather.X, weather.Y, 32, 32),
                    new Color(255, 255, 255, 150));

        // Trovoadas
        Renders.Render(Textures.Blank, 0, 0, 0, 0, ScreenWidth, ScreenHeight,
            new Color(255, 255, 255, TempMap.Current.Weather.Lightning));
    }

    public static void MapName()
    {
        // Somente se necessário
        if (string.IsNullOrEmpty(TempMap.Current.Data.Name)) return;

        // A cor do texto vária de acordo com a moral do mapa
        var color = TempMap.Current.Data.Moral switch
        {
            Moral.Dangerous => Color.Red,
            _ => Color.White
        };

        // Desenha o nome do mapa
        Renders.DrawText(TempMap.Current.Data.Name, 426, 48, color);
    }

    public static void MapItems()
    {
        // Desenha todos os itens que estão no chão
        for (byte i = 0; i < TempMap.Current.Item.Length; i++)
        {
            var data = TempMap.Current.Item[i];

            // Somente se necessário
            if (data.Item == null) continue;

            // Desenha o item
            var position = new Point(CameraUtils.ConvertX(data.X * Grid), CameraUtils.ConvertY(data.Y * Grid));
            Renders.Render(Textures.Items[data.Item.Texture], position);
        }
    }

    public static void MapBlood()
    {
        // Desenha todos os sangues
        for (byte i = 0; i < TempMap.Current.Blood.Count; i++)
        {
            var data = TempMap.Current.Blood[i];
            Renders.Render(Textures.Blood, CameraUtils.ConvertX(data.X * Grid), CameraUtils.ConvertY(data.Y * Grid),
                data.TextureNum * 32, 0, 32, 32, new Color(255, 255, 255, data.Opacity));
        }
    }
}
