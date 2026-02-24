using Arch.Core;
using Arch.System;
using CryBits.Client.Components;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Client.Utils;

namespace CryBits.Client.Systems;

internal sealed class TextRenderSystem(World world) : BaseSystem<World, int>(world)
{
    private readonly QueryDescription _query = new QueryDescription()
        .WithAll<TransformComponent, TextComponent>();

    public override void Update(in int t)
    {
        World.Query(in _query, (ref TransformComponent transform, ref TextComponent textComp) =>
        {
            var screenX = CameraUtils.ConvertX(transform.X) + textComp.OffsetX;
            var screenY = CameraUtils.ConvertY(transform.Y) + textComp.OffsetY;

            if (textComp.Centered)
            {
                int textWidth = TextUtils.MeasureString(textComp.Text);
                screenX -= textWidth / 2;
            }

            Renders.DrawText(textComp.Text, screenX, screenY, textComp.Color);
        });
    }
}
