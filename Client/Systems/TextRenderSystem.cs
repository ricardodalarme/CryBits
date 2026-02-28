using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Client.Graphics;
using static CryBits.Client.Utils.TextUtils;

namespace CryBits.Client.Systems;

/// <summary>
/// Draws text labels attached to ECS entities at their world position.
/// Draws in world space — the SFML view handles panning.
/// </summary>
internal sealed class TextRenderSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, TextComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query, (ref TransformComponent transform, ref TextComponent text) =>
        {
            var x = transform.X + text.OffsetX;
            var y = transform.Y + text.OffsetY;

            if (text.Centered)
                x -= MeasureString(text.Text) / 2;

            Renders.DrawText(text.Text, x, y, text.Color);
        });
    }
}
