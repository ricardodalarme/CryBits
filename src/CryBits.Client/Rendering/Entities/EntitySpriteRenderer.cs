using CryBits.Client.Components;
using Microsoft.Xna.Framework.Graphics;
using CryBits.Client.Framework.Assets;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using FontStashSharp;
using Microsoft.Xna.Framework;

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
            var frameW = sheet.FrameW(sprite.Texture.Width);
            var frameH = sheet.FrameH(sprite.Texture.Height);
            var isHurt = world.Has<HurtComponent>(entity);

            DrawShadow(transform, frameW, frameH);
            DrawSprite(transform, sprite, anim, frameW, frameH, isHurt);
            var nameColor = world.Get<NameColorComponent>(entity);
            DrawName(transform, name, frameW, frameH, nameColor);
        }
    }

    private void DrawShadow(TransformComponent transform, int frameW, int frameH)
    {
        var texture = Textures.Shadow;
        var source = new Rectangle(0, 0, texture.Width, texture.Height);

        var dest = new Rectangle(
            transform.X,
            transform.Y + frameH - texture.Height + 5,
            frameW,
            texture.Height);

        spriteBatch.Draw(texture, dest, source, Color.White);
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

        var dest = new Rectangle(transform.X, transform.Y, source.Width, source.Height);

        var tint = isHurt
            ? new Color((byte)205, (byte)125, (byte)125, sprite.Tint.A)
            : sprite.Tint;

        spriteBatch.Draw(sprite.Texture, dest, source, tint);
    }

    private void DrawName(
        TransformComponent transform,
        PlayerAppearance appearance,
        int frameW,
        int frameH,
        NameColorComponent? nameColor)
    {
        var font = Fonts.Default;
        var textSize = font.MeasureString(appearance.Name);

        var x = transform.X + (frameW / 2) - textSize.X / 2;
        var y = transform.Y - (frameH / 2);
        var color = nameColor?.Value ?? Color.White;

        spriteBatch.DrawString(font, appearance.Name, new Vector2(x, y), color);
    }
}
