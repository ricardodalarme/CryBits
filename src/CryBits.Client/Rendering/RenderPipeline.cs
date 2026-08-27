using CryBits.Client.Rendering.Camera;
using CryBits.Client.Rendering.Effects;
using CryBits.Client.Rendering.Entities;
using CryBits.Client.Rendering.Items;
using CryBits.Client.Rendering.Map;
using CryBits.Definitions.Maps;
using CryBits.Simulation.Core;
using Microsoft.Xna.Framework.Graphics;

namespace CryBits.Client.Rendering;

internal sealed class RenderPipeline(World world, SpriteBatch spriteBatch, CameraManager cameraManager)
{
    private readonly SpriteBatch _spriteBatch = spriteBatch;
    private readonly CameraManager _cameraManager = cameraManager;
    private readonly TilemapRenderer _tilemapRenderer = new(spriteBatch, world, cameraManager);
    private readonly IRenderer[] _groundRenderers =
    [
        new GroundSpriteRenderer(world, spriteBatch),
        new EntitySpriteRenderer(world, spriteBatch)
    ];
    private readonly IRenderer[] _fringeRenderers =
    [
        new HealthBarRenderer(world, spriteBatch),
        new WeatherParticleRenderer(world, spriteBatch),
        new FogRenderer(world, spriteBatch)
    ];

    private static readonly BlendState DefaultBlend = BlendState.NonPremultiplied;
    private static readonly SamplerState DefaultSampler = SamplerState.PointClamp;
    private static readonly SpriteSortMode DefaultSort = SpriteSortMode.Deferred;

    public void Present()
    {
        _spriteBatch.Begin(DefaultSort, DefaultBlend, DefaultSampler, transformMatrix: _cameraManager.WorldTransform);

        _tilemapRenderer.DrawPanorama();
        _tilemapRenderer.DrawLayer(Layer.Ground);

        foreach (var renderer in _groundRenderers)
            renderer.Render();

        _tilemapRenderer.DrawLayer(Layer.Fringe);

        foreach (var renderer in _fringeRenderers)
            renderer.Render();

        _spriteBatch.End();
    }
}
