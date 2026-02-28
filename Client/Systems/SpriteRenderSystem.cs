using System.Drawing;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;

namespace CryBits.Client.Systems;

/// <summary>
/// Renders all entities that have a <see cref="TransformComponent"/> and a
/// <see cref="SpriteComponent"/>.
/// Draws in world space — the SFML view (set by CameraManager) handles panning.
/// </summary>
internal sealed class SpriteRenderSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, SpriteComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query, (ref TransformComponent transform, ref SpriteComponent sprite) =>
        {
            var source = sprite.SourceRect ?? new Rectangle(Point.Empty, sprite.Texture.ToSize());
            var dest = source with { X = transform.X, Y = transform.Y };

            Renderer.Instance.Draw(sprite.Texture, source, dest, sprite.Tint);
        });
    }
}
