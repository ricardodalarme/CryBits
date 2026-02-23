using System.Drawing;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Client.Utils;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Client.Logic.Camera;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.ECS.Systems;

/// <summary>
/// Issues all draw calls for the current map: panorama, tile layers, fog,
/// weather, blood splatters and dropped items.
///
/// The system reads <see cref="BloodSplatComponent"/> and <see cref="MapItemComponent"/>
/// entities from the world rather than arrays stored on <c>MapInstance</c>.
/// </summary>
internal sealed class MapRenderSystem : IRenderSystem
{
    public void Render(GameContext ctx)
    {
        if (ctx.CurrentMap == null) return;

        DrawPanorama(ctx);
        DrawTiles(ctx, (byte)Layer.Ground);
        DrawBlood(ctx);
        DrawItems(ctx);
    }

    /// <summary>Draw the fringe (above-character) layer. Called after character rendering.</summary>
    public void RenderFringe(GameContext ctx)
    {
        if (ctx.CurrentMap == null) return;

        DrawTiles(ctx, (byte)Layer.Fringe);
        DrawWeather(ctx);
        DrawFog(ctx);
        DrawMapName(ctx);
    }

    // ─── Private draw helpers ────────────────────────────────────────────────

    private static void DrawPanorama(GameContext ctx)
    {
        var panorama = ctx.CurrentMap!.Data.Panorama;
        if (panorama > 0)
            Renders.Render(Textures.Panoramas[panorama], new Point(0));
    }

    private static void DrawTiles(GameContext ctx, byte layerType)
    {
        var data = ctx.CurrentMap!.Data;
        if (data.Name == null) return;

        var raw = data.Color;
        var color = new Color(raw.R, raw.G, raw.B);

        for (byte c = 0; c < data.Layer.Count; c++)
        {
            if (data.Layer[c].Type != layerType) continue;

            for (var x = TileSight.X; x <= TileSight.Width; x++)
                for (var y = TileSight.Y; y <= TileSight.Height; y++)
                {
                    if (Map.OutLimit((short)x, (short)y)) continue;

                    var tile = data.Layer[c].Tile[x, y];
                    if (tile.Texture <= 0) continue;

                    if (!tile.IsAutoTile)
                        Renders.Render(Textures.Tiles[tile.Texture],
                            CameraUtils.ConvertX(x * Grid), CameraUtils.ConvertY(y * Grid),
                            tile.X * Grid, tile.Y * Grid, Grid, Grid, color);
                    else
                        DrawAutoTile(new Point(CameraUtils.ConvertX(x * Grid), CameraUtils.ConvertY(y * Grid)),
                            tile, color);
                }
        }
    }

    private static void DrawAutoTile(Point position, MapTileData data, Color color)
    {
        for (byte i = 0; i < 4; i++)
        {
            var dst = position;
            var src = data.Mini[i];
            switch (i)
            {
                case 1: dst.X += 16; break;
                case 2: dst.Y += 16; break;
                case 3: dst.X += 16; dst.Y += 16; break;
            }
            Renders.Render(Textures.Tiles[data.Texture],
                new Rectangle(src.X, src.Y, 16, 16),
                new Rectangle(dst, new Size(16, 16)), color);
        }
    }

    private static void DrawBlood(GameContext ctx)
    {
        foreach (var (_, blood) in ctx.World.Query<BloodSplatComponent>())
            Renders.Render(Textures.Blood,
                CameraUtils.ConvertX(blood.TileX * Grid),
                CameraUtils.ConvertY(blood.TileY * Grid),
                blood.TextureNum * 32, 0, 32, 32,
                new Color(255, 255, 255, blood.Opacity));
    }

    private static void DrawItems(GameContext ctx)
    {
        foreach (var (_, item) in ctx.World.Query<MapItemComponent>())
        {
            if (item.Item == null) continue;
            Renders.Render(Textures.Items[item.Item.Texture],
                new Point(CameraUtils.ConvertX(item.TileX * Grid), CameraUtils.ConvertY(item.TileY * Grid)));
        }
    }

    private static void DrawWeather(GameContext ctx)
    {
        var weather = ctx.CurrentMap!.Data.Weather;
        if (weather.Type == 0) return;

        byte x = weather.Type == Weather.Snowing ? (byte)32 : (byte)0;

        foreach (var particle in ctx.CurrentMap.Weather.Particles)
            if (particle.Visible)
                Renders.Render(Textures.Weather,
                    new Rectangle(x, 0, 32, 32),
                    new Rectangle(particle.X, particle.Y, 32, 32),
                    new Color(255, 255, 255, 150));

        Renders.Render(Textures.Blank, 0, 0, 0, 0, ScreenWidth, ScreenHeight,
            new Color(255, 255, 255, ctx.CurrentMap.Weather.Lightning));
    }

    private static void DrawFog(GameContext ctx)
    {
        var fog = ctx.CurrentMap!.Data.Fog;
        if (fog.Texture <= 0) return;

        var size = Textures.Fogs[fog.Texture].ToSize();
        for (var x = -1; x <= Map.Width * Grid / size.Width; x++)
            for (var y = -1; y <= Map.Height * Grid / size.Height; y++)
                Renders.Render(Textures.Fogs[fog.Texture],
                    new Point(x * size.Width + ctx.CurrentMap.Fog.X,
                              y * size.Height + ctx.CurrentMap.Fog.Y),
                    new Color(255, 255, 255, fog.Alpha));
    }

    private static void DrawMapName(GameContext ctx)
    {
        var name = ctx.CurrentMap!.Data.Name;
        if (string.IsNullOrEmpty(name)) return;

        var color = ctx.CurrentMap.Data.Moral == Moral.Dangerous ? Color.Red : Color.White;
        Renders.DrawText(name, 426, 48, color);
    }
}
