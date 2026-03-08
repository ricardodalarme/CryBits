using System.Collections.Generic;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Managers;
using CryBits.Client.Worlds;
using CryBits.Entities.Map;
using CryBits.Enums;
using SFML.Graphics;
using SFML.System;
using static CryBits.Globals;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Graphics.Renderers;

/// <summary>
/// Renders map tiles using SFML <see cref="VertexArray"/> batching.
/// </summary>
internal sealed class MapRenderer(Renderer renderer, GameContext context, CameraManager cameraManager)
{
    public static MapRenderer Instance { get; } = new(Renderer.Instance, GameContext.Instance, CameraManager.Instance);

    // One VertexArray per tileset texture index — reused across frames.
    // Key = texture index from Textures.Tiles, Value = accumulated geometry for that texture.
    private readonly Dictionary<int, VertexArray> _batches = [];

    /// <summary>
    /// Build and submit all tile geometry for the given layer type.
    /// Regular and auto-tiles are handled automatically.
    /// </summary>
    public void DrawLayer(byte layerType)
    {
        if (context.CurrentMap.Data.Name is null) return;

        var map = context.CurrentMap.Data;
        var sight = cameraManager.TileSight;
        var tc = map.Color;
        var tint = new Color(tc.R, tc.G, tc.B);

        // --- Reset all batches for this frame ---------------------------------
        foreach (var va in _batches.Values)
            va.Clear();

        // --- Accumulate geometry per tile ------------------------------------
        for (byte c = 0; c < map.Layer.Count; c++)
        {
            if (map.Layer[c].Type != layerType) continue;

            for (var x = sight.Position.X; x <= sight.Size.X; x++)
                for (var y = sight.Position.Y; y <= sight.Size.Y; y++)
                {
                    if (Map.OutLimit((short)x, (short)y)) continue;

                    var data = map.Layer[c].Tile[x, y];
                    if (data.Texture <= 0) continue;

                    var va = GetBatch(data.Texture);

                    if (!data.IsAutoTile)
                        AppendTile(va, x, y, data.X * Grid, data.Y * Grid, Grid, Grid, tint);
                    else
                        AppendAutoTile(va, x, y, data, tint);
                }
        }

        // --- Submit one draw call per tileset --------------------------------
        foreach (var (texIndex, va) in _batches)
        {
            if (va.VertexCount == 0) continue;
            renderer.RenderWindow.Draw(va, new RenderStates(Textures.Tiles[texIndex]));
        }
    }

    /// <summary>Render the panorama background as a single sprite (already 1 draw call).</summary>
    public void DrawPanorama()
    {
        var panorama = context.CurrentMap.Data.Panorama;
        if (panorama > 0)
            renderer.Draw(Textures.Panoramas[panorama], new Vector2i(0, 0));
    }

    /// <summary>Render the map name label (drawn in world space near the top-right).</summary>
    public void DrawMapName()
    {
        var name = context.CurrentMap.Data.Name;
        if (string.IsNullOrEmpty(name)) return;

        var color = context.CurrentMap.Data.Moral switch
        {
            Moral.Dangerous => Color.Red,
            _ => Color.White
        };

        renderer.DrawText(name, 426, 48, color);
    }

    // -------------------------------------------------------------------------
    // Internal helpers
    // -------------------------------------------------------------------------

    private VertexArray GetBatch(int textureIndex)
    {
        if (_batches.TryGetValue(textureIndex, out var va)) return va;

        va = new VertexArray(PrimitiveType.Triangles);
        _batches[textureIndex] = va;

        return va;
    }

    /// <summary>
    /// Append two triangles (= one quad) for a standard tile to the array.
    /// Tile coordinates are converted to pixel coordinates by multiplying by <see cref="Grid"/>.
    /// </summary>
    private static void AppendTile(VertexArray va, int tileX, int tileY, float srcX, float srcY, float w, float h,
        Color tint) =>
        AppendQuad(va, tileX * Grid, tileY * Grid, srcX, srcY, w, h, tint);

    private static void AppendQuad(VertexArray va, float px, float py, float srcX, float srcY, float w, float h,
        Color tint)
    {
        // Triangle 1
        va.Append(new Vertex(new Vector2f(px, py), tint, new Vector2f(srcX, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));

        // Triangle 2
        va.Append(new Vertex(new Vector2f(px, py + h), tint, new Vector2f(srcX, srcY + h)));
        va.Append(new Vertex(new Vector2f(px + w, py), tint, new Vector2f(srcX + w, srcY)));
        va.Append(new Vertex(new Vector2f(px + w, py + h), tint, new Vector2f(srcX + w, srcY + h)));
    }

    /// <summary>
    /// Append 4 × 2 triangles for an auto-tile (4 sub-tiles, each 16×16).
    /// </summary>
    private static void AppendAutoTile(VertexArray va, int tileX, int tileY, MapTileData data, Color tint)
    {
        float baseX = tileX * Grid;
        float baseY = tileY * Grid;

        for (byte i = 0; i < 4; i++)
        {
            var srcPt = data.Mini[i];
            var dx = baseX + (i is 1 or 3 ? 16 : 0);
            var dy = baseY + (i is 2 or 3 ? 16 : 0);

            AppendQuad(va, dx, dy, srcPt.X, srcPt.Y, 16f, 16f, tint);
        }
    }
}
