using CryBits.Client.Core;
using Microsoft.Xna.Framework.Graphics;
using CryBits.Client.Framework.Assets;
using CryBits.Client.Rendering.Camera;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using CryBits.Simulation.Spatial;
using Microsoft.Xna.Framework;
using static CryBits.Definitions.Globals;

namespace CryBits.Client.Rendering.Map;

internal sealed class TilemapRenderer(SpriteBatch spriteBatch, World world, CameraManager cameraManager)
{
    public void DrawLayer(Layer layerType)
    {
        var map = world.CurrentMap;
        if (map == null || map.Chunks.Count == 0) return;

        var sight = cameraManager.TileSight;
        var tint = new Color((uint)map.ColorArgb);

        var startChunkX = (short)(sight.X / ChunkGrid.ChunkSize);
        var startChunkY = (short)(sight.Y / ChunkGrid.ChunkSize);
        var endChunkX = (short)(sight.Width / ChunkGrid.ChunkSize);
        var endChunkY = (short)(sight.Height / ChunkGrid.ChunkSize);

        for (var cx = startChunkX; cx <= endChunkX; cx++)
            for (var cy = startChunkY; cy <= endChunkY; cy++)
            {
                if (!map.Chunks.TryGetValue((cx, cy), out var chunk)) continue;
                if (chunk.Tiles == null) continue;

                var baseX = cx * ChunkGrid.ChunkSize;
                var baseY = cy * ChunkGrid.ChunkSize;

                for (var tx = 0; tx < ChunkGrid.ChunkSize; tx++)
                    for (var ty = 0; ty < ChunkGrid.ChunkSize; ty++)
                    {
                        var tileX = baseX + tx;
                        var tileY = baseY + ty;

                        if (tileX < sight.X || tileX > sight.Width || tileY < sight.Y || tileY > sight.Height)
                            continue;

                        var data = chunk.Tiles[tx, ty];
                        if (data is not { IsVisible: true } || data.Layer != layerType) continue;

                        if (data.IsAutoTile) continue;

                        var texture = Textures.Tiles[data.Texture];
                        var sourceRect = new Rectangle(data.SourceX * Grid, data.SourceY * Grid, Grid, Grid);
                        var destRect = new Rectangle(tileX * Grid, tileY * Grid, Grid, Grid);

                        spriteBatch.Draw(texture, destRect, sourceRect, tint);
                    }
            }
    }

    public void DrawPanorama()
    {
        if (world.CurrentMap == null) return;
        var panorama = world.CurrentMap!.Panorama;
        if (panorama > 0)
            spriteBatch.Draw(Textures.Panoramas[panorama], new Vector2(0, 0), Color.White);
    }
}
