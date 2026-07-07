using CryBits.Client.Components;
using CryBits.Client.Framework.Assets;
using CryBits.Definitions.Characters;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using System.Drawing;
using Color = SFML.Graphics.Color;
using TextAlign = CryBits.Definitions.Common.TextAlign;

namespace CryBits.Client.Rendering.Entities;

internal sealed class EntitySpriteRenderer(World world, SpriteBatch spriteBatch) : IRenderer
{
    private readonly List<(int Y, EntityId Entity)> _drawList = [];

    public void Render()
    {
        _drawList.Clear();

        foreach (var entityId in world.All)
        {
            var transform = world.Get<TransformComponent>(entityId);
            if (transform == null) continue;
            if (!world.Has<SpriteComponent>(entityId)) continue;
            if (!world.Has<AnimationState>(entityId)) continue;
            if (!world.Has<PlayerAppearance>(entityId)) continue;

            _drawList.Add((transform.Y, entityId));
        }

        _drawList.Sort(static (a, b) => a.Y.CompareTo(b.Y));

        foreach (var (_, entity) in _drawList)
        {
            var transform = world.Get<TransformComponent>(entity);
            var sprite = world.Get<SpriteComponent>(entity);
            var anim = world.Get<AnimationState>(entity);
            var name = world.Get<PlayerAppearance>(entity);
            if (transform == null || sprite == null || anim == null || name == null) continue;

            var sheet = SpriteSheet.Default;
            var textureSize = sprite.Texture.ToSize();
            var fw = sheet.FrameW(textureSize.Width);
            var fh = sheet.FrameH(textureSize.Height);
            var isHurt = world.Has<HurtComponent>(entity);

            DrawShadow(transform, fw, fh);
            DrawSprite(transform, sprite, anim, fw, fh, isHurt);
            var nameColor = world.Get<NameColorComponent>(entity);
            DrawName(transform, name, fw, fh, nameColor);
        }
    }

    private void DrawShadow(TransformComponent transform, int frameW, int frameH)
    {
        var texture = Textures.Shadow;
        var shadowSize = texture.ToSize();
        var source = new Rectangle(0, 0, shadowSize.Width, shadowSize.Height);

        var dest = new Rectangle(
            transform.X,
            transform.Y + frameH - shadowSize.Height + 5,
            frameW,
            shadowSize.Height);

        spriteBatch.Draw(texture, source, dest);
    }

    private void DrawSprite(
        TransformComponent transform,
        SpriteComponent sprite,
        AnimationState anim,
        int frameW,
        int frameH,
        bool isHurt)
    {
        var source = new Rectangle(
            anim.FrameX * frameW,
            anim.FrameY * frameH,
            frameW,
            frameH);

        var dest = source with { X = transform.X, Y = transform.Y };

        var tint = isHurt
            ? new Color(205, 125, 125, sprite.Tint.A)
            : sprite.Tint;

        spriteBatch.Draw(sprite.Texture, source, dest, tint);
    }

    private void DrawName(
        TransformComponent transform,
        PlayerAppearance appearance,
        int frameW,
        int frameH,
        NameColorComponent? nameColor)
    {
        var x = transform.X + frameW / 2;
        var y = transform.Y - frameH / 2;
        var color = nameColor?.Value ?? Color.White;
        spriteBatch.DrawText(appearance.Name, x, y, color, TextAlign.Center);
    }
}
