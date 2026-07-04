using CryBits.Client.Framework.Graphics;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Spatial;
using SFML.Graphics;
using SFML.System;
using static CryBits.Definitions.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

internal sealed class MapRenderer(Renderer renderer, GameContext context, CameraManager cameraManager)
{
    public static MapRenderer Instance { get; } = new(Renderer.Instance, GameContext.Instance, CameraManager.Instance);

    private readonly Dictionary<int, VertexArray> _batches = [];

    public void DrawLayer(Layer layerType)
    {
        if (context.CurrentMap == null) return;
        var map = context.CurrentMap;
        if (map == null || map.Chunks.Count == 0) return;

        var sight = cameraManager.TileSight;
        var tint = new Color((byte)(map.ColorArgb >> 16), (byte)(map.ColorArgb >> 8), (byte)map.ColorArgb);

        foreach (var va in _batches.Values)
            va.Clear();

        var startChunkX = (short)(sight.X / ChunkGrid.ChunkSize);
        var startChunkY = (short)(sight.Y / ChunkGrid.ChunkSize);
        var endChunkX = (short)(sight.Width / ChunkGrid.ChunkSize);
        var endChunkY = (short)(sight.Height / ChunkGrid.ChunkSize);

        for (var cx = startChunkX; cx <= endChunkX; cx++)
        {
            for (var cy = startChunkY; cy <= endChunkY; cy++)
            {
                if (!map.Chunks.TryGetValue(((short)cx, (short)cy), out var chunk)) continue;
                if (chunk.Tiles == null) continue;

                var baseX = cx * ChunkGrid.ChunkSize;
                var baseY = cy * ChunkGrid.ChunkSize;

                for (var tx = 0; tx < ChunkGrid.ChunkSize; tx++)
                {
                    for (var ty = 0; ty < ChunkGrid.ChunkSize; ty++)
                    {
                        var tileX = baseX + tx;
                        var tileY = baseY + ty;

                        if (tileX < sight.X || tileX > sight.Width || tileY < sight.Y || tileY > sight.Height)
                            continue;

                        var data = chunk.Tiles[tx, ty];
                        if (data == null || !data.IsVisible || data.Layer != layerType) continue;

                        var va = GetBatch(data.Texture);

                        if (!data.IsAutoTile)
                            AppendTile(va, tileX, tileY, data.SourceX * Grid, data.SourceY * Grid, Grid, Grid, tint);
                    }
                }
            }
        }

        foreach (var (texIndex, va) in _batches)
        {
            if (va.VertexCount == 0) continue;
            renderer.RenderWindow.Draw(va, new RenderStates(Textures.Tiles[texIndex]));
        }
    }

    public void DrawPanorama()
    {
        if (context.CurrentMap == null) return;
        var panorama = context.CurrentMap.Panorama;
        if (panorama > 0)
            renderer.Draw(Textures.Panoramas[panorama], new System.Drawing.Point(0));
    }

    private VertexArray GetBatch(int textureIndex)
    {
        if (_batches.TryGetValue(textureIndex, out var va)) return va;

        va = new VertexArray(PrimitiveType.Triangles);
        _batches[textureIndex] = va;

        return va;
    }

    private static void AppendTile(VertexArray va, int tileX, int tileY, float srcX, float srcY, float w, float h, Color tint) =>
        AppendQuad(va, tileX * Grid, tileY * Grid, srcX, srcY, w, h, tint);

    private static void AppendQuad(VertexArray va, float px, float py, float srcX, float srcY, float w, float h, Color tint)
    {
        va.Append(new Vertex(new Vector2f(px, py), tint, new Vector2f(srcX, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));

        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py + h), tint, new Vector2f(srcX + w, srcY + h)));
    }
}
