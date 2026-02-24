using System.Drawing;
using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Client.Utils;
using Color = SFML.Graphics.Color;

namespace CryBits.Client.Systems;

/// <summary>
/// Renders all entities that have a <see cref="TransformComponent"/> and a
/// <see cref="SpriteComponent"/>.
/// </summary>
internal sealed class SpriteRenderSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, SpriteComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query, (ref TransformComponent transform, ref SpriteComponent sprite) =>
        {
            var screenX = CameraUtils.ConvertX(transform.X);
            var screenY = CameraUtils.ConvertY(transform.Y);
            var source = sprite.SourceRect ?? new Rectangle(Point.Empty, sprite.Texture.ToSize());

            if (screenX + source.Width < 0 || screenX > Globals.ScreenWidth ||
            screenY + source.Height < 0 || screenY > Globals.ScreenHeight)
            {
                return;
            }

            var dest = source with { X = screenX, Y = screenY };
            var color = new Color(255, 255, 255, sprite.Opacity);

            Renders.Render(sprite.Texture, source, dest, color);
        });
    }
}
