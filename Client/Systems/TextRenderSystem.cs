using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Client.Graphics;
using CryBits.Client.Utils;

namespace CryBits.Client.Systems;

internal sealed class TextRenderSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, TextComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query, (ref TransformComponent transform, ref TextComponent text) =>
        {
            var screenX = CameraUtils.ConvertX(transform.X) + text.OffsetX;
            var screenY = CameraUtils.ConvertY(transform.Y) + text.OffsetY;

            if (text.Centered)
            {
                int textWidth = TextUtils.MeasureString(text.Text);
                screenX -= textWidth / 2;
            }

            Renders.DrawText(text.Text, screenX, screenY, text.Color);
        });
    }
}