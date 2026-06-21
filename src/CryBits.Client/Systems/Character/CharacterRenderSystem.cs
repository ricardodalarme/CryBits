using CryBits.Client.Components;
using CryBits.Client.Core;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Graphics;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.State;
using System.Drawing;
using Color = SFML.Graphics.Color;
using TextAlign = CryBits.Definitions.Common.TextAlign;

namespace CryBits.Client.Systems.Character;

internal sealed class CharacterRenderSystem(World world, Renderer renderer) : IClientRenderSystem
{
    private readonly List<(int Y, EntityId Entity)> _drawList = [];

    public void Render()
    {
        _drawList.Clear();

        foreach (var state in world.All)
        {
            var transform = state.Get<TransformComponent>();
            if (transform == null) continue;
            if (!state.Has<SpriteComponent>()) continue;
            if (!state.Has<AnimatedSpriteComponent>()) continue;
            if (!state.Has<PlayerAppearance>()) continue;

            _drawList.Add((transform.Y, state.Id));
        }

        _drawList.Sort(static (a, b) => a.Y.CompareTo(b.Y));

        foreach (var (_, entity) in _drawList)
        {
            var transform = world.Get<TransformComponent>(entity);
            var sprite = world.Get<SpriteComponent>(entity);
            var anim = world.Get<AnimatedSpriteComponent>(entity);
            var name = world.Get<PlayerAppearance>(entity);
            if (transform == null || sprite == null || anim == null || name == null) continue;

            var isHurt = world.Has<HurtComponent>(entity);

            DrawShadow(transform, anim);
            DrawSprite(transform, sprite, anim, isHurt);
            var nameColor = world.Get<NameColorComponent>(entity);
            DrawName(transform, anim, name, nameColor);
        }
    }

    private void DrawShadow(
        TransformComponent transform,
        AnimatedSpriteComponent anim)
    {
        var texture = Textures.Shadow;
        var shadowSize = texture.ToSize();
        var source = new Rectangle(0, 0, shadowSize.Width, shadowSize.Height);

        var dest = new Rectangle(
            transform.X,
            transform.Y + anim.FrameHeight - shadowSize.Height + 5,
            anim.FrameWidth,
            shadowSize.Height);

        renderer.Draw(texture, source, dest);
    }

    private void DrawSprite(
        TransformComponent transform,
        SpriteComponent sprite,
        AnimatedSpriteComponent anim,
        bool isHurt)
    {
        var source = new Rectangle(
            anim.CurrentFrameX * anim.FrameWidth,
            anim.CurrentFrameY * anim.FrameHeight,
            anim.FrameWidth,
            anim.FrameHeight);

        var dest = source with { X = transform.X, Y = transform.Y };

        var tint = isHurt
            ? new Color(205, 125, 125, sprite.Tint.A)
            : sprite.Tint;

        renderer.Draw(sprite.Texture, source, dest, tint);
    }

    private void DrawName(
        TransformComponent transform,
        AnimatedSpriteComponent anim,
        PlayerAppearance appearance,
        NameColorComponent? nameColor)
    {
        var x = transform.X + anim.FrameWidth / 2;
        var y = transform.Y - anim.FrameHeight / 2;
        var color = nameColor?.Value ?? Color.White;
        renderer.DrawText(appearance.Name, x, y, color, TextAlign.Center);
    }
}
